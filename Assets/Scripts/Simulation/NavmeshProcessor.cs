using System;
using PedestrianSimulation.Agent.LocalAvoidance;
using System.Collections.Generic;
using System.Linq;
using JMTools;
using UnityEngine;
using UnityEngine.AI;

namespace PedestrianSimulation.Simulation
{
    public static class NavmeshProcessor
    {
        public static List<Wall> GetNavmeshBoundaryEdges(NavMeshTriangulation navMeshTriangulation)
        {
            IEnumerable<(Vector3,Vector3)> boundaryEdges = GetBoundaryEdges(navMeshTriangulation.indices, navMeshTriangulation.vertices);
            
            List<Wall> walls = new List<Wall>();
            foreach((Vector3 a,Vector3 b) in boundaryEdges)
            {
                walls.Add(new Wall(a, b));
            }
            return walls;
        }
        
        public static IEnumerable<(Vector3,Vector3)> GetBoundaryEdges(IList<int> indices, IList<Vector3> vertices)
        {
            const int n = 3; //number of verts in triangle

            var singleEdges = new Dictionary<(Vector3,Vector3), int>();

            for(int t = 0; t < indices.Count / n; t++)
            {
                int index = t * n;

                int i_1 = indices[index++];
                int i_2 = indices[index++];
                int i_3 = indices[index++];

                CheckVisited(i_1, i_2);
                CheckVisited(i_1, i_3);
                CheckVisited(i_2, i_3);
            }
            
            return singleEdges.Keys.Where(x => singleEdges[x] == 1);
            
            void CheckVisited(int a, int b)
            {
                Vector3 aPos = vertices[a];//.Round(4);
                Vector3 bPos = vertices[b];//.Round(4);

                var edge = aPos.GetHashCode() < bPos.GetHashCode() ? (aPos, bPos) : (bPos, aPos);
                
                singleEdges.TryGetValue(edge, out int occurence);
                singleEdges[edge] = occurence + 1;
            }
        }
        
    }
}
