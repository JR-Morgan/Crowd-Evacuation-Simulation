using PedestrianSimulation.Simulation;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Res = UnityEngine.Resources;

namespace PedestrianSimulation.UI.Elements
{
    public class SimulationSetupElement : VisualElement
    {
        private static readonly VisualTreeAsset window = Res.Load<VisualTreeAsset>(@"UI/Views/SetupWindow");

        private readonly Button runSimulation;

        public SimulationSetupElement()
        {
            Add(window.CloneTree());
            runSimulation = this.Q<Button>("ButtonSubmit");
            runSimulation.SetEnabled(false);
        }

        public void SetStatus(bool isBusy) => runSimulation.SetEnabled(!isBusy);


        public void SetViewModel(object viewModel)
        {
            VisualElement parameterParent = this.Q<VisualElement>("ParameterParent");
            parameterParent.Clear();
            foreach (FieldInfo field in viewModel.GetType().GetFields(BindingFlags.Instance |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Public)) //TODO custom attribute
            {
                VisualElement v = ViewModelElementFactory.CreateElement(field, viewModel);
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

            
            runClickEvent = new EventCallback<ClickEvent>(e => ChangeButton(b, OnRun()));
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

        public Func<bool> OnRun;
        public Func<bool> OnCancel;



        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<SimulationSetupElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
        #endregion
    }
}