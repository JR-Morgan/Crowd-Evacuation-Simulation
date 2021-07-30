using PedestrianSimulation.Agent.LocalAvoidance;
using PedestrianSimulation.Simulation;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace PedestrianSimulation.Agent
{
    [SelectionBase, DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    [AddComponentMenu("Simulation/Legacy/Pedestrian Agent")]
    public class LegacyPedestrianAgent : AbstractAgent
    {
        private const float GOAL_DISTANCE_THRESHOLD = 2f;
        private NavMeshAgent navAgent;
        private NavMeshPath path = null;
        private int id;
        public override void Initialise(int id, ILocalAvoidance localAvoidance, AgentEnvironmentModel initialEnvironmentModel)
        {
            this.id = id;
            this.name = $"{nameof(LegacyPedestrianAgent)} {id}";
        }

        public override bool TrySetGoal(Vector3 terminalGoal)
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
                    Debug.LogWarning($"{typeof(LegacyPedestrianAgent)} was not properly initialised with a {nameof(path)}!, must call {nameof(TrySetGoal)} before {nameof(Start)}", this);
                    if (TrySetGoal(go.transform.position)) Debug.Log($"{typeof(LegacyPedestrianAgent)} found a backup goal {go} by tag", this);
                    else Debug.LogWarning($"{typeof(LegacyPedestrianAgent)}  could not find a backup goal {go} by tag", this);
                }

            }
            if (path != null)
            {
                navAgent.SetPath(path);
            }

        }

        public override AgentState State
        {
            get
            {
                return new AgentState(
                    id: this.id,
                    active: this.enabled,
                    radius: navAgent.radius,
                    desiredSpeed: navAgent.speed,
                    goal: navAgent.destination,
                    position: transform.position,
                    rotation: transform.rotation,
                    velocity: navAgent.desiredVelocity);
            }
            set
            {
                Debug.Assert(this.id == value.id, $"Error, ID: {{{value.id}}} does not match ID: {{{this.id}}} of this {typeof(LegacyPedestrianAgent)}", this);
                
                AgentActive = value.active;
                //navAgent.radius = value.radius;
                navAgent.speed = value.desiredSpeed;

                transform.position = value.position;
                navAgent.nextPosition = value.position;

                transform.rotation = value.rotation;
                navAgent.velocity = value.velocity;
            }
        }


        private bool AgentActive
        {
            get => this.enabled;
            set
            {
                bool previous = this.enabled;
                
                navAgent.obstacleAvoidanceType = value ? ObstacleAvoidanceType.HighQualityObstacleAvoidance : ObstacleAvoidanceType.NoObstacleAvoidance;
                this.enabled = value;

                if (!previous && enabled)
                {
                    OnGoalRegress();
                }
            }
        }


        private void Update()
        {
            if (Vector3.Distance(navAgent.destination, transform.position) < GOAL_DISTANCE_THRESHOLD)
            {
                AgentActive = false;
                OnGoalComplete();
            }
        }

        public override void UpdateIntentions(float timeStep)
        {
            //TODO
        }

    }
}