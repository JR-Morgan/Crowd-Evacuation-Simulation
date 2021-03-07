using Speckle.ConnectorUnity;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Stream = Speckle.Core.Api.Stream;




public class ImportManager : MonoBehaviour
{
    #region Singleton
    private static ImportManager _instance;
    public static ImportManager Instance { get => _instance; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        Receivers = new Dictionary<Stream, Receiver>();

    }
    #endregion

    #region Prefab References
    [SerializeField]
    private GameObject streamParentPrefab;
    #endregion

    public List<Stream> StreamList { get; private set; }

    public Dictionary<Stream, Receiver> Receivers { get; private set; }

    private async void Start()
    {
        if (streamParentPrefab == null)
        {
            Debug.LogError($"{nameof(streamParentPrefab)} was unassigned");
            return;
        }

        var defaultAccount = AccountManager.GetDefaultAccount();
        if (defaultAccount == null)
        {
            Debug.Log("Please set a default account in SpeckleManager");
            return;
        }

        StreamList = await Speckle.ConnectorUnity.Streams.List();
        if (!StreamList.Any())
        {
            Debug.Log("There are no streams in your account, please create one online.");
            return;
        }
        else
        {
            OnReady.Invoke();
        }

    }

    #region Actions
    public void AddReceiver(Stream stream, bool receiveNow = true, bool autoRecieve = false, bool deleteOld = true)
    {
        var streamId = stream.id;
        var autoReceive = autoRecieve;

        var streamPrefab = Instantiate(streamParentPrefab, Vector3.zero, Quaternion.identity);
        streamPrefab.name = $"Receiver-{streamId}";

        var receiver = streamPrefab.AddComponent<Receiver>();

        receiver.Init(streamId, autoReceive, deleteOld,
          onDataReceivedAction: (go) =>
          {
              // when the stream has finished being received
              OnReceiverUpdate.Invoke(stream, receiver);
              Debug.Log($"Finished receiving {stream}");
          },
          onTotalChildrenCountKnown: (count) => { receiver.TotalChildrenCount = count; },
          onProgressAction: (dict) =>
          {
            //Run on a dispatcher as GOs can only be retrieved on the main thread
            Dispatcher.Instance().Enqueue(() =>
              {
                  //When a part of the model has been recieved.
                  double val = dict.Values.Average() / receiver.TotalChildrenCount;
                  OnReceiverUpdate.Invoke(stream, receiver, val);
              });
          });

        Receivers.Add(stream, receiver);
        OnReceiverAdd.Invoke(stream, receiver);
        if (receiveNow) receiver.Receive();
    }

    /// <summary>
    /// Removes the receiver associated with a stream
    /// </summary>
    /// <param name="stream"></param>
    public void RemoveReceiver(Stream stream)
    {
        if (Receivers.TryGetValue(stream, out Receiver receiver))
        {
            Receivers.Remove(stream);
            OnReceiverRemove.Invoke(stream, receiver);
            Destroy(receiver.gameObject);
        }
        else
        {
            Debug.Log($"Tried to hide {typeof(Stream)} {stream} that did not exist in {this}.{nameof(Receivers)}");
        }
    }

    /// <summary>
    /// Hides the receiver associated with a stream
    /// </summary>
    /// <param name="stream"></param>
    public void HideStream(Stream stream)
    {
        if (Receivers.TryGetValue(stream, out Receiver receiver))
        {
            receiver.gameObject.SetActive(!receiver.gameObject.activeInHierarchy);
            OnReceiverUpdate.Invoke(stream, receiver);
        }
        else
        {
            Debug.Log($"Tried to hide {typeof(Stream)} {stream} that did not exist in {this}.{nameof(Receivers)}");
        }
    }

    /// <summary>
    /// Updates the receiver associated with a stream
    /// </summary>
    /// <param name="stream"></param>
    public void UpdateStream(Stream stream)
    {
        if (Receivers.TryGetValue(stream, out Receiver receiver))
        {
            receiver.Receive();
        }
        else
        {
            Debug.Log($"Tried to receive {typeof(Stream)} {stream} that did not exist in {this}.{nameof(Receivers)}");
        }
    }
    #endregion

    #region Events

    public delegate void ReceiverTransactionEventHandler(Stream stream, Receiver receiver);
    public event ReceiverTransactionEventHandler OnReceiverRemove;
    public event ReceiverTransactionEventHandler OnReceiverAdd;

    public delegate void ReceiverReceiveUpdate(Stream stream, Receiver receiver, double progress = 1d);
    public event ReceiverReceiveUpdate OnReceiverUpdate;

    public event Action OnReady;
    #endregion
}
