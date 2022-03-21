using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core
{
    // содержит чанк и свою позиция в этом чанку  0 <= x,y <= 15
    struct Tile
    {
        public Chunk Chunk { get; private set; }
        public int[] Position { get; private set; }

        public Tile(int ix, int iy, Chunk chunk)
        {
            this.Chunk = chunk;
            Position = new int[2] { ix, iy };
        }
    }
}
