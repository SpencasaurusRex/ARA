using ARACore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public struct RobotComponents
    {
        // TODO: This probably wouldn't work if we changed any of these components
        public GameObject obj;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public TileEntity tileEntity;
    }

    public class IdManager
    {
        static Dictionary<ulong, RobotComponents> robotComponents = new Dictionary<ulong, RobotComponents>();
        static ulong currentId;

        public ulong Assign(GameObject obj)
        {
            RobotComponents comp;
            comp.obj = obj;
            comp.meshFilter = obj.GetComponent<MeshFilter>();
            comp.meshRenderer = obj.GetComponent<MeshRenderer>();
            comp.tileEntity = obj.GetComponent<TileEntity>();

            comp.tileEntity.Id = currentId;
            robotComponents[currentId] = comp;
            return currentId++;
        }

        public void Unassign(ulong id)
        {
            robotComponents.Remove(id);
        }

        public RobotComponents Get(ulong id)
        {
            return robotComponents[id];
        }
    }
}