using System.Collections;
using System.Collections.Generic;
using PedestrianSimulation.Agent;
using PedestrianSimulation.Agent.LocalAvoidance;
using UnityEngine;

namespace PedestrianSimulation
{
    [System.Obsolete]
    public static class _PreviousSFM
    {
        
        public static Vector3 CalculateNextVelocity(AgentState agent, AgentEnvironmentModel model)
        {
            Vector3 drivingForce = DrivingForce(agent);
            Vector3 interactionForce = InteractionForce(agent, model.Neighbours);
            Vector3 obstacleInteractionForce = ObstacleInteractionForce(agent, model.Walls);

            interactionForce.y = 0; //Assumes all agents are vertically oriented
            obstacleInteractionForce.y = 0; //Assumes all walls are vertical oriented (perhaps a less fair assumption than the latter)
            
            return drivingForce +
                   interactionForce +
                   obstacleInteractionForce;
        }
        
        public static Vector3 DrivingForce(AgentState agent)
        {
            const float relaxationT = 0.54f;
            Vector3 desiredDirection = agent.goal - agent.position;
            desiredDirection.Normalize();

            Vector3 drivingForce = (agent.desiredSpeed * desiredDirection - agent.velocity) / relaxationT;
            return drivingForce;
        }

        public static Vector3 InteractionForce(AgentState agent, IEnumerable<AgentState> neighbours)
        {
            const float lambda = 2.0f;
            const float gamma = 0.35f;
            const float nPrime = 3.0f;
            const float n = 2.0f;
            //const float A = 4.5f;
            const float A = 47f;
            float B, theta;
            int K;
            Vector3 interactionForce = new Vector3(0f, 0f, 0f);

            Vector3 vectorToAgent = new Vector3();

            foreach (AgentState neighbour in neighbours)
            {
                if (agent.id == neighbour.id) continue;

                vectorToAgent = neighbour.position - agent.position;

                // Skip if agent is too far
                if (Vector3.SqrMagnitude(vectorToAgent) > 10f * 10f) continue;

                Vector3 directionToAgent = vectorToAgent.normalized;
                Vector3 interactionVector = lambda * (agent.velocity - neighbour.velocity) + directionToAgent;

                B = gamma * Vector3.Magnitude(interactionVector);

                Vector3 interactionDir = interactionVector.normalized;

                theta = Mathf.Deg2Rad * Vector3.Angle(interactionDir, directionToAgent);

                if (theta == 0) K = 0;
                else if (theta > 0) K = 1;
                else K = -1;

                float distanceToAgent = Vector3.Magnitude(vectorToAgent);

                float deceleration = -A * Mathf.Exp(-distanceToAgent / B
                                                    - (nPrime * B * theta) * (nPrime * B * theta));
                float directionalChange = -A * K * Mathf.Exp(-distanceToAgent / B
                                                             - (n * B * theta) * (n * B * theta));
                Vector3 normalInteractionVector = new Vector3(-interactionDir.z, interactionDir.y, interactionDir.x);
                //Vector3 normalInteractionVector = new Vector3(-interactionDir.y, interactionDir.x, 0);

                interactionForce += deceleration * interactionDir + directionalChange * normalInteractionVector; 
            }
            return interactionForce;
        }

        public static Vector3 ObstacleInteractionForce(AgentState agent, IEnumerable<Wall> walls)
        {
            const float A = 3f;
            const float B = 0.8f;

            float squaredDist = Mathf.Infinity;
            float minSquaredDist = Mathf.Infinity;
            Vector3 minDistVector = new Vector3();

            // Find distance to nearest obstacles
            foreach (Wall w in walls)
            {
                Vector3 vectorToNearestPoint = agent.position - w.GetNearestPoint(agent.position);
                squaredDist = Vector3.SqrMagnitude(vectorToNearestPoint);

                if (squaredDist < minSquaredDist)
                {
                    minSquaredDist = squaredDist;
                    minDistVector = vectorToNearestPoint;
                }
            }

            float distToNearestObs = Mathf.Sqrt(squaredDist) - agent.radius;

            float interactionForce = A * Mathf.Exp(-distToNearestObs / B);

            minDistVector.Normalize();
            minDistVector.y = 0;
            Vector3 obsInteractForce = interactionForce * minDistVector.normalized;

            return obsInteractForce;

        }
    }
}
