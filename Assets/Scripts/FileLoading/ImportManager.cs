using Speckle.ConnectorUnity;
using Speckle.Core.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using JMTools;
using UnityEngine;
using SpeckleStream = Speckle.Core.Api.Stream;
using SpeckleStreams = Speckle.ConnectorUnity.Streams;


namespace PedestrianSimulation.Import.Speckle
{
    /// <summary>
    /// An  <see cref="ImportManager"/> manages <see cref="SpeckleStream"/>s and <see cref="Receiver"/>s
    /// </summary>
    public class ImportManager : Singleton<ImportManager>
    {
        #region Prefab References
        [Tooltip("Instances of this prefab will be created with every Receiver")]
        [SerializeField]
        private GameObject streamParentPrefab;
        #endregion

        [SerializeField]
        [Tooltip("The layer that should be applied to stream objects")]
        private LayerMask layer;

        private Transform parent;

        public Dictionary<string, SpeckleStream> StreamFromID { get; private set; }
        public ICollection<SpeckleStream> Streams => StreamFromID.Values;

        public Dictionary<string, Receiver> Receivers { get; private set; }


        protected override void Awake()
        {
            base.Awake();
            busyReceivers = new BusySet<Receiver>();
            busyReceivers.OnStatusChange += UpdateBusy;
            
            StreamFromID = new Dictionary<string, SpeckleStream>();
            Receivers = new Dictionary<string, Receiver>();
        }

        private async void Start()
        {
            //Initial Checks
            {
                const string environmentTag = "Environment";
                parent = GameObject.FindGameObjectWithTag(environmentTag).transform;
                Debug.Assert(parent != null, $"Could not find parent target GameObject with tag \"{environmentTag}\"", this);
            }
            
            if (streamParentPrefab == null)
            {
                Debug.LogError($"{nameof(streamParentPrefab)} was unassigned", this);
                return;
            }
            var defaultAccount = AccountManager.GetDefaultAccount();
            if (defaultAccount == null)
            {
                Debug.Log("Please set a default account in the Speckle Manager desktop application", this); //TODO proper exceptions that are handled by UI
                return;
            }

            
            //Setup
            List<SpeckleStream> streamList = await SpeckleStreams.List();
            if (!streamList.Any())
            {
                Debug.Log("There are no streams in your account, please create one online."); //TODO proper exceptions that are handled by UI
                return;
            }

            foreach(SpeckleStream s in streamList)
            {
                StreamFromID.Add(s.id, s);
            }

            OnReadyToReceive?.Invoke(defaultAccount.userInfo, defaultAccount.serverInfo);


                //If there is already a model imported 
            GameObject environment = GameObject.FindGameObjectWithTag("Environment");
            if (environment.GetComponentInChildren<Renderer>(false) != null)
            {
                OnBusyChange?.Invoke(false);
            }
        }

        #region Busy
        private BusySet<Receiver> busyReceivers;

        /// <summary><see langword="true" /> when any <see cref="Receiver"/>s are currently receiving</summary>
        public bool IsBusy { get; private set; } = false;
        private void UpdateBusy(bool receiverBusy)
        {
            IsBusy = receiverBusy || parent.GetComponentsInChildren<Transform>().Length <= 1; // 0 (+ 1 for the parent transform)

            OnBusyChange?.Invoke(IsBusy);
        }
        
        public void UpdateBusy() => UpdateBusy(busyReceivers.IsBusy);

        #endregion

        #region Create Receiver

        private Receiver CreateReceiver(SpeckleStream stream, Transform parent, bool autoReceive, bool deleteOld)
        {
            string streamID = stream.id;
            GameObject streamGameObject = Instantiate(streamParentPrefab, Vector3.zero, Quaternion.identity, parent);

            streamGameObject.name = $"Receiver-{streamID}";

            Receiver receiver = streamGameObject.AddComponent<Receiver>();

            receiver.Init(streamID, autoReceive, deleteOld,
                onDataReceivedAction: (go) =>
                {
                    // when the stream has finished being received
                    go.transform.parent = receiver.transform;

                    { //Dissable revit group objects
                        Transform room = go.transform.Find("@Rooms");
                        if(room) room.gameObject.SetActive(false);
                        Transform model = go.transform.Find("@Model Groups");
                        if (model) model.gameObject.SetActive(false);
                    }

                    Debug.Log($"Finished receiving {stream}");
                    busyReceivers.RemoveItem(receiver);
                    HideReceiver(receiver, true);
                    OnStreamReceived?.Invoke(stream, receiver);

                },
                onTotalChildrenCountKnown: (count) => { receiver.TotalChildrenCount = count; },
                onProgressAction: (dict) =>
                {
                    //Run on a dispatcher as GOs can only be retrieved on the main thread
                    Dispatcher.Instance().Enqueue(() =>
                    {
                        //When a part of the model has been received.
                        double val = dict.Values.Average() / receiver.TotalChildrenCount;
                        OnReceiverUpdate?.Invoke(stream, receiver, val);
                    });
                });



            return receiver;
        }
        #endregion

