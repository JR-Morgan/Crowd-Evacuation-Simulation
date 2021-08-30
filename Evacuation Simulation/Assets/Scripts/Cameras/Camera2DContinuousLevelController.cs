using System;
using System.Collections;
using System.Collections.Generic;
using JMTools;
using PedestrianSimulation.Import.Speckle;
using PedestrianSimulation.Simulation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace PedestrianSimulation.Cameras
{
    [AddComponentMenu("Simulation/Camera/2D/Continuous Camera Controller")]
    [RequireComponent(typeof(Camera)), DisallowMultipleComponent]
    public class Camera2DContinuousLevelController : MonoBehaviour
    {
        
        private Camera cam;


        [SerializeField]
        private float _modelHeight;
        public float ModelHeight
        {
            get => _modelHeight;
            set
            {
                _modelHeight = value;
                ValueChangeHandler();
            }
        }
        

        
        [SerializeField, Range(0f, 1f)]
        private float _yProportion;
        public float YProportion
        {
            get => _yProportion;
            set
            {
                _yProportion = value;
                ValueChangeHandler();
            }
        }
        

        
        [SerializeField, Range(0f, 1f)]
        private float _depthProportion = DEFAULT_DEPTH;
        public float DepthProportion
        {
            get => _depthProportion;
            set
            {
                _depthProportion = value;
                ValueChangeHandler();
            } 
        }
        


        private const float DEFAULT_POSITION = DEFAULT_DEPTH;
        public float YPosition
        {
            get => ModelHeight * YProportion;
            set => YProportion = Mathf.Min(ModelHeight / value, ModelHeight);
        }
        
        private const float DEFAULT_DEPTH = 3f;
        
        
        public float YDepthDistance
        {
            get => ModelHeight * DepthProportion;
            set => DepthProportion = Mathf.Min(ModelHeight / value, ModelHeight);
        }

        
        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void Start()
        {
            ImportManager.Instance.OnStreamVisibilityChange += ModelChangeHandler;
            ImportManager.Instance.OnStreamReceived += ModelChangeHandler;
        }

        private void ModelChangeHandler<A,B>(A a = default, B b = default)
        {
            ModelChangeHandlerNoNotify();
            OnModelChange.Invoke(this);
        }
        
        private void ModelChangeHandlerNoNotify()
        {
            ModelHeight = GameObject.FindGameObjectWithTag("Environment").CalculateRendererBounds().size.y;//TODO
            
            if (YDepthDistance == default) YDepthDistance = DEFAULT_DEPTH;
            if (YPosition == default) YPosition = DEFAULT_POSITION;
        }
        

        private void ValueChangeHandler()
        {
            ValueChangeHandlerNoNotify();
            OnValueChange.Invoke(this);
        }
        
        private void ValueChangeHandlerNoNotify()
        {
            var position = transform.position;
            
            position = new Vector3(position.x, YPosition, position.z);
            transform.position = position;
        }

        public UnityEvent<Camera2DContinuousLevelController> OnValueChange;
        public UnityEvent<Camera2DContinuousLevelController> OnModelChange;
        public UnityEvent<Camera2DContinuousLevelController> OnControllerEnabledChange;

        private void OnValidate()
        {
            ValueChangeHandler();
        }

        private void OnEnable()
        {
            OnControllerEnabledChange.Invoke(this);
        }
        
        private void OnDisable()
        {
            OnControllerEnabledChange.Invoke(this);
        }
    }
}
