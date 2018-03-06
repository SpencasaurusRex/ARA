using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARACore
{
    public enum BlockType : byte
    {
        Null  = 0, // TODO: get rid of this
        Air   = 1,
        Grass = 2,
        Robot = 3,
    }
}
