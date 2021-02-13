using Assets.Scripts.Agent.GoalSelectionStrategy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Agent.AgentModel
{
	[Serializable]
	class AgentSFM : IAgentModel
	{

		[SerializeField]
		private float desiredSpeed = 1.29f; //(float)speedDistrabution.Sample();
		[SerializeField]
		private float drivingWeight = 1, interactionWeight = 0.06f, obsticalWeight = 0;
		[SerializeField]
		private RandomGoal goalSelection = new RandomGoal();

		private static int idCounter = 0;
		public int ID { get; }
		public Vector3 CurrentGoal => goalSelection.CurrentGoal(Position);
		public Vector3 Position { get; private set; }
		public Vector3 Velocity { get; private set; }

		//private static readonly Normal speedDistrabution = new (1.29f, 0.19f);

		public AgentSFM()
        {
			ID = idCounter++;
		}
		public void Update(in Vector3 currentVelocity, in Vector3 currentPosition)
        {
			Velocity = currentVelocity;
			Position = currentPosition;
		}


        public Vector3 CalculateNextVelocity(IEnumerable<IAgentModel> agents, float deltaTime)
        {
			void Check(Vector3 vector, object message) {
				if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z)) Debug.Log(this.ID.ToString() + " " + message + " was NaN");
			}

			Vector3 drivingForce = (GoalDrivingForce(CurrentGoal, Position, Velocity, desiredSpeed) * drivingWeight);
			Vector3 interactionForce = (AgentAvoidenceForce(agents, this) * interactionWeight);

			Check(drivingForce, nameof(drivingForce));
			Check(interactionForce, nameof(interactionForce));

			Vector3 acceleration = drivingForce + interactionForce;

			Check(acceleration, nameof(acceleration));




			Vector3 desiredVelocity = (Velocity + acceleration) * deltaTime;
			Check(desiredVelocity, nameof(desiredVelocity));

			if (desiredVelocity.sqrMagnitude > (desiredSpeed * desiredSpeed))
            {
                desiredVelocity.Normalize();
                desiredVelocity *= desiredSpeed;
				Check(desiredVelocity, nameof(desiredVelocity) +" AF");
			}

			return desiredVelocity;
        }

        private static Vector3 GoalDrivingForce(in Vector3 goal, in Vector3 currentPosition, in Vector3 currentVelocity, float desiredSpeed)
        {
            const float T = 0.54F;  // Relaxation time based on in secconds (Moussaid et al., 2009)
            Vector3 e_i, f_i;

            // Compute Desired Direction
            // Formula: e_i = (position_target - position_i) / ||(position_target - position_i)||
            e_i = goal - currentPosition;
			e_i.Normalize();

            // Compute Driving Force
            // Formula: f_i = ((desiredSpeed * e_i) - velocity_i) / T
            f_i = ((desiredSpeed * e_i) - currentVelocity) * (1 / T);

			return f_i;
        }

		private static Vector3 AgentAvoidenceForce(IEnumerable<IAgentModel> agents, IAgentModel self)
		{
			// Constant Values Based on (Moussaid et al., 2009)
			const float lambda = 2f;   // Weight reflecting relative importance of velocity vector against position vector
			const float gamma = 0.35f;  // Speed interaction
			const float n_prime = 3f;  // Angular interaction
			const float n = 2f;        // Angular intaraction
			const float A = 4f;        // Modal parameter A

			Vector3 distance_ij, e_ij, D_ij, t_ij, n_ij, f_ij;
			float B, theta, f_v, f_theta;
			int K;

			f_ij = new Vector3(0f, 0f, 0f);

			foreach (IAgentModel agent_j in agents) {
				// Do Not Compute Interaction Force to Itself
				if (agent_j.ID != self.ID)
				{
					// Compute Distance Between Agent j and i
					distance_ij = agent_j.Position - self.Position;

					// Skip Computation if Agents i and j are Too Far Away
					if (distance_ij.sqrMagnitude > (2f * 2f))
						continue;

					// Compute Direction of Agent j from i
					// Formula: e_ij = (position_j - position_i) / ||position_j - position_i||
					e_ij = distance_ij;
					e_ij.Normalize();

					// Compute Interaction Vector Between Agent i and j
					// Formula: D = lambda * (velocity_i - velocity_j) + e_ij
					D_ij = lambda * (self.Velocity - agent_j.Velocity) + e_ij;

					// Compute Modal Parameter B
					// Formula: B = gamma * ||D_ij||
					B = gamma * D_ij.magnitude;

					// Compute Interaction Direction
					// Formula: t_ij = D_ij / ||D_ij||
					t_ij = D_ij;
					t_ij.Normalize();

					// Compute Angle Between Interaction Direction (t_ij) and Vector Pointing from Agent i to j (e_ij)
					//theta = t_ij.angle(e_ij);
					theta = (float)(Math.Acos(Vector3.Dot(t_ij, e_ij)) * (180 / Math.PI));



					// Compute Sign of Angle 'theta'
					// Formula: K = theta / |theta|
					K = (theta == 0) ? 0 : (int)(theta / Math.Abs(theta));

					// Compute Amount of Deceleration
					// Formula: f_v = -A * exp(-distance_ij / B - ((n_prime * B * theta) * (n_prime * B * theta)))
					f_v = (float)(-A * Math.Exp(-distance_ij.magnitude / B - ((n_prime * B * theta) * (n_prime * B * theta))));

					// Compute Amount of Directional Changes
					// Formula: f_theta = -A * K * exp(-distance_ij / B - ((n * B * theta) * (n * B * theta)))
					f_theta = (float)(-A * K * Math.Exp(-distance_ij.magnitude / B - ((n * B * theta) * (n * B * theta))));

					// Compute Normal Vector of Interaction Direction Oriented to the Left
					n_ij = new Vector3(-t_ij.z, 0f, t_ij.x);

					// Compute Interaction Force
					// Formula: f_ij = f_v * t_ij + f_theta * n_ij
					f_ij += (f_v * t_ij) + (f_theta * n_ij);
				}
			}
			if(float.IsNaN(f_ij.x)) f_ij.x = 0;
			if (float.IsNaN(f_ij.y)) f_ij.y = 0;
			if (float.IsNaN(f_ij.z)) f_ij.z = 0;
			return f_ij;
		}

        private static Vector3 ObsticalAvoidenceForce()
        {
            throw new NotImplementedException();
        }
    }
}
