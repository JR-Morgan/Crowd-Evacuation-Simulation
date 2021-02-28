using SpeckleUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ImportUIController : VisualElement
{

    private void Setup(GeometryChangedEvent evt)
    {
        TextField streamIDText = this.Q<TextField>("StreamIDText");
        Button submitButton = this.Q<Button>("SubmitButton");
        Button logoutButton = this.Q<Button>("LogoutButton");

        submitButton.RegisterCallback<ClickEvent>(ev => ImportModel(streamIDText.text));
        logoutButton.RegisterCallback<ClickEvent>(ev => Logout());

    }

    private void ImportModel(string streamID)
    {

        var speckle = GameObject.FindGameObjectWithTag("SpeckleManager").GetComponent<SpeckleUnityManager>(); //TODO singleton reference

        if(speckle.loggedInUser == null)
        {
            Logout();
        }
        else
        {
            speckle.ClearReceivers();
            speckle.AddReceiverAsync(streamID, null, true, false);
            speckle.InitializeAllClientsAsync();
        }

    }

    private void Logout()
    {
        VisualTreeAsset nextUI = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/UI/Views/LoginWindow.uxml");
        SpeckleUnityManager speckle = GameObject.FindGameObjectWithTag("SpeckleManager").GetComponent<SpeckleUnityManager>();
        speckle.Logout();
        parent.Add(nextUI.CloneTree());
        RemoveFromHierarchy();
    }

    #region UXML Factory
    public new class UxmlFactory : UxmlFactory<ImportUIController, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    { }

    public ImportUIController()
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
