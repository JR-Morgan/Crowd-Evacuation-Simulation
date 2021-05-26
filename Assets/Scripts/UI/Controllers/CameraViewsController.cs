using PedestrianSimulation.Import.Speckle;
using PedestrianSimulation.UI.Elements;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PedestrianSimulation.UI.Controllers
{
    [AddComponentMenu("Simulation/Managers/Camera Views Controller"), DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public class CameraViewsController : MonoBehaviour
    {

        private CameraViewsElement element;

        private List<GameObject> avaiableCameraGroups;

        private void Start()
        {
            UIDocument document = GetComponent<UIDocument>();
            element = document.rootVisualElement.Q<CameraViewsElement>();
            if (element != null)
            {
                element.CameraChangeEvent += ChangeCamera;

                ImportManager i = ImportManager.Instance;
                if (i != null)
                {
                    i.OnStreamReceived += CameraUpdate;
                    i.OnReceiverRemove += CameraUpdate;
                    i.OnStreamVisibilityChange += CameraUpdate;
                    i.OnReceiverUpdate += CameraUpdate;
                }

                InitialseUI();
            }
            else
            {
                Debug.LogWarning($"{this} could not find a {typeof(CameraViewsElement)} in {document}");
            }
        }
        private void CameraUpdate<A, B, C>(A a = default, B b = default, C c = default) => InitialseUI();
        private void CameraUpdate<A, B>(A a = default, B b = default) => InitialseUI();
        private void InitialseUI()
        {
            element.Clear();
            avaiableCameraGroups = new List<GameObject>();

            //Dynamic cameras
            foreach (GameObject cameraParent in GameObject.FindGameObjectsWithTag("Cameras"))
            {
                foreach (Transform child in cameraParent.transform)
                {
                    if (child.GetComponentInChildren<Camera>(true) != null || child.GetComponentInChildren<Camera>(true) != null)
                    {
                        avaiableCameraGroups.Add(child.gameObject);
                    }
                    else
                    {
                        Debug.LogWarning($"Camera child {child.gameObject} missing {typeof(Camera)} component in self or children", child.gameObject);
                    }
                }
            }

            //Static Cameras
            foreach (GameObject environment in GameObject.FindGameObjectsWithTag("Environment"))
            {
                foreach (Camera cam in environment.GetComponentsInChildren<Camera>(true))
                {
                    avaiableCameraGroups.Add(cam.gameObject);
                }
            }


            //Create UI for all cameras
            foreach (GameObject camGroup in avaiableCameraGroups)
            {
                if (camGroup.transform.parent.gameObject.activeInHierarchy)
                {
                    element.AddElement(new CameraViewViewModel() { cameraGroup = camGroup, name = camGroup.name });
                }
            }
        }

        private void ChangeCamera(GameObject cam)
        {
            foreach (GameObject c in avaiableCameraGroups)
            {
                c.SetActive(false);
            }
            cam.SetActive(true);
        }
    }
}
