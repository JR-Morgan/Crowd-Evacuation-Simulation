using DOTS_AI.Components;
using DOTS_AI.Extensions;
using Unity.Collections;
using Unity.Mathematics;

namespace DOTS_AI
{
    public static class SocialForceModel
    {
        public static float3 DrivingForce(in AgentComponent agent)
        {
            const float relaxationT = 0.54f; //Value based on Moussaid et al., 2009 (agrees with trinhthanhtrung)

            float3 desiredDirection = math.normalize(agent.goal - agent.translation);

            float3 drivingForce = ((agent.desiredSpeed * desiredDirection) - agent.velocity) / relaxationT;
            return drivingForce;
        }

        public static float3 InteractionForce(in AgentComponent agent, in NativeArray<AgentComponent> neighbours)
        {
            const float observableRadiusSquared = 10f * 10f;
            const float lambda = 2f;
            const float gamma = 0.35f;
            const float nPrime = 3f;
            const float n = 2f;
            const float A = 4.5f; // (47.0 trinhthanhtrung) (4.5 Moussaid)

            float B, theta;
            int K;

            float3 interactionForce = float3.zero;


            foreach (AgentComponent neighbour in neighbours)
            {
                if (agent.id == neighbour.id) continue;

                //continue if agent is too far (this might be replaced later with a hash grid system)
                float3 translationToNeighbour = agent.translation - neighbour.translation;
                if (math.lengthsq(translationToNeighbour) > observableRadiusSquared) continue;

                float3 directionToNeighbour = math.normalize(translationToNeighbour);
                float3 interactionVector = lambda * (agent.velocity - neighbour.velocity) + directionToNeighbour;

                B = gamma * math.length(interactionVector);

                float3 interactionDirection = math.normalize(interactionVector);

                theta = MathE.Angle(interactionDirection, directionToNeighbour);


                K = (int)math.sign(theta);

                float distanceToNeighbour = math.length(translationToNeighbour);
                float deceleration = -A * math.exp(-distanceToNeighbour / B - (nPrime * B * theta) * (nPrime * B * theta));
                float directionalChange = -A * K * math.exp(-distanceToNeighbour / B - (n * B * theta) * (n * B * theta));

                float3 normalInteractionVector = new float3(-interactionDirection.z, interactionDirection.y, interactionDirection.x);

                interactionForce += deceleration * interactionDirection + directionalChange * normalInteractionVector;
            }

            return interactionForce;
        }
    }
}
