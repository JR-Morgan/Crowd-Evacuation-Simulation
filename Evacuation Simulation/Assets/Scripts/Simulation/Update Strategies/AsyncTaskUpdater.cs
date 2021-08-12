using PedestrianSimulation.Agent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public class AsyncTaskUpdater : IAgentUpdater
    {
        

        public void Tick(float timeStep, IEnumerable<IAgent> agents)
        {
            IEnumerable<Task> tasks = agents.Select(a => Task.Run(() => a.UpdateIntentions(timeStep)));

            Task.WaitAll(tasks.ToArray());
        }
    }
}