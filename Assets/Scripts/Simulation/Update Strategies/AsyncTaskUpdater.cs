using PedestrianSimulation.Agent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public class AsyncTaskUpdater : IAgentUpdater<PedestrianAgent>
    {
        private List<Task> updateTasks;
        private float timeStep;

        public void Initialise(ICollection<PedestrianAgent> agents)
        {
            updateTasks = new List<Task>(agents.Count);
            AddRange(agents);
        }

        public void Add(PedestrianAgent agent)
        {
            updateTasks.Add(Task.Run(() => agent.UpdateIntentions(timeStep)));
        }

        public void AddRange(IEnumerable<PedestrianAgent> agents)
        {
            foreach (var a in agents)
            {
                Add(a);
            }
        }

        public void Tick(float timeStep, IEnumerable<PedestrianAgent> _ = null)
        {
            Task t = TickAsync(timeStep);
            //t.Start();
            t.Wait();
        }

        private async Task TickAsync(float timeStep)
        {
            this.timeStep = timeStep;
            await Task.WhenAll(updateTasks);
        }
    }
}