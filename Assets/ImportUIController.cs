using SpeckleUnity;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class OLDImportUIController : MonoBehaviour
{
    private CallbackEventHandler import;
    private EventCallback<ClickEvent> callback;

    void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        import = rootVisualElement.Q<Button>("ButtonImport");

        TextField serverText = rootVisualElement.Q<TextField>("ServerText");
        TextField emailText = rootVisualElement.Q<TextField>("EmailText");
        TextField passwordText = rootVisualElement.Q<TextField>("PasswordText");
        TextField streamIDText = rootVisualElement.Q<TextField>("StreamIDText");

        callback = ev => ImportModel(serverText.text, emailText.text, passwordText.text, streamIDText.text);

        import.RegisterCallback(callback);
    }

    void OnDisable()
    {
        import.UnregisterCallback(callback);
    }

    private static void ImportModel(string server, string email, string password, string streamID)
    {

        var speckle = GameObject.FindGameObjectWithTag("SpeckleManager").GetComponent<SpeckleUnityManager>(); //TODO singleton reference

        speckle.SetServerUrl(server);

        speckle.LoginAsync(email, password, async u =>
        {
            if(u != null)
            {
                speckle.ClearReceivers();
                await speckle.AddReceiverAsync(streamID, null, true, false);
                await speckle.InitializeAllClientsAsync();
            }
        });

        

    }

}
