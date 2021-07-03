using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using JMTools;
using JMTools.Geometry.Cellular;
using PedestrianSimulation.Agent.LocalAvoidance;
using PedestrianSimulation.Simulation;
using UnityEngine;
using UnityEngine.AI;

namespace PedestrianSimulation.Environment
{

    public readonly struct Chunk
    {
        public readonly List<Wall> walls;

        public Chunk(List<Wall> walls)
        {
            this.walls = walls;
        }
        
        public Chunk(Wall wall)
        {
            this.walls = new List<Wall>() { wall };
        }
    }
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshSurface))]
    public class EnvironmentManager : MonoBehaviour
    {

        private HashedChunkGrid chunkGrid;
        
        private NavMeshSurface navMeshSurface;
        public NavMeshTriangulation navMeshTriangulation { get; private set; }

        private void Awake()
        {
            chunkGrid = new HashedChunkGrid();
            navMeshSurface = GetComponent<NavMeshSurface>();
        }

        public void InitialiseEnvironment()
        {
            Transform[] children = this.GetComponentsInChildren<Transform>(true);

            int layerNumber = this.gameObject.layer;
            Debug.Assert(navMeshSurface.layerMask.ContainsLayer(layerNumber), $"Expected {typeof(EnvironmentManager)}'s layer to be included in {typeof(NavMeshSurface)}.{nameof(NavMeshSurface.layerMask)} but was not. Ensure {gameObject}'s layer is set!", this);
            
            foreach (Transform t in children)
            {
                t.gameObject.layer = layerNumber;
            }
            
            navMeshSurface.BuildNavMesh();
            
            navMeshTriangulation = NavMesh.CalculateTriangulation();
            
            chunkGrid = new HashedChunkGrid(GenerateChunks(navMeshTriangulation));
        }

        private static ConcurrentDictionary<int, Chunk> GenerateChunks(NavMeshTriangulation navMeshTriangulation)
        {
            ConcurrentDictionary<int, Chunk> dict = new ConcurrentDictionary<int, Chunk>();
            
            IList<Wall> walls = NavmeshProcessor.GetNavmeshBoundaryEdges(navMeshTriangulation);
            
            //Add walls
            foreach(Wall wall in walls)
            {
                var chunkIndices = BresenhamsLine.Intersect(
                    IndexFunction(wall.StartPoint),
                    IndexFunction(wall.EndPoint)
                    );

                foreach (var point in chunkIndices)
                {
                    AddWall(wall, HashFunction(point));
                }
            }
            
            return dict;

            void AddWall(Wall wall, int hash)
            {
                if (!dict.TryAdd(hash, new Chunk(wall)))
                {
                    dict[hash].walls.Add(wall);
                }
            }
        }


        public bool TryGetChunk(int hash, out Chunk chunk) => chunkGrid.HashedChunks.TryGetValue(hash, out chunk);
        public bool TryGetChunk(Vector3 position, out Chunk chunk) => TryGetChunk(HashFunction(position), out chunk);


        public static Vector2Int IndexFunction(Vector3 position) => new Vector2Int((int) position.x, (int) position.z); //TODO for now just cast to int, consider using a rounding function for variable chunk size
        private static int HashFunction(int x, int y) => (((x + y) * (x + y + 1) / 2) + y);
        private static int HashFunction(Vector2Int xy) => HashFunction(xy.x, xy.y);
        public static int HashFunction(Vector3 position) => HashFunction(IndexFunction(position));
    }
}
