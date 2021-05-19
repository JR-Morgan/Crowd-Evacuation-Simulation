using UnityEngine.UIElements;
using Res = UnityEngine.Resources;

namespace PedestrianSimulation.UI.Elements
{
    public class RecieverElement : VisualElement
    {
        private static readonly VisualTreeAsset view = Res.Load<VisualTreeAsset>(@"UI/Views/ReceiverView");

        public RecieverElement()
        {
            Add(view.CloneTree());
        }

        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<RecieverElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        #endregion
    }
}