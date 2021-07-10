using System.Collections.Generic;
using System.Linq;
using Results_Core;
using AgentState = PedestrianSimulation.Agent.AgentState;
using ResultsAgentState = Results_Core.AgentState;
using Vector3 = UnityEngine.Vector3;
using ResultsVector3 = Results_Core.Vector3;

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
                => agentStates.Select(a => new TimeData(a.Select(b => b.AsResult()).ToArray())).ToArray();
        }

        
        #region AsResult conversion methods
        private static ResultsAgentState AsResult(this AgentState state)
        {
            return new ResultsAgentState
            {
                id = state.id,
                active = state.active,
                radius = state.radius,
                rotation = state.rotation.eulerAngles.AsResult(),
                desiredSpeed = state.desiredSpeed,
                goal = state.goal.AsResult(),
                position =  state.position.AsResult(),
                velocity = state.velocity.AsResult(),
            };
        }
        
        private static ResultsVector3 AsResult(this Vector3 vector)
        {
            return new ResultsVector3()
            {
                x = vector.x,
                y = vector.y,
                z = vector.z,
            };
        }
        #endregion
    }
}
