using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//Was a struct but in order to do reflection in SimulationSetupElement, need this to be a class
public class SimulationSettings
{
    public int seed;
    public int numberOfAgents;
    public Transform goal;

}

[RequireComponent(typeof(NavMeshSurface))]
public class SimulationManager : Singleton<SimulationManager>
{

    #region Serialized Field

    [Header("Prefab References")]
    [SerializeField]
    private GameObject AgentPrefab;

    #endregion;


    private NavMeshSurface navMeshSurface;

    public bool HasGenerated => Agents != null;
    public List<AgentBehaviour> Agents { get; private set; } = null;

    protected override void Awake()
    {
        base.Awake();
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    public void RunSimulation(in SimulationSettings settings, GameObject environment)
    {
        // 1. Random
        Random.InitState(settings.seed);
        
        // 2. NavMesh
        navMeshSurface.BuildNavMesh();

        // 3. Agents
        Agents = AgentFactory.SpawnAllAgents(
            agentParent: transform,
            goal: settings.goal,
            agentPrefab: AgentPrefab,
            numberOfAgents: settings.numberOfAgents,
            environmentModel: environment
            );

        // 4 World State
        WorldStateManager.Instance.Initialise();



    }



}
