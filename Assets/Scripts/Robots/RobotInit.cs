using Assets.Scripts.Chunk;
using UnityEngine;

namespace Assets.Scripts.Robots
{
    public class RobotInit
    {
        public Vector3Int Position;
        public CardinalHeading Heading;

        public RobotInit(Vector3Int position, CardinalHeading heading = CardinalHeading.North)
        {
            Position = position;
            Heading = heading;
        }
    }
}
