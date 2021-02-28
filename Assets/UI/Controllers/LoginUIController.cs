using SpeckleUnity;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginUIController : VisualElement
{
    private Label labelError;

    private void Setup(GeometryChangedEvent evt)
    {
        TextField inputServer = this.Q<TextField>("InputServer");
        TextField inputEmail = this.Q<TextField>("InputEmail");
        TextField inputPassword = this.Q<TextField>("InputPassword");
        Button buttonSubmit = this.Q<Button>("ButtonSubmit");
        labelError = this.Q<Label>("LabelError");
        labelError.text = string.Empty;

        buttonSubmit.RegisterCallback<ClickEvent>(ev => SpeckleLogin(inputServer.text, inputEmail.text, inputPassword.text));
    }

    private void SpeckleLogin(string server, string email, string password)
    {

        var speckle = GameObject.FindGameObjectWithTag("SpeckleManager").GetComponent<SpeckleUnityManager>(); //TODO singleton reference

        speckle.SetServerUrl(server);

        string message = null;
        if      (string.IsNullOrWhiteSpace(server))   message = "Server is invalid";
        else if (string.IsNullOrWhiteSpace(email))    message = "Email is invalid";
        else if (string.IsNullOrWhiteSpace(password)) message = "Password is invalid";

        if(message == null)
        {
            labelError.text = "Logging in...";
            speckle.LoginAsync(email, password, u =>
            {
                if (u != null)
                {
                    labelError.text = "Login Successful";
                    
                    VisualTreeAsset nextUI = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/UI/Views/ImportWindow.uxml");
                    //VisualTreeAsset nextUI = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/UI/SubView/Import.uxml");
                    VisualElement element = nextUI.CloneTree();

                    parent.Add(element);
                    RemoveFromHierarchy();
                }
                else
                {
                    labelError.text = "Login Failed";
                }
            });
        }
        else
        {
            labelError.text = message;
        }

    }
    #region UXML Factory
    public new class UxmlFactory : UxmlFactory<LoginUIController, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    { }
    public LoginUIController()
    {
        void GeometryChange(GeometryChangedEvent evt)
        {
            this.UnregisterCallback<GeometryChangedEvent>(Setup);
            Setup(evt);
        }

        this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
    }
    #endregion

}
