using PedestrianSimulation.Import.Speckle;
using PedestrianSimulation.UI.Elements;
using PedestrianSimulation.UI.ViewModels;
using Speckle.ConnectorUnity;
using Speckle.Core.Credentials;
using UnityEngine;
using UnityEngine.UIElements;
using Stream = Speckle.Core.Api.Stream;

namespace PedestrianSimulation.UI.Controllers
{
    [AddComponentMenu("Simulation/UI/Import Controller"), DisallowMultipleComponent]
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


            foreach(Transform child in GameObject.FindGameObjectWithTag("Environment").transform)
            {
                StreamViewModel streamViewModel = new StreamViewModel()
                {
                    streamID = "local" + child.GetInstanceID(),
                    streamName = child.name
                };
                ReceiverElement e = element.AddReceiver(
                    viewModel: streamViewModel,
                    OnHide: () => {
                        child.gameObject.SetActive(!child.gameObject.activeInHierarchy);
                        element.SetVisibility(streamViewModel, child.gameObject.activeInHierarchy);
                        },
                    OnUpdate: null,
                    OnRemove: null,
                    enabled: true
                    );

                element.SetVisibility(streamViewModel, child.gameObject.activeInHierarchy);
            }
            





            manager.OnReadyToReceive += Initialise;
        }


        private void Initialise(UserInfo user, ServerInfo server)
        {
            foreach (Stream stream in manager.Streams)
            {
                element.AddAvaiableStreams(
                    viewModel: ToViewModel(stream),
                    OnImport: () => manager.CreateReceiver(stream));
            }

            manager.OnReceiverAdd += AddReceiver;
            manager.OnReceiverRemove += RemoveReceiver;
            manager.OnReceiverUpdate += UpdateReceiver;
            manager.OnStreamVisibilityChange += VisiblityChange;
            manager.OnStreamReceived += StreamReceived;

            element.SetServerName(server.name);
        }
        private void AddReceiver(Stream stream, Receiver receiver)
        {
            element.AddReceiver(ToViewModel(stream), () => manager.HideReceiver(receiver), () => manager.Receive(receiver), () => manager.RemoveReceiver(receiver));
        }

        private void RemoveReceiver(Stream stream, Receiver receiver = null) => RemoveReceiver(ToViewModel(stream));
        private void RemoveReceiver(StreamViewModel stream)
        {
            element.RemoveReceiver(stream);
        }


        private void VisiblityChange(Stream stream, Receiver receiver) => VisiblityChange(ToViewModel(stream), receiver);
        private void VisiblityChange(StreamViewModel stream, Component receiver)
        {
            bool visible = receiver.gameObject.activeInHierarchy;
            element.SetVisibility(stream, visible);
        }

        private void StreamReceived(Stream stream, Receiver receiver = null) => StreamReceived(ToViewModel(stream));
        private void StreamReceived(StreamViewModel stream)
        {
            element.StreamFinished(stream);
        }

        private void UpdateReceiver(Stream stream, Receiver receiver, double progress) => UpdateReceiver(ToViewModel(stream), receiver, progress);
        private void UpdateReceiver(StreamViewModel stream, Component receiver, double progress)
        {
            StreamViewModel s = stream;
            s.progress = progress;
            element.UpdateReciever(s);
        }

        public static StreamViewModel ToViewModel(Stream stream, double progress = 1d) => new StreamViewModel()
        {
            streamName = stream.name,
            streamID = stream.id,
            status = "",
            streamDesc = stream.description,
            progress = progress,
        };

    }
}