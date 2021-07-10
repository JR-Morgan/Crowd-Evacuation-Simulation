using PedestrianSimulation.Agent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public class ParralelForUpdater<T> : IAgentUpdater<T> where T : AbstractAgent
    {
        public void Initialise(ICollection<T> agents)
        {
            //No initialisation required
        }

        public void Tick(float timeStep, IEnumerable<T> agents)
        {
            Parallel.ForEach(agents, (a, s) => a.UpdateIntentions(timeStep));
        }
    }
}
