using Assets.Scripts.Core;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.UnityComponents
{
    public class GameObjectCreationSystem
    {
        EntitySet globalSet;
        GameObject blankPrefab;
        UnityEngine.Transform entityBase;

        public GameObjectCreationSystem(World world, GameObject blankPrefab, UnityEngine.Transform entityBase)
        {
            globalSet = world.GetEntities().With<Global>().AsSet();
            this.blankPrefab = blankPrefab;
            this.entityBase = entityBase;
        }

        public void Update()
        {
            var mapping = globalSet.GetEntities()[0].Get<GameObjectMapping>();

            foreach (int id in mapping.Creations)
            {
                GameObject go = Object.Instantiate(blankPrefab, entityBase);
                go.name = id.ToString();
                mapping.Add(id, go);
            }

            mapping.Creations.Clear();
        }
    }
}
