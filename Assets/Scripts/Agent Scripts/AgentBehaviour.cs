using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
[RequireComponent(typeof(NavMeshAgent))]
public class AgentBehaviour : MonoBehaviour
{
    //private static uint pendingPath = 0;
    //public static bool hasStarted { get; private set; } = false;
    //private delegate void AgentStartEvent();
    //private static event AgentStartEvent OnStart;

    //public static void CheckPathPending()
    //{
    //    if (pendingPath <= 0 && !hasStarted)
    //    {
    //        OnStart.Invoke();

    //        foreach(var d in OnStart.GetInvocationList())
    //        {
    //            OnStart -= (AgentStartEvent)d;
    //        }
    //    }
    //}

    private NavMeshAgent navAgent;
    private NavMeshPath path  = null;

    public bool CalcualtePath(Vector2 terminalGoal)
    {
        path = new NavMeshPath();
        navAgent.CalculatePath(terminalGoal, path);


        return path.status == NavMeshPathStatus.PathComplete;
    }

    void Awake()
    {
        navAgent = this.GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        //Disable automatic agent movement so that we can apply it's desired velocity in Tick();
        //navAgent.isStopped = true;
        //if(terminalGoal == null) terminalGoal = GameObject.FindGameObjectWithTag("Goal").transform;

        //if(path == null) CalcualtePath(terminalGoal.position);
        if (path != null)
        {
            navAgent.SetPath(path);
        }
        
    }

    //[SerializeField]
    //bool update = false;
    //[SerializeField]
    //bool fixedUpdate = false;

    //public void FixedUpdate()
    //{
    //    if (update && fixedUpdate)
    //        Tick(Time.fixedDeltaTime);
    //}

    //private void Update()
    //{
    //    if(update && !fixedUpdate)
    //    {
    //        navAgent.isStopped = false;
    //        Tick(Time.deltaTime);
    //    }
    //}

    //public void Tick(float timeStep)
    //{
    //    Debug.Log($"Path: {navAgent.pathStatus}, Velocity: {navAgent.velocity}, Desired: {navAgent.desiredVelocity}");
    //    transform.position += this.navAgent.desiredVelocity * timeStep;
    //}
}
