using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class RVOAgent : MonoBehaviour
{




    private NavMeshPath path = null;



    [SerializeField]
    private Vector3 desiredVelocity;

    [SerializeField]
    private int areaMask;

    private Rigidbody rb;

    private bool CalculatePath(Vector2 terminalGoal)
    {
       path = new NavMeshPath();
       NavMesh.CalculatePath(transform.position, terminalGoal, areaMask, path);
    
        return path.status == NavMeshPathStatus.PathComplete;
    }


    void Start()
    {
        if (path == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Goal");
            if (go != null)
            {
                Debug.LogWarning($"{typeof(RVOAgent)} was not properly initialised with a {nameof(path)}!, must call {nameof(CalculatePath)} before {nameof(Start)}", this);
                if (CalculatePath(go.transform.position)) Debug.Log($"{typeof(RVOAgent)} found a backup goal {go} by tag", this);
                else Debug.LogWarning($"{typeof(RVOAgent)} could not find a backup goal {go} by tag", this);
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }





    private void FixedUpdate()
    {
        rb.AddForce(desiredVelocity);
    }
















    //public void Start()
    //{
    //    if(path == null)
    //    {
    //        Debug.LogWarning($"{typeof(RVOAgent)} {{{this}}} was not properly initialised with a {nameof(path)}!, must call {nameof(CalculatePath)} before {nameof(Start)}");
    //    }
    //

    //}

    /*
    [Header("Path")]
    [SerializeField]
    Vector3[] path;
    [SerializeField]
    [Header("Fields")]
    Vector3 desiredVelocity, velocity, steeringTarget;
    [SerializeField]
    bool updateRotation, updatePosition;
    [SerializeField]
    float remainingDistance;

    bool laststop = false;
    [SerializeField]
    bool stop = false;

    private void OnValidate()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.velocity = velocity;
            navMeshAgent.updateRotation = updateRotation;
            navMeshAgent.updatePosition = updatePosition;
            navMeshAgent.isStopped = stop;
        }
    }

    public void Update()
    {

        stop = navMeshAgent.isStopped;
        path = navMeshAgent.path.corners;
        desiredVelocity = navMeshAgent.desiredVelocity;
        velocity = navMeshAgent.velocity;
        updateRotation = navMeshAgent.updateRotation;
        updatePosition = navMeshAgent.updatePosition;
        remainingDistance = navMeshAgent.remainingDistance;
        steeringTarget = navMeshAgent.steeringTarget;
    }*/
}
