using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class Movement
    {
        public Vector3Int Start;
        public Vector3Int Destination;
        public Vector3Int Direction;
        public bool Blocked;
        public Entity Entity;
    }
}
