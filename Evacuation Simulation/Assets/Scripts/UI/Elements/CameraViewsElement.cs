using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Res = UnityEngine.Resources;

namespace PedestrianSimulation.UI.Elements
{
    public struct CameraViewViewModel
    {
        public string name;
        public GameObject gameObject;
    }

    public class CameraViewsElement : VisualElement
    {
        private static readonly VisualTreeAsset cameraView = Res.Load<VisualTreeAsset>(@"UI/Views/CameraView");

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
                CameraChangeEvent.Invoke(viewModel.gameObject);
            });

            return e;
        }


        #region Events
        public delegate void CameraChangeEventHandler(GameObject cameraGroup);
        public event CameraChangeEventHandler CameraChangeEvent;
        #endregion

        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<CameraViewsElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
        #endregion


    }
}