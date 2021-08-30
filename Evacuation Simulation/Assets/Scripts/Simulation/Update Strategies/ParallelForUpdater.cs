using PedestrianSimulation.Agent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public class ParallelForUpdater : IAgentUpdater
    {
        public void Tick(float timeStep, IEnumerable<IAgent> agents)
        {
            timeStep *= Time.deltaTime;
            Parallel.ForEach(agents, (a, s) => a.UpdateIntentions(timeStep));
        }
    }
}
