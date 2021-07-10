namespace Results_Core
{
    public struct AgentState
    {
        public int id;
        public bool active;

        public float radius;
        public float desiredSpeed;
        
        public Vector3 goal, position, rotation, velocity;
    }
}