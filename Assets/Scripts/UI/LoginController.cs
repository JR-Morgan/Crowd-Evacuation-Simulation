using Assets.UI.Elements;
using SpeckleUnity;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(ImportController))]
[RequireComponent(typeof(UIDocument))]
public class LoginController : MonoBehaviour
{
    private SpeckleUnityManager speckle;
    private LoginUIElement element;

    private void Start()
    {
        //Speckle setup
        {
            speckle = GameObject.FindGameObjectWithTag("SpeckleManager").GetComponent<SpeckleUnityManager>(); //TODO singleton reference
            if (speckle == null)
            {
                Debug.LogWarning($"Could not find {typeof(SpeckleUnityManager)} in scene");
                return;
            }
        }
    }


    private void SpeckleLogin(string server, string email, string password, Action<bool> callback)
    {
        speckle.SetServerUrl(server);

        string message = null;
        if (string.IsNullOrWhiteSpace(server)) message = "Server is invalid";
        else if (string.IsNullOrWhiteSpace(email)) message = "Email is invalid";
        else if (string.IsNullOrWhiteSpace(password)) message = "Password is invalid";

        if (message == null)
        {
            element.SetMessage("Logging in...");
            speckle.LoginAsync(email, password, u =>
            {
                bool success = false;
                if (u != null)
                {
                    element.SetMessage("Login Successful");
                    success = true;

                    this.GetComponent<ImportController>().enabled = true;
                    this.enabled = false;
                }
                else
                {
                    element.SetMessage("Login Failed");
                    
                }
                callback.Invoke(success);
            });
        }
        else
        {
            element.SetMessage(message, true);
            callback.Invoke(false);
        }
    }


    private void OnEnable()
    {
        DeleteElement();

        UIDocument document = GetComponent<UIDocument>();
        element = document.rootVisualElement.Q<LoginUIElement>();
        if (element == null)
        {
            VisualElement windowContainer = document.rootVisualElement.Q<VisualElement>(ImportController.WINDOW_CONTAINER);
            if (windowContainer == null)
            {
                Debug.LogWarning($"{this} could not find an element of name: \"{ImportController.WINDOW_CONTAINER}\" in {document}");
                return;
            }

            element = new LoginUIElement();
            windowContainer.Add(element);

        }

        element.OnSubmit += SpeckleLogin;
    }

    private void OnDisable() => DeleteElement();

    private void OnDestroy()=> DeleteElement();

    private void DeleteElement()
    {
        if (element != null)
        {
            element.OnSubmit -= SpeckleLogin;
            element.RemoveFromHierarchy();
        }
    }
}
