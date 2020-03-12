using System.Reflection;
using ARACore;
using Assets.Scripts.Movement;
using Assets.Scripts.Rendering;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class Game : MonoBehaviour
    {
        public Material mat;
        
        World world;
        UpdateManager updateManager;
        RenderingSystem renderingSystem;

        void Start()
        {
            world = new World();

            updateManager = new UpdateManager
            (
                new TileEntityUpdateSystem(),
                new ScriptUpdateSystem(),
                new MovementUpdateSystem(world)
            );

            renderingSystem = new RenderingSystem(world);

            var testMesh = world.CreateEntity();

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var meshFilter = go.GetComponent<MeshFilter>();

            testMesh.Set(meshFilter.mesh);
            testMesh.Set(mat);

            Destroy(go);
        }

        void Update()
        {
            updateManager.Update(Time.deltaTime);
            renderingSystem.Update();
        }
    }
}
