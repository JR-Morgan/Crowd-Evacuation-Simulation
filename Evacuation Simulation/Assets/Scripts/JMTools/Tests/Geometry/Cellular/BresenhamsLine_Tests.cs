using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace JMTools.Geometry.Cellular.Tests
{
    [TestFixture, TestOf(typeof(BresenhamsLine))]
    public class BresenhamsLine_Tests
    {
        private const int Start = -255, End = 255, C = 5;
        
        /// <summary>
        /// Tests that the optimised <see cref="BresenhamsLine.Intersect"/> method behaves the same as the source implementation that is was based on
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        [Test, Sequential]
        public void MatchesSource(
            [Random(Start,End, C)] int x1,
            [Random(Start,End, C)] int y1,
            [Random(Start,End, C)] int x2,
            [Random(Start,End, C)] int y2)
        {
            Vector2Int point1 = new Vector2Int(x1, y1);
            Vector2Int point2 = new Vector2Int(x2, y2);
            
            var expected = SourceImplementation(point1, point2);
            var actual = BresenhamsLine.Intersect(point1, point2);
            
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        //based on https://stackoverflow.com/a/11683720/13874150 used under https://creativecommons.org/licenses/by-sa/3.0/
        private List<Vector2Int> SourceImplementation(Vector2Int point1, Vector2Int point2)
        {
            List<Vector2Int> intersect = new List<Vector2Int>();
            
            int w = point2.x - point1.x ;
            int h = point2.y - point1.y ;
            
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0 ;
            if (w<0) dx1 = -1 ; else if (w>0) dx1 = 1 ;
            if (h<0) dy1 = -1 ; else if (h>0) dy1 = 1 ;
            if (w<0) dx2 = -1 ; else if (w>0) dx2 = 1 ;
            int longest = Math.Abs(w) ;
            int shortest = Math.Abs(h) ;
            if (!(longest>shortest)) {
                longest = Math.Abs(h) ;
                shortest = Math.Abs(w) ;
                if (h<0) dy2 = -1 ; else if (h>0) dy2 = 1 ;
                dx2 = 0 ;            
            }
            int numerator = longest >> 1 ;
            for (int i=0;i<=longest;i++) {
                intersect.Add(point1);
                numerator += shortest ;
                if (!(numerator<longest)) {
                    numerator -= longest ;
                    point1.x += dx1 ;
                    point1.y += dy1 ;
                } else {
                    point1.x += dx2 ;
                    point1.y += dy2 ;
                }
            }
            return intersect;
        }
    }
}
