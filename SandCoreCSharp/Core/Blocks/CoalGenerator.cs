using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core.Blocks
{
    class CoalGenerator : ElectroMachine
    {
        public CoalGenerator(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "coal_generator";
            isSaving = true;
            Hardness = 0;
            IsSolid = true;
            Instrument = "";
            EnergyConsumption = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (Energy + 256 < MaxEnergy)
                Energy += 256;

            base.Update(gameTime);
        }

        public override void Break()
        {
            Resources resources = (Game as SandCore).resources;
            resources.AddResource("coal_generaotr", 1);

            base.Break();
        }

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("CoalGenerator");

            base.LoadContent();
        }
    }
}
