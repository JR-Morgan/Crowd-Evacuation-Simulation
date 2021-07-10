using PedestrianSimulation.Import.Speckle;
using PedestrianSimulation.Simulation;
using PedestrianSimulation.UI.Elements;
using System;
using PedestrianSimulation.Environment;
using UnityEngine;
using UnityEngine.UIElements;

namespace PedestrianSimulation.UI.Controllers
{
    [AddComponentMenu("Simulation/UI/Setup Controller"), DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public class SetupController : MonoBehaviour
    {
        private EnvironmentManager environment;
        

        private void Start()
        {
            { //Environment Setup
                GameObject environmentGO = GameObject.FindGameObjectWithTag("Environment");
                environment = environmentGO.GetComponent<EnvironmentManager>();
                //environment ??= environmentGO.AddComponent<EnvironmentManager>();
                Debug.Assert(environment != null, $"{this} could not find a {typeof(EnvironmentManager)} in {environmentGO}");
            }
            
            { // UI Setup
                UIDocument document = GetComponent<UIDocument>();

                SimulationSetupElement element = document.rootVisualElement.Q<SimulationSetupElement>();
                Debug.Assert(element != null, $"{this} could not find a {typeof(SimulationSetupElement)} in {document}");

                element.OnRun = RunSimulation;

                element.OnCancel = SimulationManager.Instance.CancelSimulation;

                element.SetViewModel(SimulationManager.Instance.Settings);
                ImportManager.Instance.OnBusyChange += element.SetStatus;
            }
        }

        private bool RunSimulation()
        {
            return environment != null && SimulationManager.Instance.RunSimulation(environment);
        }
    }
}
