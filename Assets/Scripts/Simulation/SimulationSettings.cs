using PedestrianSimulation.Agent;
using PedestrianSimulation.Agent.Behaviour.LocalAvoidance;
using PedestrianSimulation.Simulation.Initialisation;
using PedestrianSimulation.Simulation.UpdateStrategies;
using System;
using UnityEngine;

namespace PedestrianSimulation.Simulation
{
    //TODO can't be a struct in order to do reflection in SimulationSetupElement, need this to be a class
    [Serializable]
    public class SimulationSettings
    {
        public int seed = 255;
        public int numberOfAgents = 100;
        public Transform goal;


        public IAgentDistribution<PedestrianAgent> agentDistribution = new UniformAgentDistribution<PedestrianAgent>(); //TODO

        #region Update Strategy
        public UpdateStrategy updateStrategy;
        public IAgentUpdater<PedestrianAgent> NewUpdater() => NewUpdater(updateStrategy);
        public static IAgentUpdater<PedestrianAgent> NewUpdater(UpdateStrategy updateStrategy)
        {
            return updateStrategy switch
            {
                UpdateStrategy.Task => new AsyncTaskUpdater(),
                UpdateStrategy.Parallel => new ParralelForUpdater(),
                UpdateStrategy.Synchronous => new SynchronousUpdater(),
                _ => throw new NotImplementedException($"No {nameof(UpdateStrategy)} implementation for {nameof(UpdateStrategy)}.{updateStrategy}"),
            };
        }
        #endregion
    }


    public enum UpdateStrategy
    {
        Task,
        Parallel,
        Synchronous
    }
}