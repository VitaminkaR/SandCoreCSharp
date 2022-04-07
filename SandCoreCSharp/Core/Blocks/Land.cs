using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core.Blocks
{
    // класс грядки (основа земледелия)
    class Land : Block
    {
        public Land(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "land";
            IsSolid = false;
            Hardness = 0;
            Instrument = "";
        }



        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override void Using()
        {
            Resources res = SandCore.resources;
            Inventory inventory = SandCore.inventory;

           

            base.Using();
        }
    }
}
