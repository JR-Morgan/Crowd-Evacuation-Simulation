using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float keyTranslateSpeed = 10f, mouseTranslateSpeed = 150f, mouseRotateSpeed = 2000f;


    [Header("Input Scale")]
    [Range(0f, 10f)]
    public float keyTranslateScaleX = 1f;
    [Range(0f, 10f)]
    public float keyTranslateScaleZ = 1f, keyTranslateModifier = 2f, mouseTranslateScaleX = 1f, mouseTranslateScaleY = 1f, mouseRotateScaleX = 1f, mouseRotateScaleY = 1f;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;
    }


    void Update()
    {

        float translateX = 0f, translateY = 0f, translateZ = 0f;

        // Key Inputs
        translateX += (Input.GetAxis("CameraHorizontal") * keyTranslateScaleX * keyTranslateSpeed * (keyTranslateModifier * Input.GetAxis("CameraTranslateModifier") + 1)) * Time.deltaTime;
        translateZ += (Input.GetAxis("CameraVertical") * keyTranslateScaleZ * keyTranslateSpeed * (keyTranslateModifier * Input.GetAxis("CameraTranslateModifier") + 1)) * Time.deltaTime;

        // Mouse Inputs
        translateX += Input.GetAxis("CameraTranslateMode") * Input.GetAxis("Mouse X") * mouseTranslateScaleX * mouseTranslateSpeed * Time.deltaTime;
        translateY += Input.GetAxis("CameraTranslateMode") * Input.GetAxis("Mouse Y") * mouseTranslateScaleY * mouseTranslateSpeed * Time.deltaTime;

        float rotX = Input.GetAxis("CameraRotateMode") * Input.GetAxis("Mouse Y") * mouseRotateScaleX * mouseRotateSpeed * Time.deltaTime;
        float rotY = Input.GetAxis("CameraRotateMode") * Input.GetAxis("Mouse X") * mouseRotateScaleY * mouseRotateSpeed * Time.deltaTime;


        //Apply Transform
        transform.Translate(translateX, translateY, translateZ);
        transform.Rotate(0f, rotY, 0f, Space.World);
        transform.Rotate(-rotX, 0f, 0f);

    }
}
