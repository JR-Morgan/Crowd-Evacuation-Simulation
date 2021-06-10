using PedestrianSimulation.Agent;
using PedestrianSimulation.Simulation.UpdateStrategies;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace PedestrianSimulation.Simulation
{


    public partial class SFMSimulationManager : Singleton<SFMSimulationManager>, ISimulationManager<PedestrianAgent>
    {
        #region Scene References
        private NavMeshSurface navMeshSurface;
        #endregion

        #region Events
        [SerializeField]
        private UnityEvent _onSimulationStart;
        public UnityEvent OnSimulationStart => _onSimulationStart;

        [SerializeField]
        private UnityEvent _onSimulationStop;
        public UnityEvent OnSimulationStop => _onSimulationStop;
        #endregion

        [SerializeField]
        private List<PedestrianAgent> _agents = null;
        public IList<PedestrianAgent> Agents
        {
            get => _agents;
            private set
            {
                _agents = value.ToList();
                updater?.Initialise(_agents);
            }
        }

        public bool HasGenerated => Agents != null;
        public bool IsRunning { get; private set; }




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
            if (IsRunning)
            {
                InitialiseManager();
                OnSimulationStop.Invoke();
            }
            return IsRunning;
        }

        /// <summary>
        /// (Re)Initialises the <see cref="SFMSimulationManager"/>. Will destroy all <see cref="PedestrianAgent"/>s in the scene.
        /// </summary>
        private void InitialiseManager()
        {
            if (HasGenerated)
            {
                if(WorldStateManager.IsSingletonInitialised) WorldStateManager.Instance.enabled = false;

                foreach (PedestrianAgent agent in Agents)
                {
                    Destroy(agent.gameObject);
                }
                Agents = null;
            }
            IsRunning = false;
        }
        #endregion
    }

}
