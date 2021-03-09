using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.UI.Elements;

[RequireComponent(typeof(UIDocument))]
public class CameraViewsController : MonoBehaviour
{

    private CameraViewsElement element;

    private IList<Camera> availableCameras;

    private void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        element = document.rootVisualElement.Q<CameraViewsElement>();
        if(element != null)
        {
            element.CameraChangeEvent += ChangeCamera;

            ImportManager i = ImportManager.Instance;
            if(i != null)
            {
                i.OnStreamReceived += OnStreamRecieved;
            }

            InitialseUI();
        }
        else
        {
            Debug.LogWarning($"{this} could not find a {typeof(CameraViewsElement)} in {document}");
        }
    }

    private void OnStreamRecieved<S, R>(S stream = default, R reciever = default) => InitialseUI();
    private void InitialseUI()
    {
        element.Clear();
        availableCameras = FindObjectsOfType<Camera>(true);
        foreach (Camera cam in availableCameras)
        {
            element.AddElement(new CameraViewViewModel() { camera = cam, name = cam.name });
        }
        
    }

    private void ChangeCamera(Camera cam)
    {
        foreach(Camera c in availableCameras)
        {
            c.gameObject.SetActive(false);
        }
        cam.gameObject.SetActive(true);
    }
}
