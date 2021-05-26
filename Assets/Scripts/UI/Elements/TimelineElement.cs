using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Res = UnityEngine.Resources;

namespace PedestrianSimulation.UI.Elements
{
    public class TimelineElement : VisualElement
    {
        private static readonly VisualTreeAsset timelineSlider = Res.Load<VisualTreeAsset>(@"UI/Views/TimelineView");

        #region Fields
        private Slider slider;
        private EventCallback<ChangeEvent<float>> callback;
        #endregion

        public float Value { get => slider.value; set { slider.value = value; } }


        public TimelineElement()
        {
            Add(timelineSlider.CloneTree());

            void GeometryChange(GeometryChangedEvent evt)
            {
                this.UnregisterCallback<GeometryChangedEvent>(GeometryChange);
                Setup();
            }

            this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
        }

        private void Setup()
        {
            slider = this.Q<Slider>("TimelineSlider");
            callback = evt => OnJump?.Invoke(evt.newValue); //evt contains more data (such as previous position) that might be needed later
            slider.RegisterCallback(callback);
        }


        public void Update(float time)
        {
            if(slider != null)
            {
                slider.UnregisterCallback(callback); //There is probably a better way to do this
                slider.highValue = Mathf.Max(slider.highValue, time);
                slider.value = time;
                slider.RegisterCallback(callback);
            }
        }


        #region Events
        public delegate void JumpEventHandler(float time);
        public event JumpEventHandler OnJump;
        #endregion

        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<TimelineElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }
        #endregion
    }
}
