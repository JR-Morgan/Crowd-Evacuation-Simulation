using PedestrianSimulation.Simulation;
using PedestrianSimulation.UI.Elements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PedestrianSimulation.UI.Controllers
{
    [AddComponentMenu("Simulation/UI/Timeline Controller"), DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public class TimelineController : MonoBehaviour
    {
        private void Start()
        {
            UIDocument document = GetComponent<UIDocument>();

            TimelineElement t = document.rootVisualElement.Q<TimelineElement>();
            Debug.Assert(t != null, $"{typeof(TimelineController)} could not find a {typeof(TimelineElement)} in {document}", this);

            t.OnJump += (time) => t.Value = WorldStateManager.Instance.JumpNearest(time);

            WorldStateManager.Instance.OnUpdate.AddListener(t.Update);

            LegacySimulationManager.Instance.OnSimulationStop.AddListener(() => t.Reset());
            

        }
    }
}