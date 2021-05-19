using PedestrianSimulation.Agent.Behaviour.LocalAvoidance;
using UnityEngine;
using UnityEngine.AI;

namespace PedestrianSimulation.Agent
{
    [SelectionBase, DisallowMultipleComponent]
    [AddComponentMenu("Simulation/Pedestrian Agent")]
    public class PedestrianAgent : MonoBehaviour, IAgent
    {
        private const float GOAL_RADIUS = 0.5f;
        private const float DEFAULT_AGENT_SPEED = 3.5f;

        [SerializeField]
        private AgentState _state;
        public AgentState State { get => _state; private set => _state = value; }

        public Vector3 IntendedVelocity { get; private set; }

        private int cornerIndex = 0;
        private Vector3[] path = null;

        public bool SetGoal(Vector3 terminalGoal)
        {
            NavMeshPath p = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, terminalGoal, NavMesh.AllAreas, p);

            path = p.corners;
            //navAgent.CalculatePath(terminalGoal, path);


            return p.status == NavMeshPathStatus.PathComplete;
        }

        public void Initialise(int id)
        {
            this.name = $"{typeof(PedestrianAgent)} {id}";
            State = new AgentState(
                id: id,
                desiredSpeed: DEFAULT_AGENT_SPEED,
                goal: CalculateCurrentGoal(transform.position),
                position: transform.position,
                velocity: Vector3.zero);
        }


        public void UpdateIntentions(float timeStep)
        {
            Vector3 desiredGoal = CalculateCurrentGoal(State.position);
            if (State.goal != desiredGoal)
            {

            }

            IntendedVelocity = CalculateNextVelocity(State, timeStep);
        }

        private Vector3 CalculateCurrentGoal(Vector3 position)
        {
            if (Vector3.Distance(position, path[cornerIndex]) < GOAL_RADIUS)
            {
                cornerIndex++;
                if (cornerIndex >= path.Length)
                {
                    //GOAL IS COMPLETE!
                }
            }

            return path[Mathf.Max(cornerIndex, path.Length - 1)]; ;
        }

        public void CommitAction()
        {
            transform.position += IntendedVelocity;

            State = new AgentState(
                id: State.id,
                desiredSpeed: State.desiredSpeed,
                goal: CalculateCurrentGoal(transform.position),
                position: transform.position,
                velocity: IntendedVelocity);
        }



        private static Vector3 CalculateNextVelocity(in AgentState current, float timeStep) //TODO calibrate timeStep to ms
        {
            var states = Simulation.SimulationManager.Instance.AgentStates; //TODO Singleton reference is costly

            Vector3 drivingForce = SocialForceModel.DrivingForce(current);
            Vector3 interactionForce = SocialForceModel.InteractionForce(current, states);

            return drivingForce + interactionForce * timeStep;
        }

    }
}
