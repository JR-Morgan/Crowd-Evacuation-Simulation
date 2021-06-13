using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSimulation.World
{
    [RequireComponent(typeof(Camera))]
    public class LegacyFloorCameraLevelController : MonoBehaviour
    {
        [SerializeField]
        private float floorHeight, groundOffset, relativeOffset;

        [SerializeField]
        private int _floor;

        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }


        public int Floor
        {
            get => _floor;
            set
            {
                _floor = value;
                SetView();
            }
        }

        private void OnValidate()
        {
            if (_camera == null) _camera = GetComponent<Camera>();
            SetView();
        }

        private void SetView()
        {
            Vector3 newPosition = _camera.transform.position;

            newPosition.y = groundOffset + ((Floor + 1) * (floorHeight + relativeOffset));

            _camera.transform.position = newPosition;

            _camera.nearClipPlane = relativeOffset;
            _camera.farClipPlane = relativeOffset + floorHeight;


        }

        void Update()
        {
            for (int i = 0; i < 10; i++)
            {
                KeyCode key = KeyCode.Alpha0 + i;
                if (Input.GetKeyDown(key))
                {
                    Floor = i;
                }
            }
        }

    }
}