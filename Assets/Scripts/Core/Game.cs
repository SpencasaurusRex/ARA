using ARACore;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class Game : MonoBehaviour
    {
        UpdateManager updateManager;
        World world;

        void Init()
        {
            world = new World();

            updateManager = new UpdateManager
            (
                new TileEntityUpdateSystem(),
                new ScriptUpdateSystem(),
                new MovementUpdateSystem(world)
            );
        }
    }
}
