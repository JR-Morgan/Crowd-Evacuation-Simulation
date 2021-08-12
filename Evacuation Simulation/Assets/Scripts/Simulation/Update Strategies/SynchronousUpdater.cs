using PedestrianSimulation.Agent;
using System.Collections.Generic;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public class SynchronousUpdater<T> : IAgentUpdater<T> where T : IAgent
    {
        public void Tick(float timeStep, IEnumerable<T> agents)
        {
            foreach(var agent in agents)
            {
                agent.UpdateIntentions(timeStep);
            }
        }
    }
}
