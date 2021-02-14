using SpeckleUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ImportUIController : VisualElement
{
    public ImportUIController()
    {
        this.RegisterCallback<GeometryChangedEvent>(Setup);
    }

    private void Setup(GeometryChangedEvent evt)
    {

        TextField streamIDText = this.Q<TextField>("StreamIDText");
        Button submitButton = this.Q<Button>("SubmitButton");
        Button logoutButton = this.Q<Button>("LogoutButton");

        submitButton.RegisterCallback<ClickEvent>(ev => ImportModel(streamIDText.text));
        logoutButton.RegisterCallback<ClickEvent>(ev => Logout());

        this.UnregisterCallback<GeometryChangedEvent>(Setup);
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
        VisualTreeAsset nextUI = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/UI/Sub Windows/LoginWindow.uxml");
        parent.Add(nextUI.CloneTree());
        RemoveFromHierarchy();
    }

    public new class UxmlFactory : UxmlFactory<ImportUIController, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    { }
}
