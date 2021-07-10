using System;
using System.Collections.Generic;

namespace Results_Core
{
    [Serializable]
    public struct TimeData
    {
        public AgentState[] agentStates;

        public float meanAvoidanceForce;

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