﻿using Microsoft.Xna.Framework;
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
        }

        public override void Initialize()
        {
            state = WireStates.CrossOff;
            statesSprites = new Texture2D[6];

            base.Initialize();
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

        public override void Draw(GameTime gameTime)
        {
            if (state == WireStates.CrossOff)
                sprite = statesSprites[0];
            if(state == WireStates.LROff)
                sprite = statesSprites[1];
            if(state == WireStates.UDOff)
                sprite = statesSprites[2];
            if(state == WireStates.CrossOn)
                sprite = statesSprites[3];
            if(state == WireStates.LROn)
                sprite = statesSprites[4];
            if(state == WireStates.UDOn)
                sprite = statesSprites[5];

            base.Draw(gameTime);
        }

        public override void Break()
        {
            Resources resources = (Game as SandCore).resources;
            resources.AddResource("wire", 1);

            base.Break();
        }
    }
}
