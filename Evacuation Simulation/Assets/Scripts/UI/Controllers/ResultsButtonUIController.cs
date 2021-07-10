using System.Diagnostics;
using PedestrianSimulation.Simulation;
using Results_Core;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

#nullable enable
namespace PedestrianSimulation.UI.Controllers
{
    
    [AddComponentMenu("Simulation/UI/Results Controller"), DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public class ResultsButtonUIController : MonoBehaviour
    {
        private UIDocument? document;
        private VisualElement? buttonParent;
        private Button? button;
        
        private void Start()
        {
            document = GetComponent<UIDocument>();
            buttonParent = document.rootVisualElement.Q("resultsParent");
            
            SimulationManager.Instance.OnSimulationStart.AddListener(DestroyButton);
            SimulationManager.Instance.OnSimulationFinished.AddListener(ShowButton);
            SimulationManager.Instance.OnSimulationTerminated.AddListener(DestroyButton);
        }

        private void ShowButton(SimulationResults results)
        {
            if (buttonParent == null) return;
            
            DestroyButton();
            
            button = new Button(() => ShowResults(results))
            {
                text = "Results"
            };
            
            buttonParent.Add(button);
        }

        private void DestroyButton()
        {
            button?.RemoveFromHierarchy();
        }

        private void ShowResults(SimulationResults results)
        {
            const string application = @"Results Viewer.exe";
            const string path =
#if UNITY_EDITOR
                @"Assets\Scripts\Results\Viewer\";
#else
                @"Assets\Scripts\Results\Viewer"; //TODO figure out how to move the exe into build dir
#endif
            
            Debug.Log("Showing results");

            Process resultsViewer = new Process
            {
                StartInfo =
                {
                    FileName = path + application
                },
            };
            resultsViewer.Start();


        }
    }
}
