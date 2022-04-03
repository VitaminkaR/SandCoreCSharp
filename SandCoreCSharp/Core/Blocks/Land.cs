﻿using Microsoft.Xna.Framework;
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
        // полита ли градяка водой
        private bool wet;

        private Texture2D[] textures;



        public Land(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "land";
            IsSolid = false;
            Hardness = 1;
            Instrument = "";
        }

        protected override void LoadContent()
        {
            textures = new Texture2D[2];
            textures[0] = content.Load<Texture2D>("Land");
            textures[1] = content.Load<Texture2D>("Mud");
            sprite = textures[0];

            base.LoadContent();
        }

        protected override void Using()
        {
            Resources res = SandCore.resources;
            Inventory inventory = SandCore.inventory;

            if (!wet && inventory.choosenBlock == "bucket")
            {
                res.Resource["water"] -= 1;
                wet = true;
                sprite = textures[1];
            }

            base.Using();
        }
    }
}
