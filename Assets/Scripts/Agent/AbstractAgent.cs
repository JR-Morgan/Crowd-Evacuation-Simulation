using PedestrianSimulation.Agent.LocalAvoidance;
using UnityEngine;

namespace PedestrianSimulation.Agent
{
    public abstract class AbstractAgent : MonoBehaviour
    {
        public abstract AgentState State { get; internal set; }

        public abstract bool SetGoal(Vector3 terminalGoal);
        public abstract void Initialise(int id, ILocalAvoidance localAvoidance, AgentEnvironmentModel initialEnvironmentModel);

        public abstract void UpdateIntentions(float timeStep);
    }
}
