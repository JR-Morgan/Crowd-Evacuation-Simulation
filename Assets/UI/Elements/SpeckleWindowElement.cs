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
        #endregion

        private Dictionary<string, RecieverElement> recieverElements = new Dictionary<string, RecieverElement>();

        public void AddReceiver(in StreamViewModel viewModel, Action OnHide, Action OnUpdate, Action OnRemove)
        {
            RecieverElement element = new RecieverElement();

            SetRecieverText(element, viewModel);

            element.Q<Button>("ButtonHide").RegisterCallback<ClickEvent>(ev => OnHide.Invoke());
            element.Q<Button>("ButtonUpdate").RegisterCallback<ClickEvent>(ev => OnUpdate.Invoke());
            element.Q<Button>("ButtonRemove").RegisterCallback<ClickEvent>(ev => OnRemove.Invoke());

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

        public void UpdateReciever(StreamViewModel viewModel)
        {
            if(recieverElements.TryGetValue(viewModel.streamID, out RecieverElement element))
            {
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
        }

        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<SpeckleWindowElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        #endregion
    }
}