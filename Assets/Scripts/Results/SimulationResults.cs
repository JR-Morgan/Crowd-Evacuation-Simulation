using System.Collections.Generic;
using PedestrianSimulation.Agent;

namespace PedestrianSimulation.Results
{
    public readonly struct SimulationResults
    {
        /// <summary> The time in seconds that it took to run the simulation (in real time units)</summary>
        public readonly float realTimeToExecute;

        /// <summary>The time in seconds that it took for all agents to evacuate (in simulation time)</summary>
        public readonly float timeToEvacuate;
        
        public readonly TimeData[] timeData;

        public SimulationResults(float realTimeToExecute, float timeToEvacuate, TimeData[] timeData)
        {
            this.realTimeToExecute = realTimeToExecute;
            this.timeToEvacuate = timeToEvacuate;
            this.timeData = timeData;
        }
    }

    public readonly struct TimeData
    {
        public readonly AgentState[] agentStates;

        public readonly float meanAvoidanceForce;

        public TimeData(AgentState[] agentStates)
        {
            this.agentStates = agentStates;
            this.meanAvoidanceForce = CalculateMeanAvoidanceForce(agentStates);
        }

        private static float CalculateMeanAvoidanceForce(IEnumerable<AgentState> agentStates)
        {
            //TODO
            return 0;
        }
    }
}
