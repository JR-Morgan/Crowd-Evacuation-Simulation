using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PedestrianSimulation.Simulation.Tests
{
    [TestFixture, TestOf(typeof(NavmeshProcessor))]
    public class NavmeshProcessor_Tests
    {
        private const int t = 3; //Number of vertices in triangle (to avoid magic numbers)
        private const int q = 4; //Number of vertices in quad
        
        private struct MockMesh
        {
            public List<int> indices;
            public List<Vector3> vertices;
        }

        private static MockMesh Quad(int offset = 0)
        {
            List<int> indices = new List<int>();
            indices.AddRange(Enumerable.Range(q * offset, t));
            indices.AddRange(Enumerable.Range(q * offset + 1, t));
            
            return new MockMesh()
            {
                indices = indices,
                vertices = new List<Vector3>()
                {
                    new Vector3(offset,     0),
                    new Vector3(offset + 1, 0),
                    new Vector3(offset,     1),
                    new Vector3(offset + 1, 1),
                },
            };
        }
        private static MockMesh Polygon(int numberOfQuads)
        {
            MockMesh mesh = new MockMesh {indices = new List<int>(), vertices = new List<Vector3>()};
            for (int i = 0; i < numberOfQuads; i++)
            {
                MockMesh quad = Quad(i);
                mesh.indices.AddRange(quad.indices);
                mesh.vertices.AddRange(quad.vertices);
            }
            return mesh;
        }
        

        [Test]
        public void GetBoundaryEdges_Polygon([NUnit.Framework.Range(1,5)] int numberOfQuads)
        {
            MockMesh mesh = Polygon(numberOfQuads);
            var boundaryEdges = NavmeshProcessor.GetBoundaryEdges(mesh.indices, mesh.vertices).ToArray();
            
            for (int i = 0; i < numberOfQuads; i++)
            {
                var v0 = new Vector3(i,     0);
                var v1 = new Vector3(i + 1, 0);
                var v2 = new Vector3(i,     1);
                var v3 = new Vector3(i + 1, 1);
                
                //Assert Contains top and bottom edge
                AssertContainsOne(boundaryEdges, true, (v0, v1), (v1, v0));
                AssertContainsOne(boundaryEdges, true, (v2, v3), (v3, v2));
                
                //Assert Doesn't contain diagonal edge
                AssertContainsOne(boundaryEdges, false, (v1, v2), (v2, v1));
                
                //Assert If beginning -> contains left edge
                AssertContainsOne(boundaryEdges, i == 0, (v0,v2), (v2, v0));
                //Assert If end -> contains right edge
                AssertContainsOne(boundaryEdges, i + 1 == numberOfQuads, (v1,v3), (v3, v1));
            }
            
            //Assert Is of expected length
            Assert.AreEqual(2 * numberOfQuads + 2, boundaryEdges.Length);
            
        }
        
        private static void AssertContainsOne<T>(IEnumerable<T> collection, bool shouldContain, params T[] values)
        {
            Assert.AreEqual(shouldContain ? 1 : 0, collection.Intersect(values).Count(),
                message: $"Expected collection to {Negative()} contain a value from {FormatCollection(values)}\n but was {FormatCollection(collection)}");
            
            string Negative() => shouldContain ? "" : "not";
            static string FormatCollection(IEnumerable<T> collection) => $"{{{string.Join(", ", collection.ToArray())}}}";
        }
        
    }
}
