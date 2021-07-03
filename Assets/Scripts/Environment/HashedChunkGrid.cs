using System.Collections.Concurrent;
using System.Collections.Generic;
using PedestrianSimulation.Environment;

namespace PedestrianSimulation
{
    public class HashedChunkGrid
    {
        public IReadOnlyDictionary<int, Chunk> HashedChunks { get; }

        public HashedChunkGrid()
            : this(new ConcurrentDictionary<int, Chunk>())
        { }

        public HashedChunkGrid(ConcurrentDictionary<int, Chunk> hashedChunks)
        {
            HashedChunks = hashedChunks;
        }

        public Chunk this[int i] => HashedChunks[i];

    }
}
