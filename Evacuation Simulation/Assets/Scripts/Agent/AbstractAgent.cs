using System;
using PedestrianSimulation.Agent.LocalAvoidance;
using UnityEngine;

namespace PedestrianSimulation.Agent
{
    public abstract class AbstractAgent : MonoBehaviour, IAgent
    {
        public abstract AgentState State { get; set; }

        public abstract bool TrySetGoal(Vector3 terminalGoal);

        public abstract void Initialise(int id, ILocalAvoidance localAvoidance, AgentEnvironmentModel initialEnvironmentModel);

        public abstract void UpdateIntentions(float timeStep);

        public event Action<AbstractAgent> GoalComplete;
        public event Action<AbstractAgent> GoalRegress;

        protected void OnGoalComplete()
        {
            GoalComplete?.Invoke(this);
        }
        
        protected void OnGoalRegress()
        {
            GoalRegress?.Invoke(this);
        }

        protected virtual AgentState ConstructState(int id, float radius, float desiredSpeed, Vector3 goal, Vector3 velocity)
        {
            return new AgentState(
                id: id,
                active: this.enabled,
                radius: radius,
                desiredSpeed: desiredSpeed,
                goal: goal,
                position: transform.position,
                rotation: transform.rotation,
                velocity: velocity);
        }

        public override string ToString() => $"{this.GetType()}{{id:{State.id}}}";
    }
}
