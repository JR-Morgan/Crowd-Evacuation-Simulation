using PedestrianSimulation.Import.Speckle;
using PedestrianSimulation.Simulation;
using PedestrianSimulation.UI.Elements;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PedestrianSimulation.UI.Controllers
{
    [AddComponentMenu("Simulation/UI/Setup Controller"), DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public class SetupController : MonoBehaviour
    {
        private SimulationSetupElement element;

        private GameObject environment;

        private void Awake()
        {
            environment = GameObject.FindGameObjectWithTag("Environment");
        }

        private void Start()
        {
            UIDocument document = GetComponent<UIDocument>();

            element = document.rootVisualElement.Q<SimulationSetupElement>();
            if (element == null)
            {
                Debug.LogWarning($"{this} could not find a {typeof(SimulationSetupElement)} in {document}");
                return;
            }


            element.OnRun = RunSimulation;

            element.OnCancel = SimulationManager.CancelSimulation;

            element.SetViewModel();
            ImportManager.Instance.OnBusyChange += element.SetStatus;

        }

        private bool RunSimulation(SimulationSettings settings)
        {
            if (settings.goal == null) settings.goal = GameObject.FindGameObjectWithTag("Goal").transform;
            return SimulationManager.RunSimulation(settings, environment);
        }

        private static ISimulationManager SimulationManager
        {
            get
            {
                try
                {
                    return Simulation.SimulationManager.Instance;
                }
                catch (UninitialisedSingletonException<SimulationManager> e)
                {
                    if (LegacySimulationManager.IsSingletonInitialised) return LegacySimulationManager.Instance;
                    else throw e;
                }
            }
        }
    }

}
