using System.Collections.Generic;
using System.Linq;
using PedestrianSimulation.Agent;

namespace PedestrianSimulation.Results
{
    public static class ResultsHelper
    {
        public static SimulationResults GenerateResults(float realTimeToExecute, float timeToEvacuate, IEnumerable<IEnumerable<AgentState>> agentStates)
        {
            return new SimulationResults(
                realTimeToExecute: realTimeToExecute ,
                timeToEvacuate: timeToEvacuate,
                timeData: ProcessTimeData(agentStates));

            static TimeData[] ProcessTimeData(IEnumerable<IEnumerable<AgentState>> agentStates)
                => agentStates.Select(x => new TimeData(x.ToArray())).ToArray();
        }
        
    }
}
