using UnityEngine.UIElements;
using Res = UnityEngine.Resources;

namespace PedestrianSimulation.UI.Elements
{
    public class AvailableStreamElement : VisualElement
    {
        private static readonly VisualTreeAsset view = Res.Load<VisualTreeAsset>(@"UI/Views/AvailableStreamView");//AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/Resources/UI/Views/AvailableStreamView.uxml");

        public AvailableStreamElement()
        {
            Add(view.CloneTree());
        }

        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<AvailableStreamElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        #endregion
    }
}