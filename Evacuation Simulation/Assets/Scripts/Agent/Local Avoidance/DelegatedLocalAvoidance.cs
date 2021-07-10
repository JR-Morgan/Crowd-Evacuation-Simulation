using System;
using UnityEngine;
namespace PedestrianSimulation.Agent.LocalAvoidance
{
    public delegate Vector3 AgentActionFunction(AgentState state, AgentEnvironmentModel model);
    public class DelegatedLocalAvoidance : ILocalAvoidance
    {
        private readonly AgentActionFunction nextVelocity;

        public DelegatedLocalAvoidance(AgentActionFunction nextVelocity)
        {
            this.nextVelocity = nextVelocity;
        }

        public Vector3 NextVelocity(AgentState state, AgentEnvironmentModel model)
        {
            return nextVelocity.Invoke(state, model);
        }
    }
}