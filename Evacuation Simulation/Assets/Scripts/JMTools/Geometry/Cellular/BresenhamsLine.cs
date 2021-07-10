using System;
using System.Collections.Generic;
using UnityEngine;

namespace JMTools.Geometry.Cellular
{
    //Implementation of this algorithm is based on the following external source,
    //and has been adapted to use Unity vector structures and optimised for performance
    //based on https://stackoverflow.com/a/11683720/13874150 used under https://creativecommons.org/licenses/by-sa/3.0/
    public static class BresenhamsLine
    {
        
        public static List<Vector2Int> Intersect(Vector2Int point1, Vector2Int point2)
        {
            List<Vector2Int> intersect = new List<Vector2Int>();
            
            int w = point2.x - point1.x ;
            int h = point2.y - point1.y ;

            int wa = Math.Abs(w), wh = Math.Abs(h);
            int longest = Mathf.Max(wa, wh);
            int shortest = Mathf.Min(wa, wh);
            
            int dx1 = Math.Sign(w);
            int dy1 = Math.Sign(h);
            int dx2, dy2;

            if (wa <= wh)
            {
                dx2 = 0;
                dy2 = dy1;
            }
            else
            {
                dx2 = dx1;
                dy2 = 0;
            }

            int numerator = longest >> 1 ;

            for (int i=0; i <= longest; i++)
            {
                intersect.Add(point1);
                
                numerator += shortest ;
                
                if (numerator >= longest)
                {
                    numerator -= longest ;
                    point1.x += dx1 ;
                    point1.y += dy1 ;
                }
                else
                {
                    point1.x += dx2 ;
                    point1.y += dy2 ;
                }
            }

            return intersect;
        }
        
        
        
    }
}
