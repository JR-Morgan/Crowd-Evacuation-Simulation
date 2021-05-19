using PedestrianSimulation.Agent;
using PedestrianSimulation.Simulation.Initialisation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



namespace PedestrianSimulation.Simulation
{
    [RequireComponent(typeof(NavMeshSurface))]
    public class LegacySimulationManager : Singleton<LegacySimulationManager>, ISimulationManager
    {

        #region Serialized Field

        [Header("Prefab References")]
        [SerializeField]
        private GameObject AgentPrefab;

        #endregion;


        private NavMeshSurface navMeshSurface;

        public bool IsRunning { get; private set; }
        public bool HasGenerated => Agents != null;
        public List<LegacyPedestrianAgent> Agents { get; private set; } = null;

        protected override void Awake()
        {
            base.Awake();
            navMeshSurface = GetComponent<NavMeshSurface>();
            Initialise();
        }



        /// <summary>
        /// Cancels the running simulation
        /// </summary>
        /// <returns>False if the simulation was not running</returns>
        public bool CancelSimulation()
        {
            if (IsRunning) Initialise();
            return IsRunning;
        }

        /// <summary>
        /// (Re)Initialises the Simulation manager. Will remove all agents in the scene.
        /// </summary>
        public void Initialise()
        {
            if (HasGenerated)
            {
                WorldStateManager.Instance.enabled = false;

                foreach (LegacyPedestrianAgent agent in Agents)
                {
                    Destroy(agent.gameObject);
                }
                Agents = null;
            }
            IsRunning = false;
        }

        /// <summary>
        /// Attempts to initialise and start a simulation with the <paramref name="settings"/>
        /// </summary>
        /// <param name="settings">The <see cref="SimulationSettings"/> that are to be setup</param>
        /// <param name="environment">The <see cref="GameObject"/> that represents the environment of agents</param>
        /// <returns>True if the simulation was successfully setup</returns>
        public bool RunSimulation(SimulationSettings settings, GameObject environment)
        {
            if (IsRunning)
            {
                Debug.Log("Unable to start simulation as one is already running!");

            }
            else
            {
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
                Agents = UniformAgentDistribution<LegacyPedestrianAgent>.InstantiateAgents(
                    agentParent: transform,
                    goal: settings.goal,
                    agentPrefab: AgentPrefab,
                    numberOfAgents: settings.numberOfAgents,
                    environmentModel: environment
                    );

                // 4 World State
                WorldStateManager.Instance.enabled = true;

                Debug.Log("Simulation has started!");

            }

            return IsRunning;
        }



    }
}