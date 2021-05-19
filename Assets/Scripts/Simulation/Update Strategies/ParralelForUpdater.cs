using PedestrianSimulation.Agent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public class ParralelForUpdater : IAgentUpdater<PedestrianAgent>
    {
        public void Initialise(ICollection<PedestrianAgent> agents)
        {
            //No initialisation required
        }

        public void Tick(float timeStep, IEnumerable<PedestrianAgent> agents)
        {
            Parallel.ForEach(agents, (a, s) => a.UpdateIntentions(timeStep));
        }
    }
}
