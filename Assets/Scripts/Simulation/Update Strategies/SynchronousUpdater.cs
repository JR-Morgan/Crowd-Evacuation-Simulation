using PedestrianSimulation.Agent;
using System.Collections.Generic;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public class SynchronousUpdater : IAgentUpdater<PedestrianAgent>
    {
        public void Initialise(ICollection<PedestrianAgent> agents)
        {
            //No initialisation required
        }

        public void Tick(float timeStep, IEnumerable<PedestrianAgent> agents)
        {
            foreach(var agent in agents)
            {
                agent.UpdateIntentions(timeStep);
            }
        }
    }
}
