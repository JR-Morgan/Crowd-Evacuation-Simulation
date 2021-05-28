using PedestrianSimulation.UI.ViewModels;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Res = UnityEngine.Resources;


namespace PedestrianSimulation.UI.Elements
{

    public class SpeckleWindowElement : VisualElement
    {

        private static readonly VisualTreeAsset window = Res.Load<VisualTreeAsset>(@"UI/Views/SpeckleWindow");

        #region Text Constants
        private const string STREAM_NAME_SEPERATOR = " | ";
        private const string STATUS_PREFIX = "Status: ";
        private const string SERVER_PREFIX = "From ";
        #endregion

        private readonly Dictionary<string, ReceiverElement> recieverElements = new Dictionary<string, ReceiverElement>();

        public Button SimulationSetup { get; private set; }



        public void SetServerName(string serverName)
        {
            this.Q<Foldout>("AvailableStreamsList").text = SERVER_PREFIX + serverName;
        }


        public void AddReceiver(in StreamViewModel viewModel, Action OnHide, Action OnUpdate, Action OnRemove)
        {
            ReceiverElement element = new ReceiverElement();

            SetRecieverText(element, viewModel);

            element.Q<Button>("ButtonHide").RegisterCallback<ClickEvent>(ev => OnHide.Invoke());
            element.Q<Button>("ButtonUpdate").RegisterCallback<ClickEvent>(ev => OnUpdate.Invoke());
            element.Q<Button>("ButtonRemove").RegisterCallback<ClickEvent>(ev => OnRemove.Invoke());

            element.Q<Button>("ButtonHide").SetEnabled(false);

            this.Q("ReceiversParent").Add(element);

            recieverElements.Add(viewModel.streamID, element);
        }


        public void AddAvaiableStreams(in StreamViewModel viewModel, Action OnImport)
        {
            AvailableStreamElement element = new AvailableStreamElement();

            element.Q<Label>("LabelStreamName").text = viewModel.streamName + STREAM_NAME_SEPERATOR + viewModel.streamID;
            element.Q<Label>("LabelStreamDesc").text = viewModel.streamDesc;

            element.Q<Button>("ButtonImport").RegisterCallback<ClickEvent>(ev => {
                OnImport.Invoke();
            });


            this.Q("AvailableStreamsParent").Add(element);
        }

        public void RemoveReceiver(StreamViewModel viewModel)
        {
            if (recieverElements.TryGetValue(viewModel.streamID, out ReceiverElement element))
            {
                recieverElements.Remove(viewModel.streamID);
                element.RemoveFromHierarchy();
            }
        }

        internal void StreamFinished(StreamViewModel viewModel)
        {
            if (recieverElements.TryGetValue(viewModel.streamID, out ReceiverElement element))
            {
                element.Q<Button>("ButtonHide").SetEnabled(true);
                element.Q<Button>("ButtonUpdate").SetEnabled(true);
            }
        }

        internal void SetVisibility(StreamViewModel viewModel, bool visible)
        {
            if (recieverElements.TryGetValue(viewModel.streamID, out ReceiverElement element))
            {
                element.Q<Button>("ButtonHide").text = visible ? "Hide" : "Show";
            }
        }

        public void UpdateReciever(StreamViewModel viewModel)
        {
            if(recieverElements.TryGetValue(viewModel.streamID, out ReceiverElement element))
            {
                element.Q<Button>("ButtonHide").SetEnabled(false);
                element.Q<Button>("ButtonUpdate").SetEnabled(false);
                SetRecieverText(element, viewModel);
            }
        }

        private static void SetRecieverText(ReceiverElement element, StreamViewModel viewModel)
        {
            element.Q<Label>("LabelStreamStatus").text = Math.Round(viewModel.progress, 2).ToString();
            element.Q<Label>("LabelStreamName").text = viewModel.streamName + STREAM_NAME_SEPERATOR + viewModel.streamID;
        }

        public SpeckleWindowElement()
        {
            Add(window.CloneTree());
        }

        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<SpeckleWindowElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        #endregion
    }
}