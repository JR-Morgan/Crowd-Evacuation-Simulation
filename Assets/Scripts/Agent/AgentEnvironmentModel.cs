using PedestrianSimulation.Agent.LocalAvoidance;
using System.Collections.Generic;

namespace PedestrianSimulation.Agent
{
    public class AgentEnvironmentModel
    {
        public List<AgentState> Neighbours { get; }
        public List<Wall> Walls { get; }

        public AgentEnvironmentModel(List<AgentState> neighbours, List<Wall> walls)
        {
            Neighbours = neighbours;
            Walls = walls;
        }

        public AgentEnvironmentModel(List<Wall> walls)
            : this(new List<AgentState>(), walls)
        { }
        
        
    }
}
