using System;
using System.Collections.Generic;
using Objects.BuiltElements.Revit;
using Speckle.Core.Api;
using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
    /// <summary>
    /// This class gets attached to GOs and is used to store Speckle's metadata when sending / receiving
    /// </summary>
    public class SpeckleProperties : MonoBehaviour, ISerializationCallbackReceiver
    {
        public Dictionary<string, object> Data { get; set; }

        [SerializeField]
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
                string assemblyQualifiedName = null, value = null;
                
                if (kvp.Value != null)
                {
                    assemblyQualifiedName = kvp.Value.GetType().AssemblyQualifiedName;
                    
                    value = SerializeValue(kvp.Value);
                }
                
                _keys.Add(kvp.Key);
                _types.Add(assemblyQualifiedName);
                _values.Add(value);
            }
            
        }
        
        public void OnAfterDeserialize()
        {
            Data = new Dictionary<string, object>();

            for (int i = 0; i != Mathf.Min(_keys.Count, _values.Count, _types.Count); i++)
            {
                Type type = Type.GetType(_types[i]);

                object deserializedObject = DeserializeValue(_values[i], type);

                Data.Add(_keys[i], deserializedObject);
            }
        }

        /// <summary>
        /// Serialize the <paramref name="value"/> to a JSON <see cref="string"/><br/>
        /// Will use <see cref="Operations.Serialize(Base)"/> if <paramref name="value"/> inherits from <see cref="Base"/><br/>
        /// Otherwise will use <see cref="JsonUtility"/> around a <see cref="Wrapper{T}"/>
        /// </summary>
        /// <param name="value">The <see cref="object"/> to serialise</param>
        /// <returns>JSON <see cref="string"/> of <paramref name="value"/>. returns <see langword="null"/> when <paramref name="value"/> is <see langword="null"/>.</returns>
        private static string SerializeValue(object value)
        {
            return value switch
            {
                null => null,
                Base @base => Operations.Serialize(@base),
                _ => JsonUtility.ToJson(Wrap(value))
            };
            
            
            static object Wrap(object value) //Creates an instance of Wrapper with generic type of value.GetType()
            {
                Type wrapperType = typeof(Wrapper<>).MakeGenericType(value.GetType());
                return Activator.CreateInstance(wrapperType, value);
            } 
        }
        
        private static object DeserializeValue(string json, Type type)
        {
            if (string.IsNullOrEmpty(json)) return null;
            
            if (type.IsSubclassOf(typeof(Base)))
            {
                return Operations.Deserialize(json);
            }
            
            //Object is not a Base, use JsonUtility to deserialize as a wrapper
            Type wrapperType = typeof(Wrapper<>).MakeGenericType(type);
            IWrapper w = (IWrapper)JsonUtility.FromJson(json, wrapperType);
            return w.Value;
        }

        [Serializable]
        private class Wrapper<T> : IWrapper
        {
            public T value;
            public object Value => value;
            public Wrapper(T value) => this.value = value;
        }
        
        private interface IWrapper
        {
            public object Value { get; }
        }
        
    }
}