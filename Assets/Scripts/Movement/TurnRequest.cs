using Assets.Scripts.Chunk;

namespace Assets.Scripts.Movement
{
    public class TurnRequest
    {
        public CardinalHeading From;
        public CardinalHeading To;

        public TurnRequest(CardinalHeading from, CardinalHeading to)
        {
            From = from;
            To = to;
        }
    }
}
