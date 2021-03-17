using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
[RequireComponent(typeof(NavMeshAgent))]
public class AgentBehaviour : MonoBehaviour
{

    private NavMeshAgent navAgent;
    private NavMeshPath path  = null;

    public bool CalculatePath(Vector2 terminalGoal)
    {
        path = new NavMeshPath();
        navAgent.CalculatePath(terminalGoal, path);


        return path.status == NavMeshPathStatus.PathComplete;
    }

    void Awake()
    {
        navAgent = this.GetComponent<NavMeshAgent>();
        //navAgent.updatePosition = false;
        //navAgent.updateRotation = false;
    }

    void Start()
    {
        //NavMesh.avoidancePredictionTime = 5f;
        //Disable automatic agent movement so that we can apply it's desired velocity in Tick();
        //navAgent.isStopped = true;
        //if(terminalGoal == null) terminalGoal = GameObject.FindGameObjectWithTag("Goal").transform;

        if (path == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Goal");
            if(go != null)
            {
                Debug.LogWarning($"{typeof(AgentBehaviour)} was not properly initialised with a {nameof(path)}!, must call {nameof(CalculatePath)} before {nameof(Start)}", this);
                if (CalculatePath(go.transform.position)) Debug.Log($"{typeof(AgentBehaviour)} found a backup goal {go} by tag", this);
                else Debug.LogWarning($"{typeof(AgentBehaviour)}  could not find a backup goal {go} by tag", this);
            }

        }
        if (path != null)
        {
            navAgent.SetPath(path);
        }
        
    }

    public AgentState State {
        get => new AgentState { active = AgentActive, rotation = this.transform.rotation, position = this.transform.position, velocity = this.navAgent.velocity };
        set {
            AgentActive = value.active;
            transform.position = value.position;
            navAgent.nextPosition = value.position;
            transform.rotation = value.rotation;
            navAgent.velocity = value.velocity;
        }
    }


    private bool AgentActive
    {
        get => navAgent.obstacleAvoidanceType != ObstacleAvoidanceType.NoObstacleAvoidance;
        set => navAgent.obstacleAvoidanceType = value ? ObstacleAvoidanceType.HighQualityObstacleAvoidance : ObstacleAvoidanceType.NoObstacleAvoidance;
    }


    private void Update()
    {

        if (Vector3.Distance(navAgent.destination, transform.position) < 0.5f)
        {
            AgentActive = false;
        }
    }

    //private void FixedUpdate()
    //{
        //Vector3 velocity = desiredVelocity;
        
    //}

}
