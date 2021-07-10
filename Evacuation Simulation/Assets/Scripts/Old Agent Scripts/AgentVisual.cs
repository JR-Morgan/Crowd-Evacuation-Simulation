using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PedestrianSimulation.Agent
{
    [System.Obsolete]
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentVisual : MonoBehaviour
    {

        private NavMeshAgent navMeshAgent;
        private Renderer _renderer;

        [SerializeField]
        private Color startColor = Color.green, endColor = Color.red;
        [SerializeField]
        float max = 0.4f, pivot = 0.25f, proportion = 0.8f;
        float min;

        [SerializeField]
        float delta;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            _renderer = GetComponentInChildren<Renderer>(true);

        }
        private void OnValidate()
        {
            min = CalculateMin(max, pivot, proportion);
        }

        private void Update()
        {
            delta = navMeshAgent.desiredVelocity.magnitude - navMeshAgent.velocity.magnitude;
            _renderer.material.color = CalculateColour(delta);
        }



        /// <param name="max">The max value</param>
        /// <param name="pivot">The pivot value</param>
        /// <param name="proportion">The proportion between <paramref name="pivot"/> and <paramref name="max"/></param>
        private static float CalculateMin(float max, float pivot, float proportion) => ((max - pivot) / proportion) * (1 - proportion);


        private Color CalculateColour(float magnitude)
        {
            float lerp = 0 + ((1 - 0) / (max - min)) * (magnitude - min);

            return Color.LerpUnclamped(startColor, endColor, lerp);
        }
    }
}