using System.Collections.Generic;
using ARACore;
using Assets.Scripts.Chunk;
using Assets.Scripts.Core;
using Assets.Scripts.TempMovement;
using Assets.Scripts.Transform;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Robots
{
    public class RobotInitSystem
    {
        EntitySet robotInitSet;
        Entity globalEntity;
        Mesh robotMesh;
        Material robotMaterial;
        IntDispenser robotIds;

        public RobotInitSystem(World world, Mesh robotMesh, Material robotMaterial)
        {
            robotInitSet = world.GetEntities().With<RobotInit>().AsSet();
            globalEntity = world.GetGlobalEntity();

            var mapping = new Dictionary<ID, Entity>();
            globalEntity.Set(mapping);

            this.robotMesh = robotMesh;
            this.robotMaterial = robotMaterial;

            robotIds = new IntDispenser(0);
        }

        public void Update()
        {
            var chunkSet = globalEntity.Get<ChunkSet>();
            var mapping = globalEntity.Get<Dictionary<ID, Entity>>();

            foreach (var entity in robotInitSet.GetEntities())
            {
                var initInfo = entity.Get<RobotInit>();
                var position = initInfo.Position;
                var heading = initInfo.Heading;

                if (chunkSet.GetBlock(position) == Block.Air)
                {
                    var id = new ID {Value = robotIds.GetFreeInt()};

                    entity.Set(robotMesh);
                    entity.Set(robotMaterial);
                    entity.Set(new LocalToWorld());
                    entity.Set(new Translation { Value = position });
                    entity.Set(new Rotation{ Value = heading.ToQuaternion() });
                    entity.Set(new GridPosition { Value = position });
                    entity.Set(new Scale { Value = new Vector3(.9f, .9f, .9f)});
                    entity.Set(CardinalHeading.North);
                    entity.Set(id);

                    mapping[id] = entity;
                }

                entity.Remove<RobotInit>();
            }
        }
    }
}
