using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.Events;

public class TimelineController : VisualElement
{

    private Slider slider;
    private EventCallback<ChangeEvent<float>> callback;
    private void Setup()
    {
        slider = this.Q<Slider>("TimelineSlider");
        callback = evt => WorldStateManager.Instance.JumpNearest(evt.newValue);
        slider.RegisterCallback(callback); //evt contains more data (such as previous position) that might be needed later
    }

    public void Initialise()
    {
        WorldStateManager.Instance.OnUpdate.AddListener(Update);
    }

    public void Update(float time)
    {
        if(slider == null) Setup();
        if(slider != null)
        {
            slider.UnregisterCallback(callback); //There is probably a better way to do this
            slider.highValue = Mathf.Max(slider.highValue, time);
            slider.value = time;
            slider.RegisterCallback(callback);
        }
    }



    #region UXML Factory
    public new class UxmlFactory : UxmlFactory<TimelineController, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    { }
    public TimelineController()
    {
        void GeometryChange(GeometryChangedEvent evt)
        {
            this.UnregisterCallback<GeometryChangedEvent>(GeometryChange);
            Setup();
        }

        this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
    }
    #endregion
}
