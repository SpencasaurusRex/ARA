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
            CenterCoords = new Vector3
            (
                (coords.x + 0.5f) * Chunk.ChunkSize,
                (coords.y + 0.5f) * Chunk.ChunkSize,
                (coords.z + 0.5f) * Chunk.ChunkSize
            );
        }

        public ChunkCoords(int x, int y, int z)
        {
            coords = new Vector3Int(x, y, z);
            CenterCoords = new Vector3
            (
                (coords.x + 0.5f) * Chunk.ChunkSize,
                (coords.y + 0.5f) * Chunk.ChunkSize,
                (coords.z + 0.5f) * Chunk.ChunkSize
            );
        }

        public Vector3 CenterCoords { get; }

        public Vector3Int ToBlockCoords() => coords * Chunk.ChunkSize;

        public ChunkCoords Offset(int dx, int dy, int dz) => new ChunkCoords(coords.x + dx, coords.y + dy, coords.z + dz);

        public override int GetHashCode() => coords.GetHashCode();

        public override bool Equals(object obj) => obj is ChunkCoords other && Equals(other);

        public bool Equals(ChunkCoords other) => coords == other.coords;

        public override string ToString()
        {
            return $"ChunkCoords{coords}";
        }
    }
}