using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PedestrianSimulation.Simulation.Tests
{
    [TestFixture, TestOf(typeof(NavmeshProcessor))]
    public class NavmeshProcessor_Tests
    {
        
        [Test]
        public void GetBoundaryEdges_Triangle()
        {
            const int n = 3; //Number of vertices in triangle
            
            IList<int> triangle = Enumerable.Range(0, n).ToList();
            var boundaryEdges = NavmeshProcessor.GetBoundaryEdges(triangle).ToArray();
            
            Assert.That(boundaryEdges.Length == n);
            
            Assert.Contains(new Vector2Int(0, 1), boundaryEdges);
            Assert.Contains(new Vector2Int(0, 2), boundaryEdges);
            Assert.Contains(new Vector2Int(1, 2), boundaryEdges);
        }
        
        [Test]
        public void GetBoundaryEdges_Quad()
        {
            const int n = 3; //Number of vertices in triangle
            const int q = 4; //Number of vertices in quad
            
            IList<int> t1 = Enumerable.Range(0, n).ToList();
            IList<int> t2 = Enumerable.Range(1, n).ToList();
            
            List<int> quad = new List<int>();
            quad.AddRange(t1);
            quad.AddRange(t2);
            
            var boundaryEdges = NavmeshProcessor.GetBoundaryEdges(quad).ToArray();
            
            Assert.That(boundaryEdges.Length == q);
            
            Assert.Contains(new Vector2Int(0, 1) ,boundaryEdges);
            Assert.Contains(new Vector2Int(0, 2) ,boundaryEdges);
            Assert.That(boundaryEdges, Has.No.Member(new Vector2Int(1, 2)));
            Assert.Contains(new Vector2Int(1, 3) ,boundaryEdges);
            Assert.Contains(new Vector2Int(2, 3) ,boundaryEdges);
        }
        
    }
}
