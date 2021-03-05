using Assets.UI.Elements;
using SpeckleUnity;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(LoginController))]
[RequireComponent(typeof(UIDocument))]
public class ImportController : MonoBehaviour
{
    public const string WINDOW_CONTAINER = "windowContainer";
    #region SerialisedFeilds
    [SerializeField]
    private bool recieveUpdates = false;
    [SerializeField]
    private Transform root;
    #endregion
    [HideInInspector]
    [SerializeField]
    private SpeckleUnityManager speckle;

    private ImportUIElement element;

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

    private void OnEnable()
    {
        DeleteElement();

        UIDocument document = GetComponent<UIDocument>();
        element = document.rootVisualElement.Q<ImportUIElement>();
        if (element == null)
        {
            VisualElement windowContainer = document.rootVisualElement.Q<VisualElement>(WINDOW_CONTAINER);
            if (windowContainer == null)
            {
                Debug.LogWarning($"{this} could not find an element of name: \"{WINDOW_CONTAINER}\" in {document}");
                return;
            }

            element = new ImportUIElement();
            windowContainer.Add(element);

        }

        element.OnSubmit += ImportModel;
        element.LogoutEvent += Logout;
    }

    private void OnDisable() => DeleteElement();

    private void OnDestroy() => DeleteElement();

    private void DeleteElement()
    {
        if (element != null)
        {
            element.OnSubmit -= ImportModel;
            element.RemoveFromHierarchy();
        }
    }


    private void ImportModel(string streamID)
    {

        var speckle = GameObject.FindGameObjectWithTag("SpeckleManager").GetComponent<SpeckleUnityManager>(); //TODO singleton reference

        if (speckle.loggedInUser == null)
        {
            speckle.Logout();
            Logout();
        }
        else
        {
            speckle.ClearReceivers();
            speckle.AddReceiverAsync(streamID, root, true, recieveUpdates);
            speckle.InitializeAllClientsAsync();
        }

    }


    private void Logout()
    {
        this.GetComponent<LoginController>().enabled = true;
        this.enabled = false;
    }

}
