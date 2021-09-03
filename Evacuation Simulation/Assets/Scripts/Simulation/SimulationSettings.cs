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
        
        public Transform[] goals;
        
        //public float timeStep = 1f / 30f;

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
        public UpdateStrategy updateStrategy = UpdateStrategy.Parallel;
        public IAgentUpdater NewUpdater() => NewUpdater(updateStrategy);
        public static IAgentUpdater NewUpdater(UpdateStrategy strategy)
        {
            return strategy switch
            {
                UpdateStrategy.Task => new AsyncTaskUpdater(),
                UpdateStrategy.Parallel => new ParallelForUpdater(),
                UpdateStrategy.Synchronous => new SynchronousUpdater(),
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
                _ => new DelegatedLocalAvoidance(SocialForceModel.CalculateNextVelocity), //For now, always return SFM. RVO doesn't use ILocalAvoidance anyway so will ignore it on initialisation
                //LocalAvoidanceStrategy.SFM => new DelegatedLocalAvoidance(SocialForceModel.CalculateNextVelocity),
                //_ => throw new StrategyNotImplementedException<LocalAvoidanceStrategy>(strategy),
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
        RVO,
        SFM,
    }

    public class StrategyNotImplementedException<T> : NotImplementedException
    {
        public StrategyNotImplementedException(T strategy)
            : base($"No {typeof(LocalAvoidanceStrategy)} implementation for {typeof(LocalAvoidanceStrategy)}.{strategy}")
        { }
    }
}