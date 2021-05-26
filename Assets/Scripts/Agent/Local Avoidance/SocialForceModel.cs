using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSimulation.Agent.Behaviour.LocalAvoidance
{
    public static class SocialForceModel
    {
        public static Vector3 DrivingForce(in AgentInternalState agent)
        {
            const float relaxationT = 0.54f; //Value based on Moussaid et al., 2009 (agrees with trinhthanhtrung)

            Vector3 desiredDirection = Vector3.Normalize(agent.goal - agent.position);

            Vector3 drivingForce = ((agent.desiredSpeed * desiredDirection) - agent.velocity) / relaxationT;
            return drivingForce;
        }

        public static Vector3 InteractionForce(in AgentInternalState agent, in IEnumerable<AgentInternalState> neighbours)
        {
            const float observableRadiusSquared = 10f * 10f;
            const float lambda = 2f;
            const float gamma = 0.35f;
            const float nPrime = 3f;
            const float n = 2f;
            const float A = 4.5f; // (47.0 trinhthanhtrung) (4.5 Moussaid)

            float B, theta;
            int K;

            Vector3 interactionForce = Vector3.zero;


            foreach (AgentInternalState neighbour in neighbours)
            {
                if (agent.id == neighbour.id) continue;

                //continue if agent is too far (this might be replaced later with a hash grid system)
                Vector3 translationToNeighbour = agent.position - neighbour.position;
                if (translationToNeighbour.sqrMagnitude > observableRadiusSquared) continue;

                Vector3 directionToNeighbour = Vector3.Normalize(translationToNeighbour);
                Vector3 interactionVector = lambda * (agent.velocity - neighbour.velocity) + directionToNeighbour;

                B = gamma * interactionVector.magnitude;

                Vector3 interactionDirection = Vector3.Normalize(interactionVector);

                theta = Vector3.Angle(interactionDirection, directionToNeighbour);


                K = (int)Mathf.Sign(theta);

                float distanceToNeighbour = translationToNeighbour.magnitude;
                float deceleration = -A * Mathf.Exp(-distanceToNeighbour / B - (nPrime * B * theta) * (nPrime * B * theta));
                float directionalChange = -A * K * Mathf.Exp(-distanceToNeighbour / B - (n * B * theta) * (n * B * theta));

                Vector3 normalInteractionVector = new Vector3(-interactionDirection.z, interactionDirection.y, interactionDirection.x);

                interactionForce += deceleration * interactionDirection + directionalChange * normalInteractionVector;
            }

            return interactionForce;
        }
    }
}