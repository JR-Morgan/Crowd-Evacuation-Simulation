using PedestrianSimulation.Agent;
using PedestrianSimulation.Agent.Behaviour.LocalAvoidance;
using PedestrianSimulation.Simulation.Initialisation;
using PedestrianSimulation.Simulation.UpdateStrategies;
using System;
using UnityEngine;

namespace PedestrianSimulation.Simulation
{
    //TODO can't be a struct in order to do reflection in SimulationSetupElement, need this to be a reference type
    [Serializable]
    public class SimulationSettings
    {
        public int seed = 255;
        public int numberOfAgents = 100;
        public Transform goal;


        #region Distribution Strategy
        public DistributionStrategy agentDistribution;
        public IAgentDistribution<T> NewDistribution<T>() where T : AbstractAgent => NewDistribution<T>(agentDistribution) ;
        public static IAgentDistribution<T> NewDistribution<T>(DistributionStrategy agentDistribution) where T : AbstractAgent
        {
            return agentDistribution switch
            {
                DistributionStrategy.Uniform => new UniformAgentDistribution<T>(),
                _ => throw new NotImplementedException($"No {typeof(DistributionStrategy)} implementation for {typeof(DistributionStrategy)}.{agentDistribution}"),
            };
        }
        #endregion

        #region Update Strategy
        public UpdateStrategy updateStrategy;
        public IAgentUpdater<T> NewUpdater<T>() where T : AbstractAgent => NewUpdater<T>(updateStrategy);
        public static IAgentUpdater<T> NewUpdater<T>(UpdateStrategy updateStrategy) where T : AbstractAgent
        {
            return updateStrategy switch
            {
                UpdateStrategy.Task => new AsyncTaskUpdater<T>(),
                UpdateStrategy.Parallel => new ParralelForUpdater<T>(),
                UpdateStrategy.Synchronous => new SynchronousUpdater<T>(),
                _ => throw new NotImplementedException($"No {typeof(UpdateStrategy)} implementation for {typeof(UpdateStrategy)}.{updateStrategy}"),
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

    public enum DistributionStrategy
    {
        Uniform,
    }
}