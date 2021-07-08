using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace JMTools.Geometry.Cellular.Tests
{
    [TestFixture, TestOf(typeof(CellularCircles2D))]
    public class CellularCircles2D_Test
    {
        private const int XMin = -255, YMin = -255, XMax = 255, YMax = 255;
        private const int RMin = 0, RMax = 16;
        private const int C = 5;
        
        [Test, Sequential]
        public void NoDuplicates(
            [Random(XMin,XMax, C)] int x,
            [Random(YMin,YMax, C)] int y,
            [Random(RMin,RMax, C)] int r
            )
        {
            var indices = CellularCircles2D.IndicesInCircle(x, y, r);
            
            Assert.That(indices, Is.EquivalentTo(indices.Distinct()));

        }
        
    }
}
