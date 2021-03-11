using Assets.UI.Elements;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SetupController : MonoBehaviour
{
    private SimulationManager manager;
    private SimulationSetupElement element;

    private GameObject environment;

    private void Awake()
    {
        manager = SimulationManager.Instance;
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


        element.OnRun += RunSimulation;
        element.SetViewModel();
        

    }

    private void RunSimulation(SimulationSettings settings)
    {
        if (settings.goal == null) settings.goal = GameObject.FindGameObjectWithTag("Goal").transform;
        manager.RunSimulation(settings, environment);
    }

}
