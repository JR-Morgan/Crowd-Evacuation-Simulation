using System;
using UnityEngine;

namespace PedestrianSimulation.Agent
{
    [Serializable]
    public struct AgentInternalState
    {
        public readonly int id;

        public readonly float radius;
        public readonly float desiredSpeed;

        public Vector3 goal, position;
        public Vector3 velocity;

        internal AgentInternalState(int id, float radius, float desiredSpeed, Vector3 goal, Vector3 position, Vector3 velocity)
        {
            this.id = id;
            this.radius = radius;
            this.desiredSpeed = desiredSpeed;
            this.goal = goal;
            this.position = position;
            this.velocity = velocity;
        }
    }
}