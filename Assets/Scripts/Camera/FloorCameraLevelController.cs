using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FloorCameraLevelController : MonoBehaviour
{
    [SerializeField]
    private float floorHeight, groundOffset, relativeOffset;

    [SerializeField]
    private int floor;

    private new Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }


    public int Floor {
        get => floor;
        set
        {
            floor = value;
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

}
