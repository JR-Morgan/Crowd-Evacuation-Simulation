using Assets.Resources.UI.Elements;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

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
        element.OnCancel = SimulationManager.Instance.CancelSimulation;
        element.SetViewModel();
        ImportManager.Instance.OnBusyChange += element.SetStatus;

    }

    private bool RunSimulation(SimulationSettings settings)
    {
        if (settings.goal == null) settings.goal = GameObject.FindGameObjectWithTag("Goal").transform;
        return SimulationManager.Instance.RunSimulation(settings, environment);
    }

}
