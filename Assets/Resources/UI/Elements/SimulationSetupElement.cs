using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Res = UnityEngine.Resources;

namespace Assets.Resources.UI.Elements
{
    public class SimulationSetupElement : VisualElement
    {
        private static readonly VisualTreeAsset window = Res.Load<VisualTreeAsset>(@"UI/Views/SetupWindow");

        private readonly SimulationSettings viewModel;

        private readonly Button runSimulation;

        public SimulationSetupElement()
        {
            Add(window.CloneTree());
            viewModel = new SimulationSettings();
            runSimulation = this.Q<Button>("ButtonSubmit");
            runSimulation.SetEnabled(false);
        }

        public void SetStatus(bool isBusy) => runSimulation.SetEnabled(!isBusy);

        public void SetViewModel()
        {
            VisualElement parameterParent = this.Q<VisualElement>("ParameterParent");
            parameterParent.Clear();
            foreach (FieldInfo field in viewModel.GetType().GetFields(BindingFlags.Instance |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Public))
            {
                VisualElement v = CreateElement(field);
                if(v != null)
                {
                    parameterParent.Add(v);
                }
                else
                {
                    Debug.LogWarning($"{typeof(SimulationSetupElement)} does not support view model fields of type {{{field.FieldType}}}");
                }
            }

            Button b = this.Q<Button>("ButtonSubmit");

            
            runClickEvent = new EventCallback<ClickEvent>(e => ChangeButton(b, OnRun(viewModel)));
            cancelClickEvent = new EventCallback<ClickEvent>(e => ChangeButton(b, OnCancel()));
            ChangeButton(b, false);
        }

        private EventCallback<ClickEvent> runClickEvent;
        private EventCallback<ClickEvent> cancelClickEvent;

        private void ChangeButton(Button b, bool isRun)
        {
            if(isRun)
            {
                b.UnregisterCallback(runClickEvent);
                b.RegisterCallback(cancelClickEvent);
                b.text = "Cancel Simulation";
            }
            else
            {
                b.UnregisterCallback(cancelClickEvent);
                b.RegisterCallback(runClickEvent);
                b.text = "Run Simulation";
            }
            
        }

        public delegate bool RunEventHandler(SimulationSettings settings);
        public RunEventHandler OnRun;
        public Func<bool> OnCancel;

#pragma warning disable CS0642 // Possible mistaken empty statement
        private VisualElement CreateElement(FieldInfo field)
        {
            string label = field.Name;
            object viewModel = this.viewModel; //Box
            return (field.GetValue(viewModel)) switch
            {
                bool v => AddCallbackNative(new Toggle(label), v),
                int v => AddCallbackString(new TextField(label), v, int.TryParse),
                string v => AddCallbackNative(new TextField(label), v),
                float v => AddCallbackString(new TextField(label), v, float.TryParse),
                double v => AddCallbackString(new TextField(label), v, double.TryParse),
                _ => null,
            };

            E AddCallbackNative<E, T>(E element, T value) where E : BaseField<T> => AddCallback<E, T, T>(element, value, ToSelf, ToSelf);

            E AddCallbackString<E, T>(E element, T value, Convert<string, T> toV) where E : BaseField<string> => AddCallback<E, string, T>(element, value, toV, ToString);

            E AddCallback<E, S, T>(E element, T value, Convert<S, T> toV, Convert<T, S> toTarget) where E : BaseField<S>
            {

                element.RegisterCallback<ChangeEvent<S>>(e =>
                {

                    if (toV(e.newValue, out T value)) ;

                    else if (toV(e.previousValue, out value)) ;
                    else value = default;

                    field.SetValue(viewModel, value);
                    SetValue(element, value, toTarget);

                });

                SetValue(element, value, toTarget);

                return element;

                static void SetValue(E element, T value, Convert<T, S> toTarget)
                {
                    if (toTarget(value, out S newValue))
                    {
                        element.value = newValue;
                    }
                    else
                    {
                        element.value = default;
                    }
                }
            }


            static bool ToString<T>(T input, out string result)
            {
                result = input.ToString();
                return true;
            }

            static bool ToSelf<T>(T input, out T result)
            {
                result = input;
                return true;
            }

        }
#pragma warning restore CS0642 // Possible mistaken empty statement

        private delegate bool Convert<A,B>(A input, out B result);

        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<SimulationSetupElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
        #endregion
    }
}