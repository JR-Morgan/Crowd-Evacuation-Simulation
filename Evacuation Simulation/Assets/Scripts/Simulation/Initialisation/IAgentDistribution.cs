using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSimulation.Simulation.Initialisation
{
    
    /// <summary>
    /// Abstraction of a distribution function that operates on an environment to distribute and initialise agents within 
    /// </summary>
    public interface IAgentDistribution<T> where T : Component
    {
        /// <summary>
        /// Attempts to instantiate <paramref name="numberOfAgents"/> <typeparamref name="T"/> as children of <paramref name="agentParent"/>.
        /// </summary>
        /// <param name="agentParent">The parent transform of all of the new agents</param>
        /// <param name="agentsGoals">Terminal goal locations (i.e. evacuation points)</param>
        /// <param name="agentPrefab">The prefab used to create agents</param>
        /// <param name="numberOfAgents">The number of agents that should be instantiated</param>
        /// <param name="environmentModel">The agent's environment, used to calculate bounds</param>
        /// <returns>The list of new agents of size <paramref name="numberOfAgents"/> - May contain <c>null</c> values if some agents failed to be instantiated</returns>
        List<T> InstantiateAgents(Transform agentParent, ICollection<Transform> agentsGoals, GameObject agentPrefab, int numberOfAgents, GameObject environmentModel);
    }
}
