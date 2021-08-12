using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public interface IAgentUpdater<in T>
    {
        void Tick(float timeStep, IEnumerable<T> agents);
    }
}
