using PedestrianSimulation.Agent.LocalAvoidance;
using System.Collections.Generic;
using JetBrains.Annotations;
#nullable enable
namespace PedestrianSimulation.Agent
{
    public class AgentEnvironmentModel
    {
        public List<AgentState> Neighbours { get; }
        public HashSet<Wall> Walls { get; }

        public AgentEnvironmentModel(HashSet<Wall>? walls = default)
        {
            Neighbours = new List<AgentState>();
            
            
           walls ??= new HashSet<Wall>();
           Walls = walls;
        }

    }
}
