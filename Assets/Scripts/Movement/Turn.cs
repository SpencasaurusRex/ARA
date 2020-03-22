using Assets.Scripts.Chunk;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class Turn
    {
        public CardinalHeading From;
        public CardinalHeading To;

        public Quaternion FromQuaternion;
        public Quaternion ToQuaternion;

        public Turn(CardinalHeading from, CardinalHeading to)
        {
            From = from;
            To = to;

            FromQuaternion = from.ToQuaternion();
            ToQuaternion = to.ToQuaternion();
        }
    }
}
