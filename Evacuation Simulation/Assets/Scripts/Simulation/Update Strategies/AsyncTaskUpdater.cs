using PedestrianSimulation.Agent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public class AsyncTaskUpdater<T> : IAgentUpdater<T> where T : AbstractAgent
    {
        private List<Task> updateTasks;
        private float timeStep;

        public void Initialise(ICollection<T> agents)
        {
            updateTasks = new List<Task>(agents.Count);
            AddRange(agents);
        }

        public void Add(T agent)
        {
            updateTasks.Add(Task.Run(() => agent.UpdateIntentions(timeStep)));
        }

        public void AddRange(IEnumerable<T> agents)
        {
            foreach (var a in agents)
            {
                Add(a);
            }
        }

        public void Tick(float timeStep, IEnumerable<T> _ = null)
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