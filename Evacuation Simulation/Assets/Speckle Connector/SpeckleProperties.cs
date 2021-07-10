using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
    
    /// <summary>
    /// This class gets attached to GOs and is used to store Speckle's metadata when sending / receiving
    /// </summary>
    public class SpeckleProperties : MonoBehaviour//, ISerializationCallbackReceiver
    {
        public Dictionary<string, object> Data { get; set; }

        [SerializeField, HideInInspector]
        private List<string>
            _keys = new List<string>(),
            _types = new List<string>(),
            _values = new List<string>();
        
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _types.Clear();
            _values.Clear();

            foreach (var kvp in Data)
            {
                string type = null, value = null;
                
                if (kvp.Value != null)
                {
                    Type t = kvp.Value.GetType();
                    type = t.AssemblyQualifiedName;
                    
                    value = JsonUtility.ToJson(Wrap(kvp.Value));
                }
                
                _keys.Add(kvp.Key);
                _types.Add(type);
                _values.Add(value);
            }
            
            object Wrap(object value) => typeof(SpeckleProperties).GetMethod(nameof(CreateWrapper))!.MakeGenericMethod(value.GetType()).Invoke(null, new []{value});
        }
        
        private static Wrapper<T> CreateWrapper<T>(T value) => new Wrapper<T>(value);

        [Serializable]
        private class Wrapper<T>
        {
            public T value;
            public Wrapper(T value) => this.value = value;
            
        }

        public void OnAfterDeserialize()
        {
            Data = new Dictionary<string, object>();

            for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
            {
                object deserializedObject = _values[i] != null
                    ? JsonUtility.FromJson(_values[i], Type.GetType(_types[i]))
                    : null;
                
                Data.Add(_keys[i], deserializedObject);
            }
        }
        
        
    }
    
}