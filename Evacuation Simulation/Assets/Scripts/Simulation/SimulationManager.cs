using PedestrianSimulation.Agent;
using PedestrianSimulation.Visualisation;
using System.Collections.Generic;
using PedestrianSimulation.Agent.LocalAvoidance;
using JMTools;
using PedestrianSimulation.Environment;
using PedestrianSimulation.Results;
using Results_Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using AgentState = Results_Core.AgentState;
using Vector3 = UnityEngine.Vector3;

namespace PedestrianSimulation.Simulation
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Simulation/Simulation Manager")]
    public class SimulationManager : Singleton<SimulationManager>
    {
        #region Serialized Field
        [Header("Asset References")]
        [SerializeField]
        private Material heatmapMaterial;
        
        [Header("Prefab References")]
        [SerializeField, Tooltip("Reference to the prefab that should be instantiated for agent initialisation.")]
        private GameObject agentPrefab;

        [SerializeField]
        private SimulationSettings settings;
        public SimulationSettings Settings => settings;
        #endregion;

        #region Events
        [Header("Events"), Tooltip("Called on the frame before the simulation starts.")]
        [SerializeField]
        private UnityEvent _onSimulationStart;
        public UnityEvent OnSimulationStart => _onSimulationStart;

        [SerializeField, Tooltip("Called when the simulation is terminated for any reason (e.g. terminated by user, completed naturally, etc.).")]
        private UnityEvent _onSimulationTerminated;
        public UnityEvent OnSimulationTerminated => _onSimulationTerminated;
        
        [SerializeField, Tooltip("Called when the simulation is naturally ends, called before" + nameof(OnSimulationTerminated))]
        private UnityEvent<SimulationResults> _onSimulationFinished;
        public UnityEvent<SimulationResults> OnSimulationFinished => _onSimulationFinished;
        #endregion

        private GameObject visualSurface;
        
        public bool HasGenerated => Agents != null;
        public float SimulationStartTime { get; private set; }

        public bool IsRunning
        {
            get => SimulationStartTime > default(float);
            private set => SimulationStartTime = value ? Time.time : default;
        }

        public IReadOnlyList<AbstractAgent> Agents { get; private set; }
        
        public HashSet<AbstractAgent> CompletedAgents { get; private set; } = new HashSet<AbstractAgent>();

        protected override void Awake()
        {
            base.Awake();
            InitialiseManager();
        }
        
        public void InitialiseManager()
        {
            if (HasGenerated)
            {
                if (WorldStateManager.IsSingletonInitialised) WorldStateManager.Instance.enabled = false;

                foreach (var agent in Agents)
                {
                    Destroy(agent.gameObject);
                }

                Agents = null;
                CompletedAgents.Clear();
            }
            IsRunning = false;
        }

        public bool RunSimulation(EnvironmentManager environment)
        {
            return settings.useLegacyAgents
                ? RunSimulation<LegacyPedestrianAgent>(environment)
                : RunSimulation<PedestrianAgent>(environment);
        }

        private bool RunSimulation<T>(EnvironmentManager environment) where T : AbstractAgent
        {
            if (IsRunning)
            {
                Debug.Log("Unable to start simulation as one is already running!", this);
            }
            else
            {
                IsRunning = true;
                
                { // 0. Cleanup from potential last run
                    if (visualSurface != null) Destroy(visualSurface);
                    if (settings.goal == null) settings.goal = GameObject.FindGameObjectWithTag("Goal").transform;
                }
                
                { // 1. Initialise Environment
                    Random.InitState(settings.seed);
                    environment.InitialiseEnvironment();
                }
                
                { // 2. Setup Visual Surface
                    
                    var parent = GameObject.FindGameObjectWithTag("Visualisations").transform;
                    visualSurface = InstantiateVisualSurfaceMesh(heatmapMaterial, parent);
                }

                { // 3.1 Instantiate Agents
                    Agents = settings.NewDistribution<T>().InstantiateAgents(
                        agentParent: transform,
                        agentsGoal: settings.goal,
                        agentPrefab: agentPrefab,
                        numberOfAgents: settings.numberOfAgents,
                        environmentModel: environment.gameObject //TODO might change this type to EnvironmentManager
                    ).AsReadOnly();
                }
                
                { // 3.2 Initialise Agents

                    AgentEnvironmentModel initialEnvironmentModel = new AgentEnvironmentModel(); //TODO for now this is the same for all agents
                    
                    ILocalAvoidance localAvoidance = Settings.NewLocalAvoidance();
                    
                    int i = 0;
                    foreach(var agent in Agents)
                    {
                        agent.Initialise(i++, localAvoidance, initialEnvironmentModel);

                        agent.GoalComplete += AgentGoalCompleteHandler;
                        agent.GoalRegress += AgentGoalRegressedHandler;
                    }
                }
                

                { // 4. Enable World State
                    WorldStateManager.Instance.enabled = true;
                }

                { // 5. Invoke Event
                    OnSimulationStart.Invoke();
                    Debug.Log("Simulation has started!", this);
                }
            }

            return IsRunning;
        }

        public bool CancelSimulation()
        {
            if (IsRunning)
            {
                InitialiseManager();
                OnSimulationTerminated.Invoke();
                Debug.Log("Simulation canceled!", this);
            }

            return IsRunning;
        }

        public bool SimulationFinish()
        {
            if (IsRunning)
            {
                OnSimulationFinished.Invoke(ResultsHelper.GenerateResults(
                    numberOfAgents: Agents.Count,
                    realTimeToExecute: SimulationStartTime - Time.deltaTime,
                    timeToEvacuate: WorldStateManager.Instance.CurrentTime,
                    agentStates: WorldStateManager.Instance.AgentStates)
                    );
                
                InitialiseManager();
                OnSimulationTerminated.Invoke();
                Debug.Log("Simulation finished!", this);
            }

            return IsRunning;
        }

        private static GameObject InstantiateVisualSurfaceMesh(Material material, Transform parent = null, string name = "Visual Surface")
        {
            GameObject visualSurfaceGO = new GameObject(name);
            visualSurfaceGO.transform.position = new Vector3(0f, 0.001f, 0f); //Small Y offset to stop Z fighting

            if (parent != null) visualSurfaceGO.transform.parent = parent;

            NavMeshTriangulation navmesh = NavMesh.CalculateTriangulation();
            Mesh mesh = new Mesh
            {
                vertices = navmesh.vertices,
                triangles = navmesh.indices
            };

            //Add Components

            MeshFilter filter = visualSurfaceGO.AddComponent<MeshFilter>();
            filter.mesh = mesh;
            MeshRenderer renderer = visualSurfaceGO.AddComponent<MeshRenderer>();
            renderer.material = material;

            visualSurfaceGO.AddComponent<VisualSurface>();

            return visualSurfaceGO;

        }
        
        
        
        #region Goal Completions
        
        private bool CheckSimulationFinished()
        {
            if (CompletedAgents.Count < Agents.Count) return false;

            var currentTime = WorldStateManager.Instance.CurrentTime;
            
            //Simulation is complete
            Debug.Log($"Simulation Finished after {currentTime:0.##} seconds");
            
            SimulationFinish();
            
            return true;
        }

        private void AgentGoalCompleteHandler(AbstractAgent agent)
        {
            CompletedAgents.Add(agent);
            CheckSimulationFinished();
        }

        private void AgentGoalRegressedHandler(AbstractAgent agent)
        {
            CompletedAgents.Remove(agent);
        }
        #endregion
    }
}