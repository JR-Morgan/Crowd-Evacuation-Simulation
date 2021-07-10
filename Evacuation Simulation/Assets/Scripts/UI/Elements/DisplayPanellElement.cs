using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Res = UnityEngine.Resources;

namespace PedestrianSimulation.UI.Elements
{
    /// <summary>
    /// <see cref="VisualElement"/> for an arbetery number of <see cref="Toggle"/> elements.<br/>
    /// Provides helper functions for adding <see cref="Toggle"/>s and registering <see cref="ClickEvent"/> callbacks
    /// </summary>
    public class DisplayPanelElement : VisualElement
    {
        private static readonly VisualTreeAsset VIEW = Res.Load<VisualTreeAsset>(@"UI/Views/DisplayPanelView");


        public VisualElement Container { get; private set; }
        public List<Toggle> ToggleElements { get; private set; }

        public Toggle AddToggle(string label, Action<bool> callback = null)
        {
            return AddToggle(label, e => callback?.Invoke(((Toggle)e.target).value));
        }

        /// <summary>
        /// <see cref="DisplayPanelElement"/> will create a new <see cref="Toggle"/> element and the specifed <paramref name="callback"/><br/>
        /// </summary>
        /// <param name="label">Desired <see cref="Label"/>'s value"/></param>
        /// <param name="callback">Callback for the <see cref="Toggle"/>'s <see cref="ClickEvent"/></param>
        /// <returns>The newly created toggle</returns>
        public Toggle AddToggle(string label, EventCallback<ClickEvent> callback)
        {
            Toggle t = new Toggle(label);
            t.RegisterCallback(callback);
            t.RegisterCallback<DetachFromPanelEvent>(a => ElementDisposeHandler(t));

            Container.Add(t);

            ToggleElements.Add(t);
            CheckHide();
            return t;

            void ElementDisposeHandler(Toggle t)
            {
                ToggleElements.Remove(t);
                t.UnregisterCallback(callback);
                CheckHide();
            }
        }

        private bool CheckHide()
        {
            bool v = ToggleElements.Count > 0;
            Container.visible = v;
            return v;
        }


        public DisplayPanelElement()
        {
            this.Add(VIEW.CloneTree());
            ToggleElements = new List<Toggle>();
            Container = this.Q("Container");
            CheckHide();
        }


        #region UXML Factory
        public new class UxmlFactory : UxmlFactory<DisplayPanelElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        #endregion
    }
}
