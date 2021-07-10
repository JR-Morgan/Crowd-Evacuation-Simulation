using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSimulation.Cameras
{
    public class CameraController2D : MonoBehaviour
    {
        private const float MIN_VALUE = 0.01f;
        
        [SerializeField]
        private KeyCode resetKey = KeyCode.R;
        
        [SerializeField, Range(0, 200)]
        private float translateSpeed = 40, mouseTranslateSpeed = 40;
        [SerializeField, Range(0,10)]
        private float zoomSpeed = 1;

        private IList<Camera> cameras;

        private Vector3 initialPosition;
        [SerializeField, Min(MIN_VALUE)]
        private float initialCameraSize = 50f;
        
        void Awake()
        {
            cameras = GetComponentsInChildren<Camera>();
            initialPosition = transform.position;
        }

        void Update()
        {

            float translateX = 0f, translateZ = 0f;

            // Key Inputs
            translateX += Input.GetAxis("CameraHorizontal") * translateSpeed * Time.deltaTime;
            translateZ += Input.GetAxis("CameraVertical") * translateSpeed * Time.deltaTime;

            // Mouse Inputs
            translateX -= Input.GetAxis("CameraTranslateMode") * Input.GetAxis("Mouse X") * mouseTranslateSpeed * Time.deltaTime;
            translateZ -= Input.GetAxis("CameraTranslateMode") * Input.GetAxis("Mouse Y") * mouseTranslateSpeed * Time.deltaTime;


            //Apply Transform
            transform.Translate(translateX, translateZ, 0f);

            float sizeDelta = Input.GetAxis("Scroll") * zoomSpeed ;
            foreach (var camera in cameras)
            {
                float currentSize = camera.orthographicSize;
                float desiredSize = currentSize - sizeDelta * currentSize;

                camera.orthographicSize = Mathf.Max(desiredSize, MIN_VALUE);
            }

            if (Input.GetKeyDown(resetKey))
            {
                Reset();
            }
        }

        private void Reset()
        {
            transform.position = initialPosition;
            foreach (var camera in cameras)
            {
                camera.orthographicSize = initialCameraSize;
            }
        }
    }
}