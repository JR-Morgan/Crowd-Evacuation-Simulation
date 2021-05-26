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

        private void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            displayPanel = uiDocument.rootVisualElement.Q<DisplayPanelElement>();

            LegacySimulationManager.Instance.OnSimulationStart.AddListener(() => SetDisplayPanelVisible(true));
            LegacySimulationManager.Instance.OnSimulationStop.AddListener(() => SetDisplayPanelVisible(false));
            SetDisplayPanelVisible(LegacySimulationManager.Instance.IsRunning);

            AddGameObjectActiveToggle(GameObject.FindGameObjectWithTag("Environment"), "Environment", HideRenderers);
            AddGameObjectActiveToggle(GameObject.FindGameObjectWithTag("Agent Parent"), "Agents", HideRenderers);
            AddGameObjectActiveToggle(GameObject.FindGameObjectWithTag("Visualisations"), "Heat-map", SetActive);
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
        #endregion

        #region Enabled/Disabled

        private void SetDisplayPanelVisible(bool visible)
        {
            Debug.Log(visible);
            if (displayPanel != null) displayPanel.SetEnabled(visible);
            if (!visible) Reset();
        }

        private void Reset()
        {
            foreach(Toggle t in displayPanel.ToggleElements)
            {
                if (!t.value)
                {
                    t.value = true;

                    using (var evt = new ClickEvent())
                    {
                        evt.target = t;
                        t.SendEvent(evt);
                    }
                }
            }
        }
        #endregion
    }
}
