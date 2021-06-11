using PedestrianSimulation.Agent.LocalAvoidance;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PedestrianSimulation
{
    public static class NavmeshProcessor
    {
        public static List<Wall> GetNavmeshBoundaryEdges(NavMeshTriangulation navMeshTriangulation)
        {
            IEnumerable<Vector2Int> boundaryEdges = GetBoundaryEdges(navMeshTriangulation.indices);
            List<Wall> walls = new List<Wall>();

            foreach(Vector2Int edge in boundaryEdges)
            {
                Vector3 v1 = navMeshTriangulation.vertices[edge.x];
                Vector3 v2 = navMeshTriangulation.vertices[edge.y];
                walls.Add(new Wall(v1, v2));
            }
            return walls;
        }

        public static HashSet<Vector2Int> GetBoundaryEdges(IList<int> indecies)
        {
            const int n = 3; //number of verts in triangle

            var singleEdges = new HashSet<Vector2Int>();

            for(int t = 0; t < indecies.Count / 3; t++)
            {
                int index = t * n;

                int i_1 = indecies[index++];
                int i_2 = indecies[index++];
                int i_3 = indecies[index++];

                CheckVisited(new Vector2Int(i_1, i_2));
                CheckVisited(new Vector2Int(i_1, i_3));
                CheckVisited(new Vector2Int(i_2, i_3));
            }

            return singleEdges;


            void CheckVisited(Vector2Int edge)
            {
                //We assume that an edge is at most, shared between two triangles
                if (!singleEdges.Remove(edge))
                {
                    singleEdges.Add(edge);
                }
            }

        }

    }
}
