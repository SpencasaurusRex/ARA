using UnityEngine;

namespace Assets.Scripts.UnityComponents
{
    public class MeshColliderInitializer
    {
        public Mesh Mesh;
        public Vector3 Position;

        public MeshColliderInitializer(Mesh mesh, Vector3 position)
        {
            Mesh = mesh;
            Position = position;
        }
    }
}
