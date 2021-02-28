using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public struct CameraViewViewModel
{
    public string name;
    public Camera camera;
}

public delegate void CameraChangeEventHandler(Camera cam);

public class CameraViewController : VisualElement
{
    private VisualTreeAsset viewElement;


    private readonly List<CameraViewViewModel> viewModels = new List<CameraViewViewModel>();

    public event CameraChangeEventHandler CameraChangeEvent;

    public void AddElement(CameraViewViewModel viewModel)
    {
        viewModels.Add(viewModel);
        this.Add(CreateView(viewModel));
    }

    public void AddRange(IEnumerable<CameraViewViewModel> viewModels)
    {
        foreach(CameraViewViewModel viewModel in viewModels)
        {
            AddElement(viewModel);
        }
    }

    private VisualElement CreateView(CameraViewViewModel viewModel)
    {
        VisualElement e = viewElement.CloneTree();
        e.Q<Label>("ViewName").text = viewModel.name;

        e.RegisterCallback<ClickEvent>(evt =>
        {
            CameraChangeEvent.Invoke(viewModel.camera);
        });

        return e;
    }




    #region UXML Factory
    public new class UxmlFactory : UxmlFactory<CameraViewController, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    { }


    public CameraViewController()
    {
        viewElement = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/UI/Views/Elements/CameraViewElement.uxml");
    }
    #endregion
}
