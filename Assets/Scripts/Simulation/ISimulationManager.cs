using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSimulation.Simulation
{
    public interface ISimulationManager<T> : ISimulationManager where T : AbstractAgent
    {
        IList<T> Agents { get; }
    }

    public interface ISimulationManager
    { 
        bool RunSimulation(SimulationSettings settings, GameObject environment);
        bool CancelSimulation();

    }
}