using PedestrianSimulation.Agent;
using System.Collections.Generic;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public class SynchronousUpdater : IAgentUpdater

    {
    public void Tick(float timeStep, IEnumerable<IAgent> agents)
    {
        foreach (var agent in agents)
        {
            agent.UpdateIntentions(timeStep);
        }
    }
    }
}
