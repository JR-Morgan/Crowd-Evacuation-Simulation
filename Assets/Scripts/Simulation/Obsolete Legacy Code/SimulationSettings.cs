using PedestrianSimulation.Agent;
using PedestrianSimulation.Agent.LocalAvoidance;
using PedestrianSimulation.Agent.LocalAvoidance.SFM;
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
        public float timeStep = 1f / 30f;
        public bool useLegacyAgents = false;


        #region Distribution Strategy
        public DistributionStrategy agentDistribution;
        public IAgentDistribution<T> NewDistribution<T>() where T : AbstractAgent => NewDistribution<T>(agentDistribution);
        public static IAgentDistribution<T> NewDistribution<T>(DistributionStrategy strategy) where T : AbstractAgent
        {
            return strategy switch
            {
                DistributionStrategy.Uniform => new UniformAgentDistribution<T>(),
                _ => throw new StrategyNotImplementedException<DistributionStrategy>(strategy),
            };
        }
        #endregion

        #region Update Strategy
        public UpdateStrategy updateStrategy;
        public IAgentUpdater<T> NewUpdater<T>() where T : AbstractAgent => NewUpdater<T>(updateStrategy);
        public static IAgentUpdater<T> NewUpdater<T>(UpdateStrategy strategy) where T : AbstractAgent
        {
            return strategy switch
            {
                UpdateStrategy.Task => new AsyncTaskUpdater<T>(),
                UpdateStrategy.Parallel => new ParralelForUpdater<T>(),
                UpdateStrategy.Synchronous => new SynchronousUpdater<T>(),
                _ => throw new StrategyNotImplementedException<UpdateStrategy>(strategy),
            };
        }
        #endregion

        #region Update Strategy
        public LocalAvoidanceStrategy localAvoidanceStrategy;
        public ILocalAvoidance NewLocalAvoidance() => NewLocalAvoidance(localAvoidanceStrategy);
        public static ILocalAvoidance NewLocalAvoidance(LocalAvoidanceStrategy strategy)
        {
            return strategy switch
            {
                LocalAvoidanceStrategy.SFM => new DelegatedLocalAvoidance(SocialForceModel.CalculateNextVelocity),
                _ => throw new StrategyNotImplementedException<LocalAvoidanceStrategy>(strategy),
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

    public enum LocalAvoidanceStrategy
    {
        SFM,
    }

    public class StrategyNotImplementedException<T> : NotImplementedException
    {
        public StrategyNotImplementedException(T strategy)
            : base($"No {typeof(LocalAvoidanceStrategy)} implementation for {typeof(LocalAvoidanceStrategy)}.{strategy}")
        { }
    }
}