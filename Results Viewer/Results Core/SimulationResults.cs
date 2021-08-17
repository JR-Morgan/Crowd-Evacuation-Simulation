
using System;

namespace Results_Core
{
    [Serializable]
    public struct SimulationResults
    {
        /// <summary> The time in seconds that it took to run the simulation (in real time units)</summary>
        public float realTimeToExecute;

        /// <summary>The total time in seconds that it took for all agents to evacuate (in simulation time)</summary>
        public float timeToEvacuate;
        
        /// <summary>The mean time in seconds that it took for agents to evacuate (in simulation time)</summary>
        public float meanTimeToEvacuate;
        
        public TimeData[] timeData;

        public int numberOfAgents;

        public SimulationResults(float realTimeToExecute, float timeToEvacuate, float meanTimeToEvacuate, TimeData[] timeData, int numberOfAgents)
        {
            this.realTimeToExecute = realTimeToExecute;
            this.timeToEvacuate = timeToEvacuate;
            this.timeData = timeData;
            this.meanTimeToEvacuate = meanTimeToEvacuate;
            this.numberOfAgents = numberOfAgents;
        }

        public override string ToString()
        {
            return //$"Run ID: {id}." +
                   $"Time to execute: \t\t{realTimeToExecute:F}s\n" +
                   $"Time to fully evacuate: \t{timeToEvacuate:F}s\n" +
                   $"Mean time to evacuate: \t{meanTimeToEvacuate:F}s\n";
        }
    }

}