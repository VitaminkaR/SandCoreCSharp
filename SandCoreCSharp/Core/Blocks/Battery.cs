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
            EnergyConsumption = 0;

            MaxEnergy += 10000;
        }

        public override void Update(GameTime gameTime)
        {
            if (res.Energy > 20)
                Powered = true;
            else
                Powered = false;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        { 

            base.Draw(gameTime);
        }

        public override void Break()
        {
            MaxEnergy -= 10000;

            base.Break();
        }

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("Battery");

            base.LoadContent();
        }
    }
}
