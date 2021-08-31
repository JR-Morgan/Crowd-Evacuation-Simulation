using System.Collections.Generic;
using UnityEngine.UIElements;

namespace JMTools
{
    #nullable enable
    public static class UIToolkitExtensions
    {
        public static void AddRange(this VisualElement parent, IEnumerable<VisualElement?> children)
        {
            foreach (var child in children)
            {
                parent.Add(child);
            }
        }

        public static void AddRange(this VisualElement parent, params VisualElement?[] childrenParams) => AddRange(parent, children: childrenParams);
    }
}
