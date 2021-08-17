using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using PedestrianSimulation.Simulation;
using PedestrianSimulation.UI.Elements;
using UnityEngine.UIElements;

namespace PedestrianSimulation.UI
{
    public static class ViewModelElementFactory
    {
        public static string FormatLabelString(string label) //TODO replace with humaniser
        {
            TextInfo t = CultureInfo.CurrentCulture.TextInfo;
            return t.ToTitleCase(Regex.Replace(label, "([A-Z])", " $1").Trim());
        }

        public static VisualElement CreateElement(FieldInfo field, object viewModel)
        {
            string label = FormatLabelString(field.Name);
            object fieldValue = field.GetValue(viewModel);

            return (fieldValue) switch
            {
                bool v => AddCallbackNative(ValueChangeHandler, new Toggle(label), v),
                int v => AddCallbackConvert(ValueChangeHandler, new TextField(label), v, int.TryParse),
                string v => AddCallbackNative(ValueChangeHandler, new TextField(label), v),
                float v => AddCallbackConvert(ValueChangeHandler, new TextField(label), v, float.TryParse),
                double v => AddCallbackConvert(ValueChangeHandler, new TextField(label), v, double.TryParse),
                LocalAvoidanceStrategy v => EnumSelection(v, typeof(LocalAvoidanceStrategy)),
                _ => null,
            };

            
            void ValueChangeHandler<T>(T newValue)
            {
                field.SetValue(viewModel, newValue);
            }

            EnumSelection EnumSelection<T>(T v, Type t) where T : Enum
                => AddCallbackEnum(ValueChangeHandler, new EnumSelection(label, t, default(T)), v);
        }
        
        #region Add Callback Methods
        private static E AddCallbackNative<E, T>(Action<T> onValueChange, E element, T value)
            where E : VisualElement, INotifyValueChanged<T>
            => AddCallback<E, T, T>(onValueChange, element, value, ToSelf, ToSelf);

        private static E AddCallbackConvert<E, T>(Action<T> onValueChange, E element, T value, Convert<string, T> toV)
            where E : VisualElement, INotifyValueChanged<string>
            => AddCallback<E, string, T>(onValueChange, element, value, toV, ToString);


        private static E AddCallbackEnum<E, T>(Action<T> onValueChange, E element, T value)
            where E : VisualElement, INotifyValueChanged<Enum>
            => AddCallback<E, Enum, T>(onValueChange, element, value, ToSelfExplicit, ToSelfExplicit);
       
        private static E AddCallback<E, S, T>(Action<T> onValueChange, E element, T value, Convert<S, T> toV, Convert<T, S> toTarget)
            where E : VisualElement, INotifyValueChanged<S>
        {

            element.RegisterCallback<ChangeEvent<S>>(e =>
            {
                T newValue = ParseNewValue(e.newValue, e.previousValue, toV);
                
                onValueChange.Invoke(newValue);

                SetValue(element, newValue, toTarget);

            });

            SetValue(element, value, toTarget);

            return element;
        }

        //private static E AddCallbackEnum<E, T>(Action<T> onValueChange, E element, T value) where E : VisualElement where T : Enum
        //{
        //
        //    element.RegisterCallback<ChangeEvent<string>>(e =>
        //    {
        //        T newValue = Enum.TryParse(e.newValue, out T result);
        //
        //        onValueChange.Invoke(newValue);
        //
        //        SetValue(element, newValue, toTarget);
        //
        //    });
        //
        //    SetValue(element, value, ToString);
        //
        //    return element;
        //}

        private static T ParseNewValue<S, T>(S newValue, S previousValue, Convert<S, T> toV)
        {
#pragma warning disable CS0642 // Possible mistaken empty statement
            if (toV(newValue, out T newTValue)) ;

            else if (toV(previousValue, out newTValue)) ;
            else newTValue = default;
#pragma warning restore CS0642
            return newTValue;
        }
        #endregion

        private static bool ToString<T>(T input, out string result)
        {
            result = input.ToString();
            return true;
        }
        
        
        private static bool ToSelf<A>(A input, out A result)
        {
            result = input;
            return true;
        }
        
        private static bool ToSelfExplicit<A,B>(A input, out B result)
        {
            result = (B)(object)input;
            return true;
        }

        private static void SetValue<E, S, T>(E element, T value, Convert<T, S> toTarget) where E : INotifyValueChanged<S>
        {
            element.value = toTarget(value, out S newValue) ? newValue : default;
        }


        private delegate bool Convert<A, B>(A input, out B result);
    }
}
