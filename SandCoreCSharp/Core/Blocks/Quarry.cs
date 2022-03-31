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

            SimpleTimer timer = new SimpleTimer(1000, Mining, null);
        }

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("Quarry");

            base.LoadContent();
        }

        private void Mining(object obj)
        {
            Resources res = SandCore.game.resources;

            res.AddResource("sand", 35);
            res.AddResource("quartz", 30);
            res.AddResource("stone", 25);
            res.AddResource("coal", 20);
            res.AddResource("raw_iron", 15);
            res.AddResource("raw_gold", 10);
            res.AddResource("adamant", 1);

            SimpleTimer timer = new SimpleTimer(1000, Mining, null);
        }
    }
}
