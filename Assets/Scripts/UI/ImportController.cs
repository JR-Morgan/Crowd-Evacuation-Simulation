using Assets.UI.Elements;
using Speckle.ConnectorUnity;
using Speckle.Core.Credentials;
using UnityEngine;
using UnityEngine.UIElements;
using Stream = Speckle.Core.Api.Stream;


public struct StreamViewModel
{
    public string streamName, streamID, streamDesc;
    public string status;
    public double progress;
}

[RequireComponent(typeof(UIDocument))]
public class ImportController : MonoBehaviour
{

    ImportManager manager;
    SpeckleWindowElement element;


    private void Start()
    {
        manager = ImportManager.Instance;

        UIDocument document = GetComponent<UIDocument>();

        element = document.rootVisualElement.Q<SpeckleWindowElement>();
        if (element == null)
        {
            Debug.LogWarning($"{this} could not find a {typeof(SpeckleWindowElement)} in {document}");
            return;
        }

        manager.OnReadyToReceive += Initialise;
        manager.OnBusyChange += element.SetBusy;
    }


    private void Initialise(UserInfo user, ServerInfo server)
    {
        foreach (Stream stream in manager.StreamList)
        {
            element.AddAvaiableStreams(
                viewModel: ToViewModel(stream),
                OnImport: () => manager.AddReceiver(stream));
        }

        manager.OnReceiverAdd += AddReceiver;
        manager.OnReceiverRemove += RemoveReceiver;
        manager.OnReceiverUpdate += UpdateReceiver;
        manager.OnStreamVisibilityChange += VisiblityChange;
        manager.OnStreamReceived += StreamReceived;

        element.SetServerName(server.name);
    }


    private void RemoveReceiver(Stream stream, Receiver receiver = null)
    {
        element.RemoveReceiver(ToViewModel(stream));
    }

    private void AddReceiver(Stream stream, Receiver receiver = null)
    {
        element.AddReceiver(ToViewModel(stream), () => manager.HideStream(stream), () => manager.UpdateStream(stream), () => manager.RemoveReceiver(stream));
    }

    private void VisiblityChange(Stream stream, Receiver receiver)
    {
        bool visible = receiver.gameObject.activeInHierarchy;
        element.SetVisibility(ToViewModel(stream), visible);
    }

    private void StreamReceived(Stream stream, Receiver receiver = null)
    {
        element.StreamFinished(ToViewModel(stream));
    }


    private void UpdateReceiver(Stream stream, Receiver receiver, double progress)
    {
        StreamViewModel s = ToViewModel(stream);
        s.progress = progress;
        element.UpdateReciever(s);
    }

    public static StreamViewModel ToViewModel(Stream stream, double progress = 1d) => new StreamViewModel() {
        streamName = stream.name,
        streamID = stream.id,
        status = "",
        streamDesc = stream.description,
        progress = progress,
    };

}
