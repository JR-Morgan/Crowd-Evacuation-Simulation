using UnityEngine.UIElements;
using Res = UnityEngine.Resources;

namespace PedestrianSimulation.UI.Elements
{
    public class ReceiverElement : VisualElement
    {
        private static readonly VisualTreeAsset view = Res.Load<VisualTreeAsset>(@"UI/Views/ReceiverView");

        public ReceiverElement()
        {
            Add(view.CloneTree());
        }

        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<ReceiverElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        #endregion
    }
}