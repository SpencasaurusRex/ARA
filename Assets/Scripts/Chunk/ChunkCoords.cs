using System;
using UnityEngine;

namespace Assets.Scripts.Chunk
{
    public struct ChunkCoords : IEquatable<ChunkCoords>
    {
        readonly Vector3Int coords;

        public int X => coords.x;
        public int Y => coords.y;
        public int Z => coords.z;

        public ChunkCoords(Vector3Int blockCoords)
        {
            coords = new Vector3Int
            (
                blockCoords.x >> Chunk.ChunkBits,
                blockCoords.y >> Chunk.ChunkBits,
                blockCoords.z >> Chunk.ChunkBits
            );
        }

        public ChunkCoords(int x, int y, int z)
        {
            coords = new Vector3Int(x, y, z);
        }

        public Vector3Int ToBlockCoords() => coords * Chunk.ChunkSize;

        public override int GetHashCode() => coords.GetHashCode();

        public override bool Equals(object obj) => obj is ChunkCoords other && Equals(other);

        public bool Equals(ChunkCoords other) => coords == other.coords;

        public override string ToString()
        {
            return $"ChunkCoords{coords}";
        }
    }
}