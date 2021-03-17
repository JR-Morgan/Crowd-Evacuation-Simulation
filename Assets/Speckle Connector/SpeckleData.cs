using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
    /// <summary>
    /// This class gets attached to GOs and is used to store Speckle's metadata when sending / receiving
    /// </summary>
    public class SpeckleData : MonoBehaviour
    {
        //[SerializeField]
        //private List<string> keys = new List<string>();
        //[SerializeField]
        //private List<object> values = new List<object>();

        public Dictionary<string, object> Data { get; set; }

        //public void Update()
        //{
        //    keys = Data.Keys.ToList();
        //    values = Data.Values.ToList();
        //}
    }
}