using PedestrianSimulation.Agent;
using PedestrianSimulation.Simulation.UpdateStrategies;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace PedestrianSimulation.Simulation
{

    [DisallowMultipleComponent]
    [AddComponentMenu("Simulation/Simulation Manager")]
    public partial class SimulationManager : Singleton<SimulationManager>, ISimulationManager
    {
        #region Scene References
        private NavMeshSurface navMeshSurface;
        #endregion

        [SerializeField]
        private List<PedestrianAgent> _agents = null;
        public IList<PedestrianAgent> Agents
        {
            get => _agents;
            private set
            {
                _agents = value.ToList();
                AgentStates = Agents.Select(a => a.State);
                updater?.Initialise(_agents);
            }
        }

        public bool HasGenerated => Agents != null;
        public bool IsRunning { get; private set; }


        public IEnumerable<AgentState> AgentStates { get; private set; }



        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            navMeshSurface = GetComponent<NavMeshSurface>();
            InitialiseManager();
        }

        #endregion

        #region Simulation
        /// <summary>
        /// Cancels the running simulation
        /// </summary>
        /// <returns><c>false</c> if the simulation was not running; <c>true</c> otherwise</returns>
        public bool CancelSimulation()
        {
            if (IsRunning) InitialiseManager();
            return IsRunning;
        }

        /// <summary>
        /// (Re)Initialises the <see cref="SimulationManager"/>. Will destroy all <see cref="PedestrianAgent"/>s in the scene.
        /// </summary>
        private void InitialiseManager()
        {
            if (HasGenerated)
            {
                //if(WorldStateManager.IsInitialised) WorldStateManager.Instance.enabled = false;

                foreach (PedestrianAgent agent in Agents)
                {
                    Destroy(agent.gameObject);
                }
                _agents = null;
            }
            IsRunning = false;
        }
        #endregion
    }

}
