using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.UI.Elements
{
    public struct CameraViewViewModel
    {
        public string name;
        public Camera camera;
    }

    public class CameraViewsElement : VisualElement
    {
        private static readonly VisualTreeAsset cameraView = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/UI/Views/CameraView.uxml");

        private readonly List<CameraViewViewModel> viewModels = new List<CameraViewViewModel>();

        public void AddElement(CameraViewViewModel viewModel)
        {
            viewModels.Add(viewModel);
            this.Add(CreateView(viewModel));
        }

        public void AddRange(IEnumerable<CameraViewViewModel> viewModels)
        {
            foreach (CameraViewViewModel viewModel in viewModels) AddElement(viewModel);
        }

        private VisualElement CreateView(CameraViewViewModel viewModel)
        {
            VisualElement e = cameraView.CloneTree();
            e.Q<Button>("ViewName").text = viewModel.name;

            e.RegisterCallback<ClickEvent>(evt =>
            {
                CameraChangeEvent.Invoke(viewModel.camera);
            });

            return e;
        }


        #region Events
        public delegate void CameraChangeEventHandler(Camera cam);
        public event CameraChangeEventHandler CameraChangeEvent;
        #endregion

        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<CameraViewsElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
        #endregion


    }
}