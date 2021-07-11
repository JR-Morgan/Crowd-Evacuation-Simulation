
using System;

namespace Results_Core
{
    [Serializable]
    public struct SimulationResults
    {
        /// <summary> The time in seconds that it took to run the simulation (in real time units)</summary>
        public float realTimeToExecute;

        /// <summary>The time in seconds that it took for all agents to evacuate (in simulation time)</summary>
        public float timeToEvacuate;
        
        public TimeData[] timeData;

        public int numberOfAgents;

        public SimulationResults(float realTimeToExecute, float timeToEvacuate, TimeData[] timeData, int numberOfAgents)
        {
            this.realTimeToExecute = realTimeToExecute;
            this.timeToEvacuate = timeToEvacuate;
            this.timeData = timeData;
            this.numberOfAgents = numberOfAgents;
        }
    }

}