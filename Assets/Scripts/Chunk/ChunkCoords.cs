using System;
using UnityEngine;

namespace Assets.Scripts.Chunk
{
    public struct ChunkCoords : IEquatable<ChunkCoords>
    {
        readonly Vector3Int coords;

        public ChunkCoords(Vector3Int coords)
        {
            this.coords = coords;
        }

        public static ChunkCoords FromBlockCoords(Vector3Int block)
        {
            var chunkCoords = new Vector3Int
            (
                block.x << Chunk.ChunkBits,
                block.y << Chunk.ChunkBits,
                block.z << Chunk.ChunkBits
            );
            return new ChunkCoords(chunkCoords);
        }

        public override int GetHashCode() => coords.GetHashCode();

        public override bool Equals(object obj) => obj is ChunkCoords other && Equals(other);

        public bool Equals(ChunkCoords other) => coords == other.coords;
    }
}