using System;
using UnityEngine;

namespace PedestrianSimulation.Agent
{
    [Serializable]
#if UNITY_EDITOR
    public struct AgentInternalState
    {
        public int id;

        public float desiredSpeed;
        public Vector3 goal;

        public Vector3 position;
        public Vector3 velocity;
#else
    public readonly struct AgentInternalState
    {
        public readonly int id;

        public readonly float desiredSpeed;
        public readonly Vector3 goal;

        public readonly Vector3 position;
        public readonly Vector3 velocity;
#endif
        internal AgentInternalState(int id, float desiredSpeed, Vector3 goal, Vector3 position, Vector3 velocity)
        {
            this.id = id;
            this.desiredSpeed = desiredSpeed;
            this.goal = goal;
            this.position = position;
            this.velocity = velocity;
        }
    }
}