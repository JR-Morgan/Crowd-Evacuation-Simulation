using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController2D : MonoBehaviour
{
    [SerializeField]
    private float translateSpeed, mouseTranslateSpeed, zoomSpeed;

    private new Camera camera;

    void Awake()
    {
        camera = GetComponent<Camera>();
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
        camera.orthographicSize -= Input.GetAxis("Scroll") * zoomSpeed * Time.deltaTime;

        camera.orthographicSize = Mathf.Max(camera.orthographicSize, 0f);

    }

}
