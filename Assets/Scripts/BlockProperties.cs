using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ARACore
{
    class BlockProperties
    {
        static Properties[] blockProperties = new Properties[Enum.GetNames(typeof(BlockType)).Length];
        static Dictionary<string, int> textureIndices = new Dictionary<string, int>();
        static int currentUVTile;

        public static Properties Get(BlockType type)
        {
            return blockProperties[(int)type];
        }

        public struct Properties
        {
            public bool generateMesh;
            public bool transparent;
            public int[] uvTileIndex;
        }

        public static void ReadJson()
        {
            string json = System.IO.File.ReadAllText(@"F:\Dev\Projects\ARA\Assets\Config\blockProperties.json");
            var blockInfo = JsonConvert.DeserializeObject<Dictionary<string, JsonBlockProperties>>(json);

            foreach (var typeName in Enum.GetNames(typeof(BlockType)))
            {
                var lowerCaseType = typeName.Substring(0, 1).ToLower() + typeName.Substring(1, typeName.Length - 1);
                JsonBlockProperties jsonProps;
                if (!blockInfo.TryGetValue(lowerCaseType, out jsonProps))
                {
                    throw new Exception(string.Format("Invalid block properties JSON: {0} is not assigned any properties", lowerCaseType));
                }
                else
                {
                    Properties properties = new Properties();
                    properties.generateMesh = jsonProps.generateMesh;
                    properties.transparent = jsonProps.transparent;
                    #region Mesh generation
                    if (jsonProps.generateMesh)
                    {
                        properties.uvTileIndex = new int[Enum.GetNames(typeof(BlockSide)).Length];

                        bool all = jsonProps.all != null;
                        bool sides = jsonProps.sides != null;
                        bool caps = jsonProps.caps != null;
                        bool top = jsonProps.top != null;
                        bool bottom = jsonProps.bottom != null;
                        bool n = jsonProps.north != null;
                        bool e = jsonProps.east != null;
                        bool s = jsonProps.south != null;
                        bool w = jsonProps.west != null;

                        // First check if less specific is overwritten
                        if (sides && n && e && s && w)
                        {
                            sides = false;
                            jsonProps.sides = null;
                        }
                        if (caps && top && bottom)
                        {
                            caps = false;
                            jsonProps.caps = null;
                        }
                        if (all && (sides || (n && e && s && w)) && (caps || (top && bottom)))
                        {
                            all = false;
                            jsonProps.all = null;
                        }

                        // Do least specific sides
                        if (all)
                        {
                            int tile = GetUVTileIndex(jsonProps.all.fileLocation);
                            properties.uvTileIndex[(int)BlockSide.Bottom] = tile;
                            properties.uvTileIndex[(int)BlockSide.Top] = tile;
                            properties.uvTileIndex[(int)BlockSide.North] = tile;
                            properties.uvTileIndex[(int)BlockSide.East] = tile;
                            properties.uvTileIndex[(int)BlockSide.South] = tile;
                            properties.uvTileIndex[(int)BlockSide.West] = tile;
                            // TODO: Draw texture, assuming 64x64 size for now
                        }
                        if (sides)
                        {
                            int tile = GetUVTileIndex(jsonProps.sides.fileLocation);
                            properties.uvTileIndex[(int)BlockSide.North] = tile;
                            properties.uvTileIndex[(int)BlockSide.East] = tile;
                            properties.uvTileIndex[(int)BlockSide.South] = tile;
                            properties.uvTileIndex[(int)BlockSide.West] = tile;
                            // TD
                        }
                        if (caps)
                        {
                            int tile = GetUVTileIndex(jsonProps.caps.fileLocation);
                            properties.uvTileIndex[(int)BlockSide.Top] = tile;
                            properties.uvTileIndex[(int)BlockSide.Bottom] = tile;
                            // TD
                        }
                        // Overwrite with more specific sides as necessary
                        if (top)
                        {
                            properties.uvTileIndex[(int)BlockSide.Top] = GetUVTileIndex(jsonProps.top.fileLocation); ;
                            // TD
                        }
                        if (bottom)
                        {
                            properties.uvTileIndex[(int)BlockSide.Bottom] = GetUVTileIndex(jsonProps.bottom.fileLocation); ;
                            // TD
                        }
                        if (n)
                        {
                            properties.uvTileIndex[(int)BlockSide.North] = GetUVTileIndex(jsonProps.north.fileLocation); ;
                            // TD
                        }
                        if (e)
                        {
                            properties.uvTileIndex[(int)BlockSide.East] = GetUVTileIndex(jsonProps.east.fileLocation);
                            // TD
                        }
                        if (s)
                        {
                            properties.uvTileIndex[(int)BlockSide.South] = GetUVTileIndex(jsonProps.south.fileLocation);
                            // TD
                        }
                        if (w)
                        {
                            properties.uvTileIndex[(int)BlockSide.West] = GetUVTileIndex(jsonProps.caps.fileLocation);
                            // TD
                        }
                    }
                    #endregion Mesh generation
                    blockProperties[(int)Enum.Parse(typeof(BlockType), typeName)] = properties;
                }
            }
        }

        static int GetUVTileIndex(string fileLocation)
        {
            int index;
            if (textureIndices.TryGetValue(fileLocation, out index))
            {
                return index;
            }
            else return textureIndices[fileLocation] = currentUVTile++;
        }

        public static Vector2[] GetUVs(int uvTileIndex)
        {
            const int CANVAS_SIZE = 8;
            const float BLOCK_SIZE = 1f / CANVAS_SIZE;
            int x = uvTileIndex % CANVAS_SIZE;
            float u = x * BLOCK_SIZE;
            int y = uvTileIndex / CANVAS_SIZE;
            float v = y * BLOCK_SIZE;

            float e = 0.002f;

            Vector2 uv00 = new Vector2(u + e, v + e);
            Vector2 uv01 = new Vector2(u + e, v + BLOCK_SIZE - e);
            Vector2 uv10 = new Vector2(u + BLOCK_SIZE - e, v + e);
            Vector2 uv11 = new Vector2(u + BLOCK_SIZE - e, v + BLOCK_SIZE - e);
            return new Vector2[] { uv11, uv01, uv00, uv10 };
        }
    }

    enum BlockSide
    {
        Top,
        Bottom,
        East,
        North,
        West,
        South
    }

    class JsonBlockProperties
    {
        public bool transparent;
        public bool generateMesh;
        public TextureProperties top;
        public TextureProperties north;
        public TextureProperties east;
        public TextureProperties south;
        public TextureProperties west;
        public TextureProperties bottom;
        public TextureProperties all;
        public TextureProperties sides;
        public TextureProperties caps;
    }

    class TextureProperties
    {
        public string fileLocation;
    }
}