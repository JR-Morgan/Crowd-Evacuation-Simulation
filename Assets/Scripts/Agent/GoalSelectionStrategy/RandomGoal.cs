using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Agent.GoalSelectionStrategy
{
    [Serializable]
    class RandomGoal : NewGoalOnAchievement
    {
        [SerializeField]
        private float lowerX = 0f, upperX = 0.12f, lowerZ = 0f, upperZ = 0.12f;

        private const float y = 0f;

        protected override Vector3 NextGoal(Vector3 lastGoal)
        {
            return new Vector3(Random.Range(lowerX, upperX), y, Random.Range(lowerZ, upperZ));
        }
    }
}
