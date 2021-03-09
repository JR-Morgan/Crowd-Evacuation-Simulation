using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;


namespace Assets.UI.Elements
{

    public class SpeckleWindowElement : VisualElement
    {

        private static readonly VisualTreeAsset window = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/UI/Views/SpeckleWindow.uxml");

        #region Text Constants
        private const string STREAM_NAME_SEPERATOR = " | ";
        private const string STATUS_PREFIX = "Status: ";
        private const string SERVER_PREFIX = "From ";
        #endregion

        private readonly Dictionary<string, RecieverElement> recieverElements = new Dictionary<string, RecieverElement>();

        public Button SimulationSetup { get; private set; }

        public void Setup()
        {
            SimulationSetup = this.Q<Button>("ButtonSimulationSetup");
            SimulationSetup.SetEnabled(false);
        }
        public void SetBusy(bool isBusy) => SimulationSetup.SetEnabled(!isBusy);

        public void SetServerName(string serverName)
        {
            this.Q<Foldout>("AvailableStreamsList").text = SERVER_PREFIX + serverName;
        }


        public void AddReceiver(in StreamViewModel viewModel, Action OnHide, Action OnUpdate, Action OnRemove)
        {
            RecieverElement element = new RecieverElement();

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
            if (recieverElements.TryGetValue(viewModel.streamID, out RecieverElement element))
            {
                recieverElements.Remove(viewModel.streamID);
                element.RemoveFromHierarchy();
            }
        }

        internal void StreamFinished(StreamViewModel viewModel)
        {
            if (recieverElements.TryGetValue(viewModel.streamID, out RecieverElement element))
            {
                element.Q<Button>("ButtonHide").SetEnabled(true);
                element.Q<Button>("ButtonUpdate").SetEnabled(true);
            }
        }

        internal void SetVisibility(StreamViewModel viewModel, bool visible)
        {
            if (recieverElements.TryGetValue(viewModel.streamID, out RecieverElement element))
            {
                element.Q<Button>("ButtonHide").text = visible ? "Hide" : "Show";
            }
        }

        public void UpdateReciever(StreamViewModel viewModel)
        {
            if(recieverElements.TryGetValue(viewModel.streamID, out RecieverElement element))
            {
                element.Q<Button>("ButtonHide").SetEnabled(false);
                element.Q<Button>("ButtonUpdate").SetEnabled(false);
                SetRecieverText(element, viewModel);
            }
        }

        private static void SetRecieverText(RecieverElement element, StreamViewModel viewModel)
        {
            element.Q<Label>("LabelStreamStatus").text = Math.Round(viewModel.progress, 2).ToString();
            element.Q<Label>("LabelStreamName").text = viewModel.streamName + STREAM_NAME_SEPERATOR + viewModel.streamID;
        }

        public SpeckleWindowElement()
        {
            Add(window.CloneTree());

            void GeometryChange(GeometryChangedEvent evt)
            {
                this.UnregisterCallback<GeometryChangedEvent>(GeometryChange);
                Setup();
            }

            this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
        }

        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<SpeckleWindowElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        #endregion
    }
}