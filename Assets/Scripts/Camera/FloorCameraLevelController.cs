using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FloorCameraLevelController : MonoBehaviour
{
    [SerializeField]
    private float floorHeight, groundOffset, relativeOffset;

    [SerializeField]
    private int _floor;

    private new Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }


    public int Floor {
        get => _floor;
        set
        {
            _floor = value;
            SetView();
        }
    }

    private void OnValidate()
    {
        if(camera == null) camera = GetComponent<Camera>();
        SetView();
    }

    private void SetView()
    {
        Vector3 newPosition = camera.transform.position;

        newPosition.y = groundOffset + ((Floor + 1) * (floorHeight + relativeOffset));

        camera.transform.position = newPosition;

        camera.nearClipPlane = relativeOffset;
        camera.farClipPlane = relativeOffset + floorHeight;


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
