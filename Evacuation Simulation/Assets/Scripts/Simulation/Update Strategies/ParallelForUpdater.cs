using PedestrianSimulation.Agent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public class ParallelForUpdater<T> : IAgentUpdater<T> where T : IAgent
    {
        public void Tick(float timeStep, IEnumerable<T> agents)
        {
            Parallel.ForEach(agents, (a, s) => a.UpdateIntentions(timeStep));
        }
    }
}
