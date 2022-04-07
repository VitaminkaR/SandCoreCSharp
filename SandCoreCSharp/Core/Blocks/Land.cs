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
        private Texture2D[] textures;

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

        protected override void LoadContent()
        {
            textures = new Texture2D[3];
            textures[0] = content.Load<Texture2D>("Land");
            textures[1] = content.Load<Texture2D>("Mud");
            textures[2] = content.Load<Texture2D>("MudWithSeeds");
            sprite = textures[0];

            base.LoadContent();
        }

        protected override void Using()
        {
            Resources res = SandCore.resources;
            Inventory inventory = SandCore.inventory;

           

            base.Using();
        }
    }
}
