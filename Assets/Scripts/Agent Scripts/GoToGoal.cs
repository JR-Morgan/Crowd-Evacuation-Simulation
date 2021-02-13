using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GoToGoal : MonoBehaviour
{
    private Vector3 goal;
    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        GameObject goal = GameObject.FindGameObjectWithTag("Goal");

        //navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        navMeshAgent.SetDestination(goal.transform.position);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
