using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.UI.Elements
{
    public class SimulationSetupElement : VisualElement
    {
        private static readonly VisualTreeAsset window = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/UI/Views/SetupWindow.uxml");

        private SimulationSettings viewModel;


        public SimulationSetupElement()
        {
            Add(window.CloneTree());
            viewModel = new SimulationSettings();
        }


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

            this.Q<Button>("ButtonSubmit").RegisterCallback<ClickEvent>(e => OnRun.Invoke(viewModel));
        }

        public delegate void RunEventHandler(SimulationSettings settings);
        public event RunEventHandler OnRun;


        private VisualElement CreateElement(FieldInfo field)
        {
            string label = field.Name;
            object viewModel = this.viewModel; //Box
            return (field.GetValue(viewModel)) switch
            {
                bool v => AddCallback(new Toggle(label), v),
                int v => AddCallback(new IntegerField(label), v),
                string v => AddCallback(new TextField(label),v),
                float v => AddCallback(new FloatField(label), v),
                double v => AddCallback(new DoubleField(label), v),
                Vector3 v => AddCallback(new Vector3Field(label), v),
                Vector3Int v => AddCallback(new Vector3IntField(label), v),
                Vector2 v => AddCallback(new Vector2Field(label), v),
                Vector2Int v => AddCallback(new Vector2IntField(label), v),
                _ => null,
            };

            E AddCallback<E,T>(E element, T value) where E : VisualElement, INotifyValueChanged<T>
            {
                element.RegisterCallback<ChangeEvent<T>>(
                e =>
                {
                    field.SetValue(viewModel, e.newValue);
                });
                element.value = value;
                return element;
            }
            
        }


        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<SimulationSetupElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
        #endregion
    }
}