using UnityEngine;

namespace Assets.Scripts.Agent.GoalSelectionStrategy
{
    interface IGoalSelection
    {
        Vector3 CurrentGoal(Vector3 currentPosition);
    }
}
