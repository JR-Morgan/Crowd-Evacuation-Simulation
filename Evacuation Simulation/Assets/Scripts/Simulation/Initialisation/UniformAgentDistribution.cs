#nullable enable
using PedestrianSimulation.Agent;
using System.Collections.Generic;
using JMTools;
using UnityEngine;
using UnityEngine.AI;

namespace PedestrianSimulation.Simulation.Initialisation
{
    public class UniformAgentDistribution<T> : IAgentDistribution<T> where T : AbstractAgent
    {
        protected const int DEFAULT_TRIES = 100;
        protected const float DEFAULT_DISTANCE = 1f;

        #region Instance Members
        public int Tries { get; }
        public float Distance { get; }

        public UniformAgentDistribution(int tries = DEFAULT_TRIES, float distance = DEFAULT_DISTANCE)
        {
            Tries = tries;
            Distance = distance;
        }

        public List<T> InstantiateAgents(Transform agentParent, Transform agentsGoal, GameObject agentPrefab, int numberOfAgents, GameObject environmentModel)
        {
            return InstantiateAgents(agentParent, agentsGoal, agentPrefab, numberOfAgents, environmentModel, this.Tries, this.Distance);
        }
        #endregion

        #region Static Members
        /// <summary>
        /// Creates the specified <paramref name="numberOfAgents"/> as children of <paramref name="agentParent"/>.
        /// </summary>
        /// <param name="agentParent">The parent transform of all of the new agents</param>
        /// <param name="goal">The agents goal</param>
        /// <param name="agentPrefab">The prefab used to create agents</param>
        /// <param name="numberOfAgents">The number of agents that should be created</param>
        /// <param name="environmentModel">The agent's environment, used to calculate bounds</param>
        /// <param name="tries">The number of times to try and instantiate an agent in the environment, Larger environments may require a larger value</param>
        /// <param name="distance">The size of voxels used to spawn agents, Should be less than or equal to the size of agents.
        ///     The smaller the more uniform the agent's distribution but the higher chance of a single try failing</param>
        /// <returns>The list of new agents. May contain null values if some agents failed to spawn within the number of <paramref name="tries"/></returns>
        public static List<T> InstantiateAgents(Transform agentParent, Transform goal, GameObject agentPrefab, int numberOfAgents, GameObject environmentModel, int tries = DEFAULT_TRIES, float distance = DEFAULT_DISTANCE)
        {
            List<T> agents = new List<T>(numberOfAgents);
            Bounds bounds =  environmentModel.CalculateRendererBounds();

            for (int i = 0; i < numberOfAgents; i++)
            {
                GameObject agentGameObject = Object.Instantiate(agentPrefab, agentParent);
                NavMeshAgent? navAgent = agentGameObject.GetComponent<NavMeshAgent>();

                if (!agentGameObject.TryGetComponent(out T agent)) agent = agentGameObject.AddComponent<T>();
 
                if (agentGameObject.GetComponentDependants<NavMeshAgent>().Count == 0)
                {
                    navAgent.DestroyApplicationSafe();
                }

                bool failed = true;
                for (int j = 0; j < tries; j++)
                {
                    Vector3 position = GetRandomPointOnNavMesh(bounds, environmentModel.transform, distance);
                    
                    if (position.Equals(Vector3.positiveInfinity)) continue;
                    
                    if (navAgent != null) navAgent.Warp(position); //TODO consider doing this check in agent
                    else agentGameObject.transform.position = position;

                    if (agent.TrySetGoal(goal.position))
                    {
                        failed = false;
                        break;
                    }

                }

                if (failed)
                {
                    agentGameObject.DestroyApplicationSafe();

                    Debug.LogWarning($"Failed to instantiate agent in a valid location after {tries} tries.");
                }
                else
                {
                    agents.Add(agent);
                }

            }
            return agents;
        }

        protected static Vector3 GetRandomPointOnNavMesh(in Bounds bounds, Transform transform, float distance)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            randomPoint = transform.TransformPoint(randomPoint);

            //randomPoint += Random.insideUnitSphere * distance;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, distance, NavMesh.AllAreas))
            {
                return hit.position + Vector3.up;
            }

            return Vector3.positiveInfinity;
        }
        #endregion
    }
}