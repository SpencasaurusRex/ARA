using UnityEngine;

namespace Assets.Scripts.Movement
{
    public struct MovementRequest
    {
        public readonly Vector3Int From;
        public readonly Vector3Int To;

        public MovementRequest(Vector3Int from, Vector3Int to)
        {
            From = from;
            To = to;
        }
    }
}
