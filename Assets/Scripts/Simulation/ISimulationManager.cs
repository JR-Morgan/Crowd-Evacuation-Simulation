using PedestrianSimulation.Simulation;
using UnityEngine;

namespace PedestrianSimulation.Simulation
{
    public interface ISimulationManager
    {
        bool RunSimulation(SimulationSettings settings, GameObject environment);
        bool CancelSimulation();
    }
}