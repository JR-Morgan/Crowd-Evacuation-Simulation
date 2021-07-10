using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedestrianSimulation.Simulation.UpdateStrategies
{
    public interface IAgentUpdater<T>
    {
        void Initialise(ICollection<T> agents);

        void Tick(float timeStep, IEnumerable<T> agents);
    }
}