        #region Actions
        /// <summary>
        /// Adds a <paramref name="receiver"/> to the <see cref="ImportManager"/>
        /// </summary>
        /// <param name="receiver">The receiver to be added</param>
        /// <returns>true if the receiver was successfully added</returns>
        public bool RegisterExisting(Receiver receiver)
        {
            string streamID = receiver.StreamId;
            if (Receivers.ContainsKey(streamID))
            {
                Debug.LogWarning($"Failed to add {typeof(Receiver)} because one already existed for {nameof(streamID)}:{streamID}");
                return false;
            }

            if (!StreamFromID.ContainsKey(streamID))
            {
                Debug.LogWarning($"Failed to add {typeof(Receiver)} because no {typeof(SpeckleStream)} with {nameof(streamID)}:{streamID} could be found");
                return false;
            }

            SpeckleStream stream = StreamFromID[streamID];


            Receivers.Add(streamID, receiver);
            OnReceiverAdd?.Invoke(stream, receiver);

            return true;
        }

        /// <summary>
        /// Creates a new <see cref="Receiver"/> for a specified <paramref name="stream"/>
        /// </summary>
        /// <param name="stream">The <see cref="SpeckleStream"/> to be associated with the <see cref="Receiver"/></param>
        /// <param name="receiveNow">When True, the <see cref="Receiver"/> will start receiving once it has been created</param>
        /// <param name="autoReceive">When True, the <see cref="Receiver"/> will automatically receive updates</param>
        /// <param name="deleteOld">When True, the old <see cref="SpeckleStream"/> data will be remove on <see cref="Receiver.Receive"/></param>
        /// <returns>True if the <see cref="Receiver"/> was successfully added to the <see cref="ImportManager"/></returns>
        public bool CreateReceiver(SpeckleStream stream, bool receiveNow = true, bool autoReceive = false, bool deleteOld = true)
        {

            Receiver receiver = CreateReceiver(stream, parent, autoReceive, deleteOld);

            if (RegisterExisting(receiver))
            {
                if (receiveNow) Receive(receiver);

                return true;
            }

            Destroy(receiver.gameObject);
            return false;

        }

        /// <summary>
        /// Calls <see cref="Receiver.Receive"/>
        /// </summary>
        /// <param name="receiver"></param>
        public void Receive(Receiver receiver)
        {
            HideReceiver(receiver, false);

            busyReceivers.AddItem(receiver);
            receiver.Receive();

        }

        /// <summary>
        /// Removes the <paramref name="receiver"/> from the <see cref="ImportManager"/>
        /// </summary>
        /// <param name="receiver">The <see cref="Receiver"/> to be removed</param>
        /// <param name="destroyGameObject">When True, the <paramref name="receiver"/>'s <see cref="Component.gameObject"/> will be removed from the scene</param>
        public void RemoveReceiver(Receiver receiver, bool destroyGameObject = true)
        {
            HideReceiver(receiver, false);

            Receivers.Remove(receiver.StreamId);
            busyReceivers.RemoveItem(receiver);
            UpdateBusy();

            if (destroyGameObject) Destroy(receiver.gameObject);

            OnReceiverRemove?.Invoke(StreamFromID[receiver.StreamId], receiver);
        }

        /// <summary>
        /// Toggles the <paramref name="receiver"/>'s <see cref="Component.gameObject"/> by calling <see cref="GameObject.SetActive(bool)"/>
        /// </summary>
        /// <param name="receiver"></param>
        public void HideReceiver(Receiver receiver)
        {
            HideReceiver(receiver, !receiver.gameObject.activeInHierarchy);
        }


        public void HideReceiver(Receiver receiver, bool value)
        {
            receiver.gameObject.SetActive(value);
            OnStreamVisibilityChange?.Invoke(StreamFromID[receiver.StreamId], receiver);
            UpdateBusy();
        }

        #endregion

        #region Events
        public delegate void ReceiverTransactionEventHandler(SpeckleStream stream, Receiver receiver);
        public event ReceiverTransactionEventHandler OnReceiverRemove;
        public event ReceiverTransactionEventHandler OnReceiverAdd;
        public event ReceiverTransactionEventHandler OnStreamReceived;
        public event ReceiverTransactionEventHandler OnStreamVisibilityChange;

        public delegate void ReceiverReceiveUpdate(SpeckleStream stream, Receiver receiver, double progress);
        public event ReceiverReceiveUpdate OnReceiverUpdate;

        public event Action<UserInfo, ServerInfo> OnReadyToReceive;
        public event Action<bool> OnBusyChange;
        #endregion
    }
}