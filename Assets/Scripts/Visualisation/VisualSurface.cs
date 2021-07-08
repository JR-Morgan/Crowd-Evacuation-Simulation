using PedestrianSimulation.Simulation;
using System;
using System.Linq;
using UnityEngine;

namespace PedestrianSimulation.Visualisation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshRenderer))]
    public class VisualSurface : MonoBehaviour
    {

        private MeshRenderer _renderer;

        private int numOfAgentsProp, bufferProp;
        private ComputeBuffer _buffer;

        private Func<Vector4[]> getPositions;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();

            numOfAgentsProp = Shader.PropertyToID("numOfAgents");
            bufferProp = Shader.PropertyToID("positionData");
        }

        private void Start()
        {
            //LegacySimulationManager.Instance.OnSimulationStart.AddListener(SimulationStartHandler);
            SimulationManager.Instance.OnSimulationTerminated.AddListener(SimulationStopHandler);
            //this.enabled = false;
            SimulationStartHandler();
        }

        private void SimulationStartHandler()
        {
            var simulationManager = SimulationManager.Instance;

            int agentCount = simulationManager.Agents.Count;
            if(agentCount <= 0 )
            {
                this.enabled = false;
                return;
            }
            
            CreateComputeBuffer(agentCount);

            getPositions = () =>
            {
                return ShaderHelper.ToHomogeneousCoordinates(simulationManager.Agents).ToArray();
            };
            this.enabled = true;
        }

        private void SimulationStopHandler()
        {
            var worldState = WorldStateManager.Instance;
            
            int agentCount = worldState.FlatAgentStates.Count;
            if(agentCount <= 0 )
            {
                this.enabled = false;
                return;
            }
            
            CreateComputeBuffer(agentCount);

            Vector4[] pos = ShaderHelper.ToHomogeneousCoordinates(worldState.FlatAgentStates).ToArray();
            getPositions = () => pos;
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
            _buffer.SetData(getPositions());

            _renderer.material.SetInt(numOfAgentsProp, _buffer.count);

            _renderer.material.SetBuffer(bufferProp, _buffer);
        }

        ~VisualSurface()
        {
            _buffer?.Dispose();
        }
    }
}
