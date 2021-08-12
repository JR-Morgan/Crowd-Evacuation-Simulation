using System;
using PedestrianSimulation.Agent.LocalAvoidance;
using UnityEngine;

namespace PedestrianSimulation.Agent
{
    public interface IAgent 
    {
        public AgentState State { get; set; }
        
        public bool TrySetGoal(Vector3 terminalGoal);
        
        public void UpdateIntentions(float timeStep);

        public event Action<AbstractAgent> GoalComplete;
        
        public event Action<AbstractAgent> GoalRegress;
        
    }
}
