using System.Collections.Generic;
using System.Threading.Tasks;
using PedestrianSimulation.Agent;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public interface IAgentUpdater
    {
        void Tick(float timeStep, IEnumerable<IAgent> agents);
    }
}
