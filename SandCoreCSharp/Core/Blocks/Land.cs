using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Utils;
using System;

namespace SandCoreCSharp.Core.Blocks
{
    // класс грядки (основа земледелия)
    class Land : Block
    {
        // полита ли грядка?
        private bool isWatered;
        // что растет
        private string seed;
        // стадия роста
        private int stage;



        public Land(Game game, Vector2 pos) : base(game, pos)
        {
            IsSolid = false;
            Hardness = 0;
            Instrument = "";

            stage = 0;
            seed = "none";
        }



        public override void Draw(GameTime gameTime)
        {
            if (isWatered)
                sprite = Sprites["mud"];

            base.Draw(gameTime);
        }

        protected override void Using()
        {
            Resources res = SandCore.resources;
            Inventory inventory = SandCore.inventory;

           if(!isWatered && seed == "none")
           {
                isWatered = true;
           }

            base.Using();
        }
    }
}
