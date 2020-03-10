using DefaultEcs;

namespace Assets.Scripts.Core
{
    public class MovementUpdateSystem : IUpdateSystem
    {
        World world;
        public MovementUpdateSystem(World world)
        {
            this.world = world;
        }

        public void Update(float fractional)
        {

        }

        public void AdvanceTick()
        {
            
        }
    }
}
