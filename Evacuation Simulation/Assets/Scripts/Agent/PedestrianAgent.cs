
using System;
using System.Linq;
using JMTools.Geometry.Cellular;
using PedestrianSimulation.Agent.LocalAvoidance;
using PedestrianSimulation.Environment;
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
        public override AgentState State { get => _state; set => _state = value; }

        private AgentEnvironmentModel environmentModel;
        #endregion

        private ILocalAvoidance localAvoidance;
        public Vector3 IntendedVelocity { get; private set; }
        
        [SerializeField]
        private int cornerIndex = 0;
        private Vector3[] path = null;

        public override bool TrySetGoal(Vector3 terminalGoal)
        {
            NavMeshPath p = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, terminalGoal, NavMesh.AllAreas, p);

            path = p.corners;

            return p.status == NavMeshPathStatus.PathComplete;
        }

        public override void Initialise(int id, ILocalAvoidance localAvoidance, AgentEnvironmentModel initialEnvironmentModel)
        {
            this.name = $"{nameof(PedestrianAgent)} {id}";
            this.localAvoidance = localAvoidance;

            State = ConstructState(id, AGENT_RADIUS, DEFAULT_AGENT_SPEED, CalculateCurrentGoal(transform.position), Vector3.zero);

            environmentModel = initialEnvironmentModel;
        }

        public void Update()
        {
            
        }

        private void Sense()
        {
            var agentIndex = EnvironmentManager.IndexFunction(transform.position);
            var chunkIndices = CellularCircles2D.IndicesInCircle(agentIndex, 2);

            
            foreach (var i in chunkIndices)
            {
                if (!EnvironmentManager.TryGetChunk(i, out Chunk c)) continue; //If no valid chunk exists for this index
                
                foreach (Wall w in c.walls)
                {
                    environmentModel.Walls.Add(w);
                }
            }
        }

        public void LateUpdate()
        {
            CommitAction();
            Sense();
        }

        public override void UpdateIntentions(float timeStep)
        {
            IntendedVelocity = localAvoidance.NextVelocity(State, environmentModel) * timeStep;
        }
        
        public void CommitAction()
        {
            var position = transform.position = State.position + IntendedVelocity;
            
            State = ConstructState(State.id, State.radius, State.desiredSpeed, CalculateCurrentGoal(position), IntendedVelocity);
            
            transform.rotation = Quaternion.LookRotation(IntendedVelocity, Vector3.up);
        }

        private Vector3 CalculateCurrentGoal(Vector3 position)
        {
            float distance = Vector3.Distance(position, path[cornerIndex]);
            
            if (distance < GOAL_RADIUS)
            {
                cornerIndex++;
                if (cornerIndex >= path.Length)
                {
                    this.enabled = false;
                    OnGoalComplete();
                    Debug.Log("Terminal Goal is Complete!");
                }

                cornerIndex = Mathf.Min(cornerIndex, path.Length - 1);
            }

            return path[cornerIndex]; ;
        }

        private void OnDrawGizmosSelected()
        {
            //Debug.DrawLine(transform.position, State.goal, Color.green);
            
            #if UNITY_EDITOR
            if (EnvironmentManager.TryGetChunk(transform.position, out Chunk chunk))
            {
                ChunkVisualiser.Dict[chunk].Selected = true;
            }
            #endif
        }
    }
}
