using System.Collections.Generic;
using Assets.Scripts.Core;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.UnityComponents
{
    public class GameObjectMapping
    {
        public List<int> Creations;
        public Dictionary<int, GameObject> mappings;
        IntDispenser idDispenser;

        public GameObjectMapping()
        {
            mappings = new Dictionary<int, GameObject>();
            Creations = new List<int>();
            idDispenser = new IntDispenser(0);
        }

        public void AddMapping(Entity entity)
        {
            int id = idDispenser.GetFreeInt();
            entity.Set(new GameObjectID(id));
            Creations.Add(id);
        }

        public void RemoveMapping(Entity entity)
        {
            int id = entity.Get<GameObjectID>().ID;

            entity.Remove<GameObjectID>();
            idDispenser.ReleaseInt(id);
        }

        public void Add(int id, GameObject go)
        {
            mappings[id] = go;
        }
    }
}
