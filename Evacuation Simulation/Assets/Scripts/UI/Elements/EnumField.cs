using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using Res = UnityEngine.Resources;

namespace PedestrianSimulation.UI.Elements
{
    public class EnumSelection : VisualElement, INotifyValueChanged<Enum>
    {
        private static readonly VisualTreeAsset View = Res.Load<VisualTreeAsset>(@"UI/Views/EnumSelectionView");

        private Enum _value;

        private Type enumType;

        private readonly Foldout foldout;
        private readonly VisualElement optionsParent;

        private string elementLabelText;

        
        public void SetValueWithoutNotify(Enum newValue)
        {
            _value = newValue;
            foldout.text = FormatElementLabelText(value);
        }

        public Enum value
        {
            get => _value;
            set
            {
                if (EqualityComparer<Enum>.Default.Equals(_value, value)) return;
                
                if (panel != null)
                {
                    using (ChangeEvent<Enum> evt = ChangeEvent<Enum>.GetPooled(_value, value))
                    {
                        evt.target = this;
                        SetValueWithoutNotify(value);
                        SendEvent(evt);
                    }
                }
                else
                {
                    SetValueWithoutNotify(value);
                }
            }
        }
        
        public void SetType(Type enumType)
        {
            value = (Enum)Activator.CreateInstance(enumType);
            optionsParent.Clear();

            this.enumType = enumType;

            foreach (Enum e in Enum.GetValues(enumType))
            {
                AddElement(e);
            }
        }

        private void AddElement(Enum value)
        {
            Button option = new Button()
            {
                name = $"Enum option - {value}",
                text = ViewModelElementFactory.FormatLabelString(value.ToString()),
            };
            option.RegisterCallback(SelectHandler(value));

            optionsParent.Add(option);
        }


        private EventCallback<ClickEvent> SelectHandler(Enum value)
        {
            return Handler;
            void Handler(ClickEvent evt)
            {
                this.value = value;
                SetOptionsVisibility(false);
            }
        }

        private void SetOptionsVisibility(bool isVisible)
        {
            foldout.value = isVisible;
        }

        private string FormatElementLabelText(object value = null)
        {
            if (value == null) return elementLabelText;

            string formattedValue = ViewModelElementFactory.FormatLabelString(value.ToString());
            return $"{elementLabelText} - {formattedValue}";
        }


        public EnumSelection(string label)
        {
            this.Add(View.CloneTree());
            
            
            
            this.style.flexDirection = FlexDirection.Column;

            foldout = this.Q<Foldout>("Foldout");
            elementLabelText = label;
            foldout.text = FormatElementLabelText();
            optionsParent = foldout.Q("unity-content");
        }

        public EnumSelection()
            : this("Enum Selection")
        {
        }

        public EnumSelection(string label, Type enumType, Enum defaultValue)
            : this(label)
        {
            SetType(enumType);
            value = defaultValue;
        }

        #region UXML Factory

        public new class UxmlFactory : UxmlFactory<EnumSelection, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }

        #endregion
    }
}