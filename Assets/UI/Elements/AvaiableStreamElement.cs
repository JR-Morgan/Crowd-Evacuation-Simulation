using UnityEditor;
using UnityEngine.UIElements;

namespace Assets.UI.Elements
{
    public class AvailableStreamElement : VisualElement
    {
        private static readonly VisualTreeAsset view = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/UI/Views/AvailableStreamView.uxml");

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