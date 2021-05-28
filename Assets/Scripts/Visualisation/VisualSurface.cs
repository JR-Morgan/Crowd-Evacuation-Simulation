using PedestrianSimulation.Simulation;
using System;
using System.Linq;
using UnityEngine;

namespace PedestrianSimulation.Visualisation
{

    [RequireComponent(typeof(MeshRenderer))]
    public class VisualSurface : MonoBehaviour
    {

        private MeshRenderer _renderer;

        private int numOfAgentsProp, bufferProp;
        private ComputeBuffer _buffer;

        private Func<Vector4[]> GetPositions;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();

            numOfAgentsProp = Shader.PropertyToID("numOfAgents");
            bufferProp = Shader.PropertyToID("positionData");
        }

        private void Start()
        {
            //LegacySimulationManager.Instance.OnSimulationStart.AddListener(SimulationStartHandler);
            LegacySimulationManager.Instance.OnSimulationStop.AddListener(SimulationStopHandler);
            //this.enabled = false;
            SimulationStartHandler();
        }

        private void SimulationStartHandler()
        {
            var simulationManager = LegacySimulationManager.Instance;

            CreateComputeBuffer(simulationManager.Agents.Count);

            GetPositions = () =>
            {
                return ShaderHelper.ToHomogeneousCoordinates(simulationManager.Agents).ToArray();
            };
            this.enabled = true;
        }

        private void SimulationStopHandler()
        {
            var worldState = WorldStateManager.Instance;

            CreateComputeBuffer(worldState.FlatAgentStates.Count);

            Vector4[] pos = ShaderHelper.ToHomogeneousCoordinates(worldState.FlatAgentStates).ToArray();
            GetPositions = () => pos;
            this.enabled = true;
        }

        private void CreateComputeBuffer(int count, int stride = sizeof(float) * 4)
        {
            _buffer?.Dispose();
            _buffer = new ComputeBuffer(count, stride);
            GC.SuppressFinalize(_buffer); 
        }


        private void LateUpdate()
        {
            _buffer.SetData(GetPositions());

            _renderer.material.SetInt(numOfAgentsProp, _buffer.count);

            _renderer.material.SetBuffer(bufferProp, _buffer);
        }

        ~VisualSurface()
        {
            _buffer?.Dispose();
        }
    }
}
