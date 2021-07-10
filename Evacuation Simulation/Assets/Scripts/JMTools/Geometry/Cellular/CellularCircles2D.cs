using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JMTools.Geometry.Cellular
{
    public static class CellularCircles2D
    {
        
        //Based on implementation by https://stackoverflow.com/a/47042679/13874150 used under https://creativecommons.org/licenses/by-sa/3.0/
        //Adapted to work with Unity vectors
        public static IEnumerable<Vector2Int> IndicesInCircle(Vector2Int center, float radius)
        {
            List<Vector2Int> tmpList = new List<Vector2Int>();
            List<Vector2Int> list = new List<Vector2Int>();
            double rSquared = radius * radius; 
            for (int x = 0; x <= radius; x++)
            for (int y = 0; y <= radius; y++)
            {
                Vector2Int v = new Vector2Int(x, y);
                if (v.sqrMagnitude <= rSquared)
                    tmpList.Add(v);
                else
                    break;
            }

            list.Add(center);

            foreach (Vector2Int v in tmpList)
            {
                Vector2Int vMirr = new Vector2Int(v.x, -1 * v.y);
                list.Add(center + v);
                list.Add(center - v);
                list.Add(center + vMirr);
                list.Add(center - vMirr);
            }


            return list.Distinct();
        }

        public static IEnumerable<Vector2Int> IndicesInCircle(int x, int y, float radius) =>
            IndicesInCircle(new Vector2Int(x, y), radius);
    }
}
