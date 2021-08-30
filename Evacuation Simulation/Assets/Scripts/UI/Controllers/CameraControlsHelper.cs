using PedestrianSimulation.Cameras;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace PedestrianSimulation.UI
{
    public static class CameraControlsHelper 
    {
        public static void CreateControls(GameObject go, VisualElement uiParent)
        {
            foreach (Component c in go.GetComponentsInChildren<Component>())
            {
                VisualElement? element = GetViewControl(c);
                if (element != null)
                {
                    uiParent.Add(element);
                }
            }
        }


        private static VisualElement? GetViewControl(Component c)
        {
            switch (c)
            {
                case Camera2DContinuousLevelController cc:
                {
                    bool @lock = false;
                    var element = new Slider(1,0)
                    {
                        value = 1,
                        transform =
                        {
                            rotation = Quaternion.Euler(0f, 0f, 90f),
                        },
                        style =
                        {
                            marginLeft = 30f,
                            marginTop = 30f,
                            width = new Length(30, LengthUnit.Percent),
                        }
                    };


                    cc.OnValueChange.AddListener(controller =>
                    {
                        if(!@lock)
                            element.SetValueWithoutNotify(controller.YProportion);
                    });

                    cc.OnModelChange.AddListener(controller =>
                    {
                        if(!@lock)
                            element.value = 1;
                    });

                    cc.OnControllerEnabledChange.AddListener(controller =>
                    {
                        if(!@lock)
                            element.visible = controller.isActiveAndEnabled;
                    });


                    element.RegisterValueChangedCallback(evt =>
                    {
                        @lock = true;
                        cc.YProportion = element.value;
                        @lock = false;
                    });

                    element.visible = cc.isActiveAndEnabled;
                    return element;
            }
                    
                default:
                    return null;
            }
        }
            
    }
}
