
using System;
using System.Linq;
using PedestrianSimulation.Agent.LocalAvoidance;
using UnityEngine;
using UnityEngine.AI;

namespace PedestrianSimulation.Agent
{
    [SelectionBase, DisallowMultipleComponent]
    [AddComponentMenu("Simulation/Pedestrian Agent")]
    public class PedestrianAgent : AbstractAgent
    {
        private const float GOAL_RADIUS = 0.5f;
        private const float DEFAULT_AGENT_SPEED = 3.5f;
        private const float AGENT_RADIUS = 0.5f;

        #region Agent State

        [SerializeField]
        private AgentState _state;
        public override AgentState State { get => _state; internal set => _state = value; }

        private AgentEnvironmentModel environmentModel;
        #endregion

        private ILocalAvoidance localAvoidance;
        public Vector3 IntendedVelocity { get; private set; }
        
        [SerializeField]
        private int cornerIndex = 0;
        private Vector3[] path = null;

        public override bool SetGoal(Vector3 terminalGoal)
        {
            NavMeshPath p = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, terminalGoal, NavMesh.AllAreas, p);

            path = p.corners;
            //navAgent.CalculatePath(terminalGoal, path);


            return p.status == NavMeshPathStatus.PathComplete;
        }

        public override void Initialise(int id, ILocalAvoidance localAvoidance, AgentEnvironmentModel initialEnvironmentModel)
        {
            this.name = $"{nameof(PedestrianAgent)} {id}";
            this.localAvoidance = localAvoidance;


            State = new AgentState(
                id: id,
                active: this.enabled,
                radius: AGENT_RADIUS,
                desiredSpeed: DEFAULT_AGENT_SPEED,
                goal: CalculateCurrentGoal(transform.position),
                position: transform.position,
                rotation: transform.rotation,
                velocity: Vector3.zero);

            environmentModel = initialEnvironmentModel;
        }

        public void Update()
        {
            UpdateIntentions(Time.deltaTime);
            Debug.DrawLine(transform.position, State.goal, Color.green);
        }

        private void Sense()
        {
            //environmentModel.Walls.Add();
        }

        public void LateUpdate()
        {
            CommitAction();
        }

        public override void UpdateIntentions(float timeStep)
        {
            //Vector3 desiredGoal = CalculateCurrentGoal(State.position);
            //if (State.goal != desiredGoal)
            //{
                //TODO stop agent
            //}

            //TODO use Time step

            IntendedVelocity = localAvoidance.NextVelocity(State, environmentModel) * timeStep;
        }
        public void CommitAction()
        {
            var position = transform.position += IntendedVelocity;

            State = new AgentState(
                id: State.id,
                active: this.enabled,
                radius: AGENT_RADIUS,
                desiredSpeed: State.desiredSpeed,
                goal: CalculateCurrentGoal(position),
                position: position,
                rotation: transform.rotation,
                velocity: IntendedVelocity);
        }

        private Vector3 CalculateCurrentGoal(Vector3 position)
        {
            float distance = Vector3.Distance(position, path[cornerIndex]);
            
            if (distance < GOAL_RADIUS)
            {
                cornerIndex++;
                if (cornerIndex >= path.Length)
                {
                    Debug.Log("Terminal Goal is Complete!");
                }

                cornerIndex = Mathf.Min(cornerIndex, path.Length - 1);
            }

            return path[cornerIndex]; ;
        }


    }
}
