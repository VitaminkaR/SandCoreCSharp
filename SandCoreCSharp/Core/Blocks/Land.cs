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

        public Land(Game game, Vector2 pos) : base(game, pos)
        {
            IsSolid = false;
            Hardness = 0;
            Instrument = "";

            Tags[0] = "f";
            Tags[1] = "none";
            Tags[2] = "first";

            LoadTags();
        }



        public override void Draw(GameTime gameTime)
        {
            if (Tags[0] == "t")
                sprite = Sprites["mud"];

            base.Draw(gameTime);
        }

        protected override void Using()
        {
            Resources res = SandCore.resources;
            Inventory inventory = SandCore.inventory;

           if(Tags[0] == "f" && Tags[1] == "none" && inventory.choosenBlock == "bucket")
           {
                Tags[0] = "t";
                SaveTags();
           }

            base.Using();
        }
    }
}
