using PedestrianSimulation.Agent.LocalAvoidance;
using System.Collections.Generic;

namespace PedestrianSimulation.Agent
{
    public class AgentEnvironmentModel
    {
        public IList<AgentState> Neighbours { get; set; }
        public IList<Wall> Walls { get; }

        public AgentEnvironmentModel(IList<AgentState> neighbours, IList<Wall> walls)
        {
            Neighbours = neighbours;
            Walls = walls;
        }

        public AgentEnvironmentModel(IList<Wall> walls)
            : this(new List<AgentState>(), walls)
        { }
    }
}
