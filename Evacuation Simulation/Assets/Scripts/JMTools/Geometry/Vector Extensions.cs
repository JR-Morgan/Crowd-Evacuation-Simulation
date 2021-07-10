using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMTools.Geometry
{
    public static class VectorExtensions
    {
        private const uint DEFAULT_BASE = 10U;
        
        public static Vector3 Round(this Vector3 toRound, int places, uint @base = DEFAULT_BASE)
        {
            var multiplier = Multiplier(places, @base);

            return new Vector3(
                Round(toRound.x, multiplier),
                Round(toRound.y, multiplier),
                Round(toRound.z, multiplier)
                );
        }
        
        public static Vector2 Round(this Vector2 toRound, int places, uint @base = DEFAULT_BASE)
        {
            var multiplier = Multiplier(places, @base);

            return new Vector3(
                Round(toRound.x, multiplier),
                Round(toRound.y, multiplier)
                );
        }

        private static float Multiplier(int places, uint @base)
        {
            float multiplier = 1;
            for (int i = 0; i <= places; i++)
            {
                multiplier *= @base;
            }

            return multiplier;
        }

        private static float Round(float toRound, float multiplier)
        {
            return Mathf.Round(toRound * multiplier) / multiplier;
        }
    }
}
