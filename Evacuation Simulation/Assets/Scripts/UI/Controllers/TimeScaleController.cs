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
    [RequireComponent(typeof(UIDocument))]
    public class TimeScaleController : MonoBehaviour
    {
       
        private VisualElement element;
        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = uiDocument.rootVisualElement;

            element = rootVisualElement.Q("speed");

            element.AddRange(GenerateSpeedControls(1,2,4));
        }


        private static List<VisualElement> GenerateSpeedControls(params float[] speedParams) => GenerateSpeedControls(speeds: speedParams);
        private static List<VisualElement> GenerateSpeedControls(IEnumerable<float> speeds)
        {
            var elements = new List<VisualElement>();
            
            foreach (float speed in speeds)
            {
                var button = new Button(() =>
                {
                    SetTimeScale(speed);
                })
                {
                    text = $"{speed}",
                };
                
                elements.Add(button);
            }

            return elements;
        }

        private static void SetTimeScale(float value) => Time.timeScale = value;
    }
}
