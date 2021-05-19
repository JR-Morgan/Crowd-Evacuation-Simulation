using PedestrianSimulation.UI.Elements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PedestrianSimulation.UI.Controllers
{
    [System.Obsolete]
    [RequireComponent(typeof(UIDocument))]
    public class TimelineController : MonoBehaviour
    {
        private void Start()
        {
            UIDocument document = GetComponent<UIDocument>();

            TimelineElement t = document.rootVisualElement.Q<TimelineElement>();
            if (t != null)
            {
                t.OnJump += (time) => WorldStateManager.Instance.JumpNearest(time);

                WorldStateManager.Instance.OnUpdate.AddListener(t.Update);
            }
            else
            {
                Debug.LogWarning($"{this} could not find a {typeof(TimelineElement)} in {document}");
            }

        }
    }
}