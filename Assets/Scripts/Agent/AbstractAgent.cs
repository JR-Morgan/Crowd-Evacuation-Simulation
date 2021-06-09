using UnityEngine;

namespace PedestrianSimulation.Agent
{
    public abstract class AbstractAgent : MonoBehaviour
    {
        public abstract bool SetGoal(Vector3 terminalGoal);
        public abstract void Initialise(int id);

        public abstract void UpdateIntentions(float timeStep);
    }
}
