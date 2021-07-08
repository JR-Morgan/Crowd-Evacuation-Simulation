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
            this.walls = new List<Wall>() {wall};
        }
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshSurface))]
    public class EnvironmentManager : Singleton<EnvironmentManager>
    {

        private HashedChunkGrid chunkGrid;

        private NavMeshSurface navMeshSurface;
        public NavMeshTriangulation navMeshTriangulation { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            chunkGrid = new HashedChunkGrid();
            navMeshSurface = GetComponent<NavMeshSurface>();
        }

        public void InitialiseEnvironment()
        {
            Transform[] children = this.GetComponentsInChildren<Transform>(true);

            int layerNumber = this.gameObject.layer;
            Debug.Assert(navMeshSurface.layerMask.ContainsLayer(layerNumber),
                $"Expected {typeof(EnvironmentManager)}'s layer to be included in {typeof(NavMeshSurface)}.{nameof(NavMeshSurface.layerMask)} but was not. Ensure {gameObject}'s layer is set!",
                this);

            foreach (Transform t in children)
            {
                t.gameObject.layer = layerNumber;
            }

            navMeshSurface.BuildNavMesh();

            navMeshTriangulation = NavMesh.CalculateTriangulation();

            chunkGrid = new HashedChunkGrid(GenerateChunks(navMeshTriangulation));
        }

        private static ConcurrentDictionary<Vector2Int, Chunk> GenerateChunks(NavMeshTriangulation navMeshTriangulation)
        {
            var dict = new ConcurrentDictionary<Vector2Int, Chunk>();

            IList<Wall> walls = NavmeshProcessor.GetNavmeshBoundaryEdges(navMeshTriangulation);

            //Add walls
            foreach (Wall wall in walls)
            {
                var chunkIndices = BresenhamsLine.Intersect(
                    IndexFunction(wall.StartPoint),
                    IndexFunction(wall.EndPoint)
                );

                foreach (var point in chunkIndices)
                {
                    AddWall(wall, point);
                }
            }
            
            #if UNITY_EDITOR
            var parent = new GameObject("DEBUG Chunks");
            foreach (var c in dict)
            {
                var go = new GameObject($"Chunk {c.Key}");
                go.transform.parent = parent.transform;
                var vis = go.AddComponent<ChunkVisualiser>();
                vis.Initialise(c.Value, c.Key);
            }
            #endif
            
            return dict;

            void AddWall(Wall wall, Vector2Int key)
            {
                if (!dict.TryAdd(key, new Chunk(wall)))
                {
                    dict[key].walls.Add(wall);
                }
            }
        }
        
        public static bool TryGetChunk(Vector2Int key, out Chunk chunk) => Instance.chunkGrid.HashedChunks.TryGetValue(key, out chunk);
        public static bool TryGetChunk(Vector3 position, out Chunk chunk) => TryGetChunk(IndexFunction(position), out chunk);
        
        public static Vector2Int IndexFunction(Vector3 position) =>
            new Vector2Int((int) position.x, (int) position.z); //TODO for now just cast to int, consider using a rounding function for variable chunk size
        
        public static Vector3 InverseIndexFunction(Vector2Int index) =>
            new Vector3(index.x + 0.5f, 0, index.y + 0.5f); 
    }
}
