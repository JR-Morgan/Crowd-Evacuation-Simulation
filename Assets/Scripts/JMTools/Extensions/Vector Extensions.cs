using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMTools
{
    public static class VectorExtensions
    {
        public static Vector3 Round(this Vector3 toRound, int places, uint @base = 10)
        {
            float multiplier = 1;
            for (int i = 0; i <= places; i++)
            {
                multiplier *= @base;
            }

            return new Vector3(
                Mathf.Round(toRound.x * multiplier) / multiplier,
                Mathf.Round(toRound.y * multiplier) / multiplier,
                Mathf.Round(toRound.z * multiplier) / multiplier);
        }
    }
}
