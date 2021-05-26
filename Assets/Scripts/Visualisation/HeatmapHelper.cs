using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSimulation.Visualisation
{

    public static class HeatmapHelper
    {
        /// <summary>Maximum array size that can be passed to a shader through <see cref="Material.SetVectorArray"/></summary>
        public const int MAX_ARRAY_SIZE = 1023;

        /// <summary>
        /// Generates an array of length <paramref name="arraySize"/> of homogeneous coordinates positions of the given <paramref name="agents"/>.<br/>
        /// If the size of <paramref name="agents"/> exceeds <paramref name="arraySize"/>, only the first <paramref name="arraySize"/> will be used.<br/>
        /// If the size of <paramref name="agents"/> is less than <paramref name="arraySize"/>, <c>default</c> values will be used for the remaining elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="agents">Desired list of <typeparamref name="T"/></param>
        /// <param name="arraySize">The size of the array returned</param>
        /// <returns>Array of homogenous coordinate positions</returns>
        public static Vector4[] GeneratePositionArray<T>(IList<T> agents, int arraySize = MAX_ARRAY_SIZE) where T : Component
        {

            Vector4[] positions = new Vector4[arraySize];

            int upperBound = Mathf.Min(agents.Count, arraySize);
            for (int i = 0; i < upperBound; i++)
            {
                positions[i] = ToHomogeneousCoordinates(agents[i].transform.position);
            }

            return positions;
        }

        public static Vector4 ToHomogeneousCoordinates(Vector3 pos) => new Vector4(pos.x, pos.y, pos.z, 1f);
    }
}
