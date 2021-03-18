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
        [SerializeField]
        private List<string> keys = new List<string>();
        [SerializeField]
        private List<string> values = new List<string>();

        public Dictionary<string, object> Data { get; set; }

        //public void Update()
        //{
        //    keys = Data.Keys.ToList();
        //    values = new List<string>(Data.Keys.Count);
        //    foreach(object o in Data.Values)
        //    {
        //        string s = "";
        //        if (o != null) s = o.ToString();
        //        values.Add(s);
        //    }
            
        //}
    }
}