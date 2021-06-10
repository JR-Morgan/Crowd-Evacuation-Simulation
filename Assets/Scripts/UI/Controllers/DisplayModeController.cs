using PedestrianSimulation.Simulation;
using PedestrianSimulation.UI.Elements;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PedestrianSimulation.UI.Controllers
{
    [AddComponentMenu("Simulation/UI/Display Mode Controller"), DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public class DisplayModeController : Singleton<DisplayModeController>
    {
        private DisplayPanelElement displayPanel;

        private List<Toggle> showOnlyWhileSimulating;

        private void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            displayPanel = uiDocument.rootVisualElement.Q<DisplayPanelElement>();

            showOnlyWhileSimulating = new List<Toggle>
            {
                AddGameObjectActiveToggle(GameObject.FindGameObjectWithTag("Environment"), "Environment", HideRenderers),
                AddGameObjectActiveToggle(GameObject.FindGameObjectWithTag("Agent Parent"), "Agents", HideRenderers),
            };

            AddGameObjectActiveToggle(GameObject.FindGameObjectWithTag("Visualisations"), "Heat-map", EnableChildren);


            SimulationManager.Instance.OnSimulationStart.AddListener(() => SetDisplayPanelVisible(true));
            SimulationManager.Instance.OnSimulationStop.AddListener(() => SetDisplayPanelVisible(false));
            SetDisplayPanelVisible(SimulationManager.Instance.IsRunning);
        }


        private Toggle AddGameObjectActiveToggle(GameObject go, string label, Action<bool, GameObject> action)
        {
            Toggle t = displayPanel.AddToggle(label, a => action.Invoke(a, go));
            t.value = (go != null && go.activeInHierarchy);
            return t;
        }

        #region Action<bool, GameObject> callbacks
        private void SetActive(bool b, GameObject go) => go.SetActive(b);

        private void HideRenderers(bool b, GameObject parent)
        { 
            foreach(Renderer r in parent.GetComponentsInChildren<Renderer>())
            {
                r.enabled = b;
            }
        }

        private void EnableChildren(bool b, GameObject parent)
        {
            foreach (Transform child in parent.transform)
            {
                child.gameObject.SetActive(b);
            }
        }
        #endregion

        #region Enabled/Disabled

        private void SetDisplayPanelVisible(bool visible)
        {
            if (displayPanel != null)
            {
                //displayPanel.SetEnabled(visible);
                foreach(Toggle t in showOnlyWhileSimulating)
                {
                    t.SetEnabled(visible);
                }
            }


            if (!visible) Reset();
        }




        private void Reset()
        {
            foreach(Toggle t in displayPanel.ToggleElements)
            {
                if (!t.value)
                {
                    t.value = true;

                    using var evt = new ClickEvent();
                    evt.target = t;
                    t.SendEvent(evt);
                }
            }
        }
        #endregion
    }
}
