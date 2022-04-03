using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SandCoreCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core.Blocks
{
    // промышленный карьер, похож на шахту, но копает новые ископаемые + намного эффективней, хоть и тратит кучу энергии
    class Quarry : ElectroMachine
    {
        public Quarry(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "quarry";
            isSaving = true;
            Hardness = 0;
            IsSolid = true;
            Instrument = "";
            EnergyConsumption = 32;
        }

        public override void Update(GameTime gameTime)
        {
            Resources res = SandCore.game.resources;

            res.AddResource("sand", 0.35f);
            res.AddResource("quartz", 0.30f);
            res.AddResource("stone", 0.25f);
            res.AddResource("coal", 0.20f);
            res.AddResource("raw_iron", 0.15f);
            res.AddResource("raw_gold", 0.1f);
            res.AddResource("adamant", 0.01f);

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("Quarry");

            base.LoadContent();
        }
    }
}
