using UnityEngine;

namespace Assets.Scripts.Chunk
{
    public enum CardinalHeading
    {
        North,
        East,
        South,
        West
    }

    public static class CardinalHeadingUtil
    {
        public static float ToDegrees(this CardinalHeading heading)
        {
            return (360f - 90 * (int) heading) % 360f;
        }

        public static CardinalHeading Clockwise(this CardinalHeading heading)
        {
            return (CardinalHeading) (((int) heading + 1) % 4);
        }

        public static CardinalHeading CounterClockwise(this CardinalHeading heading)
        {
            int newHeading = (int) heading - 1;
            if (newHeading < 0) newHeading = (int)CardinalHeading.West;
            return (CardinalHeading) newHeading;
        }

        public static Quaternion ToQuaternion(this CardinalHeading heading) =>
            Quaternion.AngleAxis(-heading.ToDegrees(), Vector3.up);
    }
}
