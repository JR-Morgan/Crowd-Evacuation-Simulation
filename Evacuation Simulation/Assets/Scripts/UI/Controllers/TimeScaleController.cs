using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using JMTools;
using UnityEngine;
using UnityEngine.UIElements;

namespace PedestrianSimulation.UI
{
    [AddComponentMenu("Simulation/UI/Time Scale Controller"), DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public class TimeScaleController : MonoBehaviour
    {
        private const string PREFIX = "X"; //"\u2715"; //for some reason, I'm having some problems getting unicode to render with UI toolkits fonts
        
        private VisualElement element;
        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = uiDocument.rootVisualElement;

            element = rootVisualElement.Q("speed");

            element.AddRange(GenerateSpeedControls(1,2,4));
        }


        private static List<VisualElement> GenerateSpeedControls(params float[] speedParams) => GenerateSpeedControls(speeds: speedParams);
        private static List<VisualElement> GenerateSpeedControls(ICollection<float> speeds)
        {
            var elements = new List<VisualElement>();

            if (speeds.Count >= 0)
            {
                var label = new Label("Speed: ");
                elements.Add(label);
                
                foreach (float speed in speeds)
                {
                    var button = new Button(() =>
                    {
                        SetTimeScale(speed);
                    })
                    {
                        text = $"{PREFIX}{speed}",
                    };
                
                    elements.Add(button);
                }
            }

            return elements;
        }

        private static void SetTimeScale(float value) => Time.timeScale = value;
    }
}
