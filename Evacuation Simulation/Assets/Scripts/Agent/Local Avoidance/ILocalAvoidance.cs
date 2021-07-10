using UnityEngine;

namespace PedestrianSimulation.Agent.LocalAvoidance
{
    public interface ILocalAvoidance
    {
        public Vector3 NextVelocity(AgentState state, AgentEnvironmentModel model);
    }
}
