using PedestrianSimulation.Agent;
using PedestrianSimulation.Agent.LocalAvoidance;
using PedestrianSimulation.Simulation.Initialisation;
using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace PedestrianSimulation.Simulation
{
    partial class SFMSimulationManager
    {
        [Header("Prefab References")]
        [SerializeField]
        private GameObject AgentPrefab;

        [SerializeField]
        private SimulationSettings settings;
        public SimulationSettings Settings => settings;

        public bool RunSimulation(GameObject environment)
        {
            if (IsRunning)
            {
                Debug.Log("Unable to start simulation as one is already running!");
                return false;
            }

            IsRunning = true;

            { // 1. Random
                Random.InitState(Settings.seed);
            }

            { // 2. Prepare Environment
                Transform[] c = environment.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in c)
                {
                    t.gameObject.layer = (int)(Mathf.Log((uint)navMeshSurface.layerMask.value, 2));
                }
            }

            { // 2. NavMesh
                navMeshSurface.BuildNavMesh();
            }

            { // 3. Agents
                Agents = Settings.NewDistribution<PedestrianAgent>().InstantiateAgents(
                    agentParent: transform,
                    agentsGoal: Settings.goal,
                    agentPrefab: AgentPrefab,
                    numberOfAgents: Settings.numberOfAgents,
                    environmentModel: environment
                    );
                NavMeshTriangulation navmeshTriangulation = NavMesh.CalculateTriangulation();
                var walls = NavmeshProcessor.GetNavmeshBoundaryEdges(navmeshTriangulation);
                AgentEnvironmentModel initialEnvironmentModel = new AgentEnvironmentModel(walls); //TODO for now this is the same for all agents
                ILocalAvoidance localAvoidance = Settings.NewLocalAvoidance();

                int i = 0;
                foreach(var agent in Agents)
                {
                    agent.Initialise(i++, localAvoidance, initialEnvironmentModel);
                }


                InitialiseUpdater(Agents, Settings.NewUpdater<PedestrianAgent>());
            }


            { // 4 World State
             //WorldStateManager.Instance.enabled = true;
            }

            Debug.Log("Simulation has started!");

            OnSimulationStart.Invoke();
            return IsRunning;
        }
    }
}
