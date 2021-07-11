using System;
using System.Diagnostics;
using System.IO;
using PedestrianSimulation.Simulation;
using Results_Core;
using Speckle.Newtonsoft.Json;
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
            SimulationManager.Instance.OnSimulationFinished.AddListener(ShowResults); //TODO change to show button
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


        private static void ShowResults(SimulationResults results) => ShowResults(JsonConvert.SerializeObject(results));
        
        private static void ShowResults(string json)
        {
            
            Debug.Log("Showing results");
            
            string jsonPath = WriteJSONToFile(json);
            StartViewer(jsonPath);

        }

        [ContextMenu("Test")]
        private void test()
        {
            StartViewer(@"C:\Users\Jedd\AppData\LocalLow\Jedd Morgan\Crowd Evacuation Simulation\results 132705128910396584.json");
        }

        private static void StartViewer(string arg)
        {
            const string application = @"Results Viewer.exe";
            string path =
#if UNITY_EDITOR
                Application.dataPath + @"/Scripts/Results/Viewer/";
#else
                Application.dataPath + @"/Scripts/Results/Viewer/"; //TODO figure out how to move the exe into build dir
#endif
            
            Process resultsViewer = new Process
            {
                StartInfo =
                {
                    FileName = path + application,
                    Arguments = $"\"{arg}\"",
                },
            };
            
            
            Debug.Log(path + application + " \"" + arg + '\"');
            resultsViewer.Start();
        }
        

        private static string WriteJSONToFile(string json)
        {
            string path = Application.persistentDataPath + @$"/results {DateTime.Now.ToFileTimeUtc()}.json";
            
            if (!File.Exists(path)) File.Create(path).Dispose();
            File.WriteAllText(path, json);
            return path;
        }
    }
}
