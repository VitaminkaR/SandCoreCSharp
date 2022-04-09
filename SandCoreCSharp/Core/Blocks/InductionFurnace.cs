using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SandCoreCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core.Blocks
{
    class InductionFurnace : ElectroMachine
    {
        public InductionFurnace(Game game, Vector2 pos) : base(game, pos)
        {
            isSaving = true;
            Hardness = 0;
            IsSolid = true;
            Instrument = "";
            EnergyConsumption = 16;
        }

        public override void Update(GameTime gameTime)
        {
            if(res.Resource["raw_gold"] > 0)
            {
                res.AddResource("gold", 1);
                res.AddResource("raw_gold", -1);
            }
            if (res.Resource["raw_iron"] > 0)
            {
                res.AddResource("iron", 1);
                res.AddResource("raw_iron", -1);
            }

            base.Update(gameTime);
        }
    }
}
