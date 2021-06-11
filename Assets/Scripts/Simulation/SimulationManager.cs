using PedestrianSimulation.Agent;
using PedestrianSimulation.Visualisation;
using System.Collections.Generic;
using PedestrianSimulation.Agent.LocalAvoidance;
using JMTools;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

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
        [SerializeField]
        private GameObject agentPrefab;

        [SerializeField]
        private SimulationSettings settings;
        public SimulationSettings Settings => settings;
        #endregion;

        #region Events
        [SerializeField, Header("Events")]
        private UnityEvent _onSimulationStart;
        public UnityEvent OnSimulationStart => _onSimulationStart;

        [SerializeField]
        private UnityEvent _onSimulationStop;
        public UnityEvent OnSimulationStop => _onSimulationStop;
        #endregion

        private GameObject visualSurface;
        private NavMeshSurface navMeshSurface;

        public bool HasGenerated => Agents != null;
        public bool IsRunning { get; private set; }
        public IReadOnlyList<AbstractAgent> Agents { get; private set; } = null;

        protected override void Awake()
        {
            base.Awake();
            navMeshSurface = GetComponent<NavMeshSurface>();
            InitialiseManager();
        }


        public bool CancelSimulation()
        {
            if (IsRunning)
            {
                InitialiseManager();
                OnSimulationStop.Invoke();
                Debug.Log("Simulation canceled!", this);
            }

            return IsRunning;
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
            }
            IsRunning = false;
        }

        public bool RunSimulation(GameObject environment)
        {
            return settings.useLegacyAgents
                ? RunSimulation<LegacyPedestrianAgent>(environment)
                : RunSimulation<PedestrianAgent>(environment);
        }

        private bool RunSimulation<T>(GameObject environment) where T : AbstractAgent
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


                { // 1. Initialise Random

                    Random.InitState(settings.seed);
                }

                { // 2. Prepare Environment
                    Transform[] c = environment.GetComponentsInChildren<Transform>(true);
                    foreach (Transform t in c)
                    {
                        t.gameObject.layer = (int)(Mathf.Log((uint)navMeshSurface.layerMask.value, 2));
                    }
                }

                { // 2. Build NavMesh
                    navMeshSurface.BuildNavMesh();
                }

                { // 3. Setup Visual Surface
                    
                    var parent = GameObject.FindGameObjectWithTag("Visualisations").transform;
                    visualSurface = InstantiateVisualSurfaceMesh(heatmapMaterial, parent);
                }

                { // 4.1 Instantiate Agents
                    Agents = settings.NewDistribution<T>().InstantiateAgents(
                        agentParent: transform,
                        agentsGoal: settings.goal,
                        agentPrefab: agentPrefab,
                        numberOfAgents: settings.numberOfAgents,
                        environmentModel: environment
                    ).AsReadOnly();
                }
                
                { // 4.2 Initialise Agents
                    NavMeshTriangulation navmeshTriangulation = NavMesh.CalculateTriangulation();
                    IList<Wall> walls = NavmeshProcessor.GetNavmeshBoundaryEdges(navmeshTriangulation);
                    AgentEnvironmentModel initialEnvironmentModel = new AgentEnvironmentModel(walls); //TODO for now this is the same for all agents
                    
                    ILocalAvoidance localAvoidance = Settings.NewLocalAvoidance();
                    
                    int i = 0;
                    foreach(var agent in Agents)
                    {
                        agent.Initialise(i++, localAvoidance, initialEnvironmentModel);
                    }
                }
                

                { // 5. Enable World State
                    WorldStateManager.Instance.enabled = true;
                }

                { // 6. Invoke Event
                    OnSimulationStart.Invoke();
                    Debug.Log("Simulation has started!", this);
                }
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


    }
}