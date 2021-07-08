using System;
using System.Collections;
using System.Collections.Generic;
using PedestrianSimulation.Results;
using PedestrianSimulation.Simulation;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace PedestrianSimulation.UI.Controllers
{
    
    [AddComponentMenu("Simulation/UI/Results Controller"), DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public class ResultsButtonUIController : MonoBehaviour
    {
        private UIDocument document;
        private Button? button;
        
        private void Start()
        {
            document = GetComponent<UIDocument>();
            
            SimulationManager.Instance.OnSimulationStart.AddListener(DestroyButton);
            SimulationManager.Instance.OnSimulationFinished.AddListener(ShowButton);
            SimulationManager.Instance.OnSimulationTerminated.AddListener(DestroyButton);
        }

        private void ShowButton(SimulationResults results)
        {
            button = new Button(() => ShowResults(results))
            {
                text = "Results"
            };

            document.rootVisualElement.Add(button);
        }

        private void DestroyButton()
        {
            button?.RemoveFromHierarchy();
        }

        private void ShowResults(SimulationResults results)
        {
            Debug.Log("Showing results"); //TODO temp
        }
    }
}
