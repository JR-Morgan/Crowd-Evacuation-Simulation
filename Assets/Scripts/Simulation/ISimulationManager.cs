using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

        UnityEvent OnSimulationStart { get; }
        UnityEvent OnSimulationStop { get; }

    }
}