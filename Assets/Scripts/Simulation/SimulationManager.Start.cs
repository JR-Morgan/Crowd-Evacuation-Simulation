using PedestrianSimulation.Agent;
using PedestrianSimulation.Simulation.Initialisation;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PedestrianSimulation.Simulation
{
    partial class SimulationManager
    {
        [Header("Prefab References")]
        [SerializeField]
        private GameObject AgentPrefab;

        public SimulationSettings SimulationSettings { get; set; }

        public bool RunSimulation(SimulationSettings settings, GameObject environment)
        {
            if (IsRunning)
            {
                Debug.Log("Unable to start simulation as one is already running!");
                return false;
            }

            IsRunning = true;

            // 1. Random
            Random.InitState(settings.seed);

            // 2. Prepare Environment
            Transform[] c = environment.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in c)
            {
                t.gameObject.layer = (int)(Mathf.Log((uint)navMeshSurface.layerMask.value, 2));
            }

            // 2. NavMesh
            navMeshSurface.BuildNavMesh();

            // 3. Agents

            Agents = settings.agentDistribution.InstantiateAgents(
                agentParent: transform,
                agentsGoal: settings.goal,
                agentPrefab: AgentPrefab,
                numberOfAgents: settings.numberOfAgents,
                environmentModel: environment
                );


            InitialiseUpdater(Agents, settings.NewUpdater());


            // 4 World State
            //WorldStateManager.Instance.enabled = true;

            Debug.Log("Simulation has started!");

            OnSimulationStart.Invoke();
            return IsRunning;
        }


    }


}
