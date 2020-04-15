using System.Collections.Generic;

namespace Assets.Scripts.Chunk
{
    public class BlockProperties
    {
        public Properties[] Values;
        int currentTile;

        public BlockProperties()
        {
            //Air,
            //Stone,
            //Grass,
            //Dirt,
            //Robot,
            //Furnace
            
            var values = new List<Properties>();
            // Air
            values.Add(new Properties
            {
                GenerateMesh = false,
                Transparent = true
            });
            // Stone
            values.Add(new Properties
            {
                GenerateMesh = true,
                Transparent = false,
                All = true
            });
            // Grass
            values.Add(new Properties
            {
                GenerateMesh = true,
                Transparent = false,
                Sides = true,
                Top = true,
                Bottom = true
            });
            // Dirt
            values.Add(new Properties
            {
                GenerateMesh = true,
                All = true
            });
            // Robot
            values.Add(new Properties
            {
                GenerateMesh = false,
                Transparent = true
            });
            // Furnace
            values.Add(new Properties
            {
                GenerateMesh = true,
                Transparent = false
            });

            Values = values.ToArray();

            CalculateTiles();
        }

        public class Properties
        {
            public bool GenerateMesh;
            public bool Transparent;
            public bool Top;
            public bool North;
            public bool East;
            public bool South;
            public bool West;
            public bool Bottom;
            public bool All;
            public bool Sides;
            public bool Caps;
            public int[] TileIndex;
        }

        public void CalculateTiles()
        {
            currentTile = 0;
            foreach (var props in Values)
            {
                if (!props.GenerateMesh) continue;
                props.TileIndex = new int[6];

                // First check if less specific is overwritten
                if (props.Sides && props.North && props.East && props.South && props.West)
                {
                    props.Sides = false;
                }

                if (props.Caps && props.Top && props.Bottom)
                {
                    props.Caps = false;
                }

                if (props.All && (props.Sides || (props.North && props.East && props.South && props.West)) &&
                    (props.Caps || (props.Top && props.Bottom)))
                {
                    props.All = false;
                }

                if (props.All)
                {
                    int tile = currentTile++;
                    props.TileIndex[(int) BlockSide.Bottom] = tile;
                    props.TileIndex[(int) BlockSide.Top] = tile;
                    props.TileIndex[(int) BlockSide.North] = tile;
                    props.TileIndex[(int) BlockSide.East] = tile;
                    props.TileIndex[(int) BlockSide.South] = tile;
                    props.TileIndex[(int) BlockSide.West] = tile;
                }

                if (props.Sides)
                {
                    int tile = currentTile++;
                    props.TileIndex[(int) BlockSide.North] = tile;
                    props.TileIndex[(int) BlockSide.East] = tile;
                    props.TileIndex[(int) BlockSide.South] = tile;
                    props.TileIndex[(int) BlockSide.West] = tile;
                }

                if (props.Caps)
                {
                    int tile = currentTile++;
                    props.TileIndex[(int) BlockSide.Top] = tile;
                    props.TileIndex[(int) BlockSide.Bottom] = tile;
                }

                if (props.Top)
                {
                    props.TileIndex[(int) BlockSide.Top] = currentTile++;
                }

                if (props.Bottom)
                {
                    props.TileIndex[(int) BlockSide.Bottom] = currentTile++;
                }

                if (props.North)
                {
                    props.TileIndex[(int) BlockSide.North] = currentTile++;
                }

                if (props.East)
                {
                    props.TileIndex[(int) BlockSide.East] = currentTile++;
                }

                if (props.South)
                {
                    props.TileIndex[(int) BlockSide.South] = currentTile++;
                }

                if (props.West)
                {
                    props.TileIndex[(int) BlockSide.West] = currentTile++;
                }
            }
        }
    }

    enum BlockSide
    {
        Top,
        Bottom,
        North,
        East,
        West,
        South
    }
}