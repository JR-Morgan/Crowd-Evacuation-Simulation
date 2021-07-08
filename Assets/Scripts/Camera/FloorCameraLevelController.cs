using System;
using Objects.BuiltElements.Revit;
using Speckle.ConnectorUnity;
using UnityEngine;

namespace PedestrianSimulation.World
{
    public class FloorCameraLevelController : MonoBehaviour
    {
        private const string EnvironmentTag = "Environment";
        
        [SerializeField]
        private float floorHeight, groundOffset, relativeOffset;

        [SerializeField]
        private int _floor;

        public int Floor
        {
            get => _floor;
            set
            {
                _floor = value;
                SetFloorView(Floor);
            }
        }

        #region Unity Methods

        private void OnValidate()
        {
            SetFloorView(Floor);
        }

        private void OnDisable()
        {
            GameObject environment = GameObject.FindGameObjectWithTag("Environment");

            foreach (SpeckleProperties d in environment.GetComponentsInChildren<SpeckleProperties>(true))
            {
                TrySetActive(d, true);
            }
        }

        private void OnEnable()
        {
            SetFloorView(Floor);
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
        #endregion


        #region Set Floor View
        private static void SetFloorView(int floor)
        {
            GameObject environment = GameObject.FindGameObjectWithTag(EnvironmentTag);
            Debug.Assert(environment != null, $"{typeof(FloorCameraLevelController)} could not find GameObject with tag \"{EnvironmentTag}\"");
            foreach (SpeckleProperties d in environment.GetComponentsInChildren<SpeckleProperties>(true))
            {
                TrySetActive(d, floor);
            }
        }


        private static bool TryGetLevel(SpeckleProperties d, out int startLevel, out int topLevel)
        {
            if (d != null // If level is defined
                && d.Data != null
                && d.Data.TryGetValue("level", out object oStartLevel)
                && oStartLevel is RevitLevel level
                && int.TryParse(level.name, out startLevel))
            {

                if (!( //If top level is not defined
                    d.Data.TryGetValue("topLevel", out object oTopLevel)
                    && oTopLevel != null
                    && int.TryParse(((RevitLevel)oTopLevel).name, out topLevel)
                    ))
                {
                    topLevel = startLevel;
                }


                return true;
            }

            startLevel = default;
            topLevel = default;
            return false;
        }

        private static void TrySetActive(SpeckleProperties d, int floor)
        {
            if (TryGetLevel(d, out int startLevel, out int topLevel))
            {
                d.gameObject.SetActive(startLevel <= floor && floor <= topLevel);
            }
        }

        private static void TrySetActive(SpeckleProperties d, bool active)
        {
            if (TryGetLevel(d, out int _, out int _))
            {
                d.gameObject.SetActive(active);
            }
        }
        #endregion

    }
}