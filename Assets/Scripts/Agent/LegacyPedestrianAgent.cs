using PedestrianSimulation.Simulation;
using UnityEngine;
using UnityEngine.AI;

namespace PedestrianSimulation.Agent
{
    [SelectionBase, DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    [AddComponentMenu("Simulation/Legacy/Pedestrian Agent")]
    public class LegacyPedestrianAgent : AbstractAgent
    {
        private const float GOAL_DISTANCE_THREASHOLD = 2f;
        private NavMeshAgent navAgent;
        private NavMeshPath path = null;

        public override void Initialise(int id)
        {
            this.name = $"{nameof(LegacyPedestrianAgent)} {id}";
        }

        public override bool SetGoal(Vector3 terminalGoal)
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
            NavMesh.avoidancePredictionTime = 5f;
            //Disable automatic agent movement so that we can apply it's desired velocity in Tick();
            //navAgent.isStopped = true;
            //if(terminalGoal == null) terminalGoal = GameObject.FindGameObjectWithTag("Goal").transform;

            if (path == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("Goal");
                if (go != null)
                {
                    Debug.LogWarning($"{typeof(LegacyPedestrianAgent)} was not properly initialised with a {nameof(path)}!, must call {nameof(SetGoal)} before {nameof(Start)}", this);
                    if (SetGoal(go.transform.position)) Debug.Log($"{typeof(LegacyPedestrianAgent)} found a backup goal {go} by tag", this);
                    else Debug.LogWarning($"{typeof(LegacyPedestrianAgent)}  could not find a backup goal {go} by tag", this);
                }

            }
            if (path != null)
            {
                navAgent.SetPath(path);
            }

        }

        public AgentStateModel State
        {
            get => new AgentStateModel { active = AgentActive, rotation = this.transform.rotation, position = this.transform.position, velocity = this.navAgent.velocity };
            set
            {
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
            set
            {
                navAgent.obstacleAvoidanceType = value ? ObstacleAvoidanceType.HighQualityObstacleAvoidance : ObstacleAvoidanceType.NoObstacleAvoidance;
                this.enabled = value;
            }
        }


        private void Update()
        {

            if (Vector3.Distance(navAgent.destination, transform.position) < GOAL_DISTANCE_THREASHOLD)
            {
                AgentActive = false;
            }
        }

        //private void FixedUpdate()
        //{
        //Vector3 velocity = desiredVelocity;

        //}

    }
}