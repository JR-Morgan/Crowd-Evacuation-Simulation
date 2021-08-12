using PedestrianSimulation.Agent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public class AsyncTaskUpdater<T> : IAgentUpdater<T> where T : IAgent
    {
        
        public void Tick(float timeStep, IEnumerable<T> agents)
        {
            Task t = TickAsync(timeStep, agents);
            t.Start();
            t.Wait();
        }

        private static async Task TickAsync(float timeStep, IEnumerable<T> agents)
        {
            IEnumerable<Task> tasks = agents.Select(a => Task.Run(() => a.UpdateIntentions(timeStep)));

            await Task.WhenAll(tasks);
        }
    }
}