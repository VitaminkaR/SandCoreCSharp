using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core.Blocks
{
    // состояния провода LR - left-right UD - up-down
    enum WireStates
    {
        CrossOff, 
        LROff, 
        UDOff,
        CrossOn,
        LROn,
        UDOn,
    }

    class Wire : Block
    {
        // спрайты
        private Texture2D[] statesSprites;

        // состояние провода
        private WireStates state;

        public Wire(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "wire";
            IsSolid = false;
            Hardness = 0;
            isSaving = true;

            statesSprites = new Texture2D[6];
        }

        protected override void LoadContent()
        {
            statesSprites[0] = content.Load<Texture2D>("wires\\Wire-1");
            statesSprites[1] = content.Load<Texture2D>("wires\\Wire-2");
            statesSprites[2] = content.Load<Texture2D>("wires\\Wire-3");
            statesSprites[3] = content.Load<Texture2D>("wires\\Wire-1o");
            statesSprites[4] = content.Load<Texture2D>("wires\\Wire-2o");
            statesSprites[5] = content.Load<Texture2D>("wires\\Wire-3o");

            base.LoadContent();
        }

        public override void Break()
        {
            Resources resources = (Game as SandCore).resources;
            resources.AddResource("wire", 1);

            base.Break();
        }
    }
}
