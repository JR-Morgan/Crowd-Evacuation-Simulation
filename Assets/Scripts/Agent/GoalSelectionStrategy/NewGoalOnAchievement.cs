using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Agent.GoalSelectionStrategy
{
    abstract class NewGoalOnAchievement : IGoalSelection
    {
		private const float radius = 0.01f;

		private Vector3 goal;

        public Vector3 CurrentGoal(Vector3 currentPosition)
        {
			Vector3 goalDelta = goal - currentPosition;
			
			if (goalDelta.sqrMagnitude < radius * radius)
			{
				goal = NextGoal(goal);
			}

			return goal;
		}

		protected abstract Vector3 NextGoal(Vector3 lastGoal);
    }
}
