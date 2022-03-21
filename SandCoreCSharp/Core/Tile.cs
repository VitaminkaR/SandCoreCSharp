using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core
{
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
