using Assets.Scripts.Core;
using DefaultEcs;

namespace Assets.Scripts.Movement
{
    public class TurnRequestSystem : IUpdateSystem
    {
        EntitySet turnRequestSet;
        EntitySet doneTurningSet;

        public TurnRequestSystem(World world)
        {
            turnRequestSet = world.GetEntities().With<TurnRequest>().AsSet();
            doneTurningSet = world.GetEntities().With<Turn>().AsSet();
        }

        public void Update(float fractional)
        {
        }

        public void EndTick()
        {
            foreach (var entity in doneTurningSet.GetEntities())
            {
                var turn = entity.Get<Turn>();
                entity.Set(turn.To);
                entity.Remove<Turn>();
            }

            foreach (var entity in turnRequestSet.GetEntities())
            {
                var request = entity.Get<TurnRequest>();

                entity.Set(new ActionResult { Result = true });
                entity.Set(new Turn(request.From, request.To));

                entity.Remove<TurnRequest>();
            }
        }
    }
}
