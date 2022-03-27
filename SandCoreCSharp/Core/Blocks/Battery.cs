using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core.Blocks
{
    class Battery : ElectroMachine
    {
        public Battery(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "battery";
            isSaving = true;
            Hardness = 0;
            IsSolid = true;
            Instrument = "";

            MaxEnergy += 10000;
        }

        public override void Break()
        {
            MaxEnergy -= 10000;
            Resources resources = (Game as SandCore).resources;
            resources.AddResource("battery", 1);

            base.Break();
        }

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("Battery");

            base.LoadContent();
        }
    }
}
