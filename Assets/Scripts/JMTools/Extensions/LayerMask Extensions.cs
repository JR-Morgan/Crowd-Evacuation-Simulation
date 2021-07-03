using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMTools
{
    public static class LayerMaskExtensions
    {
        public static bool ContainsLayer(this LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }
        
        
    }
}
