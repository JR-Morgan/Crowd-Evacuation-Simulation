using System;

namespace Results_Core
{
    [Serializable]
    public struct TimeData
    {
        public AgentState[] agentStates;
        
        public TimeData(AgentState[] agentStates)
        {
            this.agentStates = agentStates;
        }
    }
}