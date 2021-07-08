using System;
using System.Collections.Generic;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
    
    /// <summary>
    /// This class gets attached to GOs and is used to store Speckle's metadata when sending / receiving
    /// </summary>
    public class SpeckleProperties : MonoBehaviour, ISerializationCallbackReceiver
    {
        public Dictionary<string, object> Data { get; set; }

        [SerializeField] private List<string> _keys = new List<string>();
        [SerializeField] private List<string> _values = new List<string>();
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var kvp in Data)
            {
                string value = kvp.Value != null ? $"{kvp.Value.GetType()}({kvp.Value})" : "NULL";
                _keys.Add(kvp.Key);
                _values.Add(value);
            }
        }

        public void OnAfterDeserialize()
        {
            Data = new Dictionary<string, object>();

            for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
                Data.Add(_keys[i], _values[i]);
        }

        //void OnGUI()
        //{
        //    foreach (var kvp in Data)
        //        GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);
        //}
        
    }
    
}