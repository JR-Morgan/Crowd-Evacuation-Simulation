using System;
using UnityEngine;

namespace PedestrianSimulation.Agent
{
    [Serializable]
    public struct AgentState
    {
        public readonly int id;
        public bool active;

        public readonly float radius;
        public readonly float desiredSpeed;

        public Vector3 goal, position;
        public Quaternion rotation;
        public Vector3 velocity;

        public float time;

        internal AgentState(int id, bool active, float radius, float desiredSpeed, Vector3 goal, Vector3 position, Quaternion rotation, Vector3 velocity, float time = -1f)
        {
            this.id = id;
            this.active = active;
            this.radius = radius;
            this.desiredSpeed = desiredSpeed;
            this.goal = goal;
            this.position = position;
            this.rotation = rotation;
            this.velocity = velocity;
            this.time = time;
        }
    }
}