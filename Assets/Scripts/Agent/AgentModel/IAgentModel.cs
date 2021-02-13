using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Agent.AgentModel
{

    interface IAgentModel
    {
        public int ID { get; }
        
        public Vector3 Position { get; }
        public Vector3 Velocity { get; }
        public Vector3 CurrentGoal { get; }

        public Vector3 CalculateNextVelocity(IEnumerable<IAgentModel> agents, float deltaTime);
        public void Update(in Vector3 currentVelocity, in Vector3 currentPosition);
    }
}
