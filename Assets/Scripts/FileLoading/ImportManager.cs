using Speckle.ConnectorUnity;
using Speckle.Core.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SpeckleStream = Speckle.Core.Api.Stream;
using SpeckleStreams = Speckle.ConnectorUnity.Streams;


namespace PedestrianSimulation.Import.Speckle
{
    /// <summary>
    /// An  <see cref="ImportManager"/> manages <see cref="Speckle.Core.Api.Stream"/>s and <see cref="Speckle.ConnectorUnity.Receiver"/>s
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

        public Dictionary<string, SpeckleStream> StreamFromID;
        public ICollection<SpeckleStream> Streams => StreamFromID.Values;

        public Dictionary<string, Receiver> Receivers { get; private set; }

        private async void Start()
        {
            //Initial Checks
            if (streamParentPrefab == null)
            {
                Debug.LogError($"{nameof(streamParentPrefab)} was unassigned", this);
                return;
            }
            var defaultAccount = AccountManager.GetDefaultAccount();
            if (defaultAccount == null)
            {
                Debug.Log("Please set a default account in SpeckleManager");
                return;
            }

            //Setup
            StreamFromID = new Dictionary<string, SpeckleStream>();
            Receivers = new Dictionary<string, Receiver>();
            List<SpeckleStream> StreamList = await SpeckleStreams.List();
            if (!StreamList.Any())
            {
                Debug.Log("There are no streams in your account, please create one online.");
                return;
            }
            else
            {
                foreach(SpeckleStream s in StreamList)
                {
                    StreamFromID.Add(s.id, s);
                }

                OnReadyToReceive?.Invoke(defaultAccount.userInfo, defaultAccount.serverInfo);
            }

            busyRecievers = new BusySet<Receiver>();
            busyRecievers.OnStatusChange += UpdateBusy;


            //If there is already a model imported 
            GameObject environment = GameObject.FindGameObjectWithTag("Environment");
            if (environment.GetComponentInChildren<Renderer>(false) != null)
            {
                OnBusyChange(false);
            }
        }

        #region Busy
        private BusySet<Receiver> busyRecievers;

        /// <summary>True when any receivers are currently receiving</summary>
        public bool IsBusy { get; private set; } = false;
        private void UpdateBusy(bool recieverBusy)
        {
            bool oldBusy = IsBusy;

            IsBusy = recieverBusy || Receivers.Count <= 0;

            //if (oldBusy != IsBusy)
            OnBusyChange(IsBusy);
        }
        private void UpdateBusy() => UpdateBusy(busyRecievers.IsBusy);

        #endregion

        #region Create Receiver

        private Receiver CreateReciever(SpeckleStream stream, Transform parent, bool autoReceive, bool deleteOld)
        {
            string streamID = stream.id;
            GameObject streamPrefab = Instantiate(streamParentPrefab, Vector3.zero, Quaternion.identity, parent);

            streamPrefab.name = $"Receiver-{streamID}";

            Receiver receiver = streamPrefab.AddComponent<Receiver>();

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
                    busyRecievers.RemoveItem(receiver);
                    HideReceiver(receiver, true);
                    OnStreamReceived.Invoke(stream, receiver);

                },
                onTotalChildrenCountKnown: (count) => { receiver.TotalChildrenCount = count; },
                onProgressAction: (dict) =>
                {
                    //Run on a dispatcher as GOs can only be retrieved on the main thread
                    Dispatcher.Instance().Enqueue(() =>
                    {
                        //When a part of the model has been received.
                        double val = dict.Values.Average() / receiver.TotalChildrenCount;
                        OnReceiverUpdate.Invoke(stream, receiver, val);
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
            OnReceiverAdd.Invoke(stream, receiver);

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

            Transform parent = GameObject.FindGameObjectWithTag("Environment").transform;

            Receiver receiver = CreateReciever(stream, parent, autoReceive, deleteOld);

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

            busyRecievers.AddItem(receiver);
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
            busyRecievers.RemoveItem(receiver);
            UpdateBusy();

            if (destroyGameObject) Destroy(receiver.gameObject);

            OnReceiverRemove.Invoke(StreamFromID[receiver.StreamId], receiver);
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
            OnStreamVisibilityChange.Invoke(StreamFromID[receiver.StreamId], receiver);
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