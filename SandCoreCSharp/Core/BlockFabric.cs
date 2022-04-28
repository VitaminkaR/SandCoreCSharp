using Microsoft.Xna.Framework;
using SandCoreCSharp.Core.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core
{
    class BlockFabric
    {
        public virtual Block Create(string type, Vector2 pos)
        {
            Block block = null;

            if (type == "wood")
                block = new Wood(SandCore.game, pos);
            if (type == "furnace")
                block = new Furnace(SandCore.game, pos);
            if (type == "mine")
                block = new Mine(SandCore.game, pos);
            if (type == "lumberjack")
                block = new Lumberjack(SandCore.game, pos);
            if (type == "coalgenerator")
                block = new CoalGenerator(SandCore.game, pos);
            if (type == "quarry")
                block = new Quarry(SandCore.game, pos);
            if (type == "inductionfurnace")
                block = new InductionFurnace(SandCore.game, pos);
            if (type == "land")
                block = new Land(SandCore.game, pos);
            if (type == "farmer")
                block = new Farmer(SandCore.game, pos);

            return block;
        }
    }
}
