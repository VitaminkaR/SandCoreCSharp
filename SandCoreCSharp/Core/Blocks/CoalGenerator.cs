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
            if (res.Energy + 256 < Resources.MaxEnergy && res.Resource["coal"] > 0)
            {
                res.Energy += 256;
                res.AddResource("coal", -1);
            }         

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("CoalGenerator");

            base.LoadContent();
        }
    }
}
