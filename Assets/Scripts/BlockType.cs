using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARACore
{
    public enum BlockType : int
    {
        // All transparent types should be <= 0

        Glass = -1,
        Air   = 0,
        Grass = 1,
        Robot = 2,
    }
}
