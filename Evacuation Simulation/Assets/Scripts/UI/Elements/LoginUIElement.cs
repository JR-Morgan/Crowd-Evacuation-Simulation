using System;
using UnityEditor;
using UnityEngine.UIElements;
using Res = UnityEngine.Resources;

namespace PedestrianSimulation.UI.Elements
{
    public class LoginUIElement : VisualElement
    {
        private static readonly VisualTreeAsset loginWindow = Res.Load<VisualTreeAsset>(@"UI/Views/LoginWindow");


        private Label labelError;

        public LoginUIElement()
        {
            Add(loginWindow.CloneTree());

            void GeometryChange(GeometryChangedEvent evt)
            {
                this.UnregisterCallback<GeometryChangedEvent>(GeometryChange);
                Setup();
            }

            this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
        }

        private void Setup()
        {
            TextField inputServer = this.Q<TextField>("InputServer");
            TextField inputEmail = this.Q<TextField>("InputEmail");
            TextField inputPassword = this.Q<TextField>("InputPassword");
            Button buttonSubmit = this.Q<Button>("ButtonSubmit");
            labelError = this.Q<Label>("LabelError");
            labelError.text = string.Empty;

            buttonSubmit.RegisterCallback<ClickEvent>(ev => {
                this.SetEnabled(false);
                OnSubmit.Invoke(inputServer.text, inputEmail.text, inputPassword.text, (s) => this.SetEnabled(!s));
                });
        }



        public void SetMessage(string message, bool isError = false)
        {
            labelError.text = message;
            //TODO change labelError colour for isError == true or have separate elements for message and error
        }

        #region Events
        public delegate void LoginEventHandler(string server, string email, string password, Action<bool> callback);
        public event LoginEventHandler OnSubmit;
        #endregion



        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<LoginUIElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        #endregion

    }
}