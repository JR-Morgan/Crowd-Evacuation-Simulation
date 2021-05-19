using System;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTS_AI.Components
{
    [Serializable]
    public struct AgentComponent : IComponentData
    {
        public int id;
        public float desiredSpeed;

        public float3 velocity;

        public float3 goal;

        public float3 translation;

    }
}
