using PedestrianSimulation.Simulation;
using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSimulation.Visualisation
{

    [RequireComponent(typeof(MeshRenderer))]
    public class VisualSurface : MonoBehaviour
    {

        private MeshRenderer _renderer;
        private LegacySimulationManager simulationManager;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            simulationManager = LegacySimulationManager.Instance;
        }

        private void LateUpdate()
        {
            Vector4[] positions = HeatmapHelper.GeneratePositionArray(simulationManager.Agents);

            _renderer.material.SetVectorArray("positions", positions);
            _renderer.material.SetInt("numOfAgents", Mathf.Min(simulationManager.Agents.Count, HeatmapHelper.MAX_ARRAY_SIZE));
        }
    }
}
