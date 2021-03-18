using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    [SerializeField]
    private float translateSpeed, mouseTranslateSpeed, zoomSpeed;

    private IList<Camera> cameras;

    void Awake()
    {
        cameras = GetComponentsInChildren<Camera>();
    }

    void Update()
    {

        float translateX = 0f,translateZ = 0f;

        // Key Inputs
        translateX += Input.GetAxis("CameraHorizontal") * translateSpeed *  Time.deltaTime;
        translateZ += Input.GetAxis("CameraVertical") * translateSpeed * Time.deltaTime;

        // Mouse Inputs
        translateX -= Input.GetAxis("CameraTranslateMode") * Input.GetAxis("Mouse X") * mouseTranslateSpeed * Time.deltaTime;
        translateZ -= Input.GetAxis("CameraTranslateMode") * Input.GetAxis("Mouse Y") * mouseTranslateSpeed * Time.deltaTime;      


        //Apply Transform
        transform.Translate(translateX, translateZ, 0f);
        foreach(Camera camera in cameras)
        {
        camera.orthographicSize -= Input.GetAxis("Scroll") * zoomSpeed * Time.deltaTime;

        camera.orthographicSize = Mathf.Max(camera.orthographicSize, 0.01f);

        }



    }

}
