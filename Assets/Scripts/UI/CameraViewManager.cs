using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



//TODO this works (untested) for adding specific cameras, but I need to also get it to work for camera states (ie y coord, orthgrapbic, etc)
[RequireComponent(typeof(UIDocument))]
public class CameraViewManager : MonoBehaviour
{

    private CameraViewController controller;

    private IList<Camera> availableCameras;

    private void Start()
    {
        UIDocument ui = GetComponent<UIDocument>();
        controller = ui.rootVisualElement.Q<CameraViewController>();
        controller.CameraChangeEvent += ChangeCamera;

        InitialseUI();
    }
    private void InitialseUI()
    {
        availableCameras = FindObjectsOfType<Camera>(true);
        foreach (Camera cam in availableCameras)
        {
            controller.AddElement(new CameraViewViewModel() { camera = cam, name = cam.name }); //TODO relies of cam.name
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
