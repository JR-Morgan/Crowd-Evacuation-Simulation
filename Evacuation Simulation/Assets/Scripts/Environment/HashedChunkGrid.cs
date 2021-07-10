using System.Collections.Concurrent;
using System.Collections.Generic;
using PedestrianSimulation.Environment;
using UnityEngine;

namespace PedestrianSimulation
{
    public class HashedChunkGrid
    {
        public IReadOnlyDictionary<Vector2Int, Chunk> HashedChunks { get; }

        public HashedChunkGrid()
            : this(new ConcurrentDictionary<Vector2Int, Chunk>())
        { }

        public HashedChunkGrid(ConcurrentDictionary<Vector2Int, Chunk> hashedChunks)
        {
            HashedChunks = hashedChunks;
        }

        public Chunk this[Vector2Int i] => HashedChunks[i];

    }
}
