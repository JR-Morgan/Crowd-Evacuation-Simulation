using UnityEditor;
using UnityEngine.UIElements;

namespace Assets.UI.Elements
{
    public class RecieverElement : VisualElement
    {
        private static readonly VisualTreeAsset view = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/UI/Views/RecieverView.uxml");

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