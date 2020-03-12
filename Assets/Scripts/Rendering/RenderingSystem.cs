using System.Collections.Generic;
using ARACore;
using Assets.Scripts.Transform;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Rendering
{
    public class RenderingSystem
    {
        World world;
        EntitySet meshSet;

        #if UNITY_EDITOR
        public List<GizmoRender> GizmoRenderList = new List<GizmoRender>();

        public class GizmoRender
        {
            public Mesh Mesh;
            public Vector3 Translation;
            public Quaternion Quaternion;
            public Vector3 Scale;
        }
        #endif

        public RenderingSystem(World world)
        {
            this.world = world;
            meshSet = world.GetEntities().With<Mesh>().With<Material>().With<LocalToWorld>().AsSet();
        }

        public void Update()
        {
            #if UNITY_EDITOR
            GizmoRenderList.Clear();
            #endif
            foreach (var entity in meshSet.GetEntities())
            {
                var mesh = entity.Get<Mesh>();
                var material = entity.Get<Material>();
                var transform = entity.Get<LocalToWorld>().Matrix;

                Vector3 scale = Vector3.one;
                if (entity.Has<Scale>()) scale = entity.Get<Scale>().Value;
                Quaternion rotation = Quaternion.identity;
                if (entity.Has<Rotation>()) rotation = entity.Get<Rotation>().Value;

                Graphics.DrawMesh(mesh, transform, material, 0, Camera.main);
                
                #if UNITY_EDITOR
                GizmoRenderList.Add(new GizmoRender { Mesh = mesh, Translation = transform.Translation(), Quaternion = rotation, Scale = scale});
                #endif
            }
        }
    }
}
