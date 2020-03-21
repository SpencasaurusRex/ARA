using Assets.Scripts.Core;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.UnityComponents
{
    public abstract class UnityInitializer
    {
        EntitySet globalSet;
        EntitySet[] operationSets;

        protected UnityInitializer(World world)
        {
            globalSet = world.GetEntities().With<Global>().AsSet();
        }

        public void SetOperationSets(params EntitySet[] operationSets)
        {
            this.operationSets = operationSets;
        }

        public void PollForGameObjects()
        {
            var mapping = globalSet.GetEntities()[0].Get<GameObjectMapping>();

            foreach (var entitySet in operationSets)
            {
                foreach (var entity in entitySet.GetEntities())
                {
                    if (!entity.Has<GameObjectID>())
                    {
                        mapping.AddMapping(entity);
                    }
                }
            }
        }

        // Only can use this in the Update method, after we're sure that the gameObject have been created
        public GameObject GetGameObject(Entity entity)
        {
            var mapping = globalSet.GetEntities()[0].Get<GameObjectMapping>();
            var id = entity.Get<GameObjectID>().ID;
            return mapping.mappings[id];
        }

        public abstract void Update();
    }
}
