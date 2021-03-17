using UnityEditor;
using UnityEngine.UIElements;
using Res = UnityEngine.Resources;

namespace Assets.Resources.UI.Elements
{
    public class ImportUIElement : VisualElement
    {
        private static readonly VisualTreeAsset importWindow = Res.Load<VisualTreeAsset>(@"UI/Views/ImportWindow");

        public ImportUIElement()
        {
            this.Add(importWindow.CloneTree());

            void GeometryChange(GeometryChangedEvent evt)
            {
                this.UnregisterCallback<GeometryChangedEvent>(GeometryChange);
                Setup();
            }

            this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
        }

        private void Setup()
        {
            TextField streamIDText = this.Q<TextField>("StreamIDText");
            Button submitButton = this.Q<Button>("SubmitButton");
            Button logoutButton = this.Q<Button>("LogoutButton");

            submitButton.RegisterCallback<ClickEvent>(ev => OnSubmit.Invoke(streamIDText.text));
            logoutButton.RegisterCallback<ClickEvent>(ev => LogoutEvent.Invoke());
        }


        #region Events
        public delegate void ImportModelEventHandler(string streamId);
        public event ImportModelEventHandler OnSubmit;

        public delegate void LogoutEventHandler();
        public event LogoutEventHandler LogoutEvent;
        #endregion

        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<ImportUIElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        #endregion
    }
}
