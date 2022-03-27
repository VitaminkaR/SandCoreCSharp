using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

namespace SandCoreCSharp.Core.Blocks
{
    // состояния провода LR - left-right UD - up-down
    public enum WireStates
    {
        Cross, 
        LR, 
        UD,
    }

    public class Wire : ElectroMachine
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
            state = WireStates.Cross;
            statesSprites = new Texture2D[6];

            Wires.Add(this);

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
            if (state == WireStates.Cross)
                sprite = statesSprites[0];
            if(state == WireStates.LR)
                sprite = statesSprites[1];
            if(state == WireStates.UD)
                sprite = statesSprites[2];
            if(state == WireStates.Cross && Powered)
                sprite = statesSprites[3];
            if(state == WireStates.LR && Powered)
                sprite = statesSprites[4];
            if(state == WireStates.UD && Powered)
                sprite = statesSprites[5];

            base.Draw(gameTime);
        }

        public override void Break()
        {
            Resources resources = (Game as SandCore).resources;
            resources.AddResource("wire", 1);
            Wires.Remove(this);

            base.Break();
        }

        public override void Update(GameTime gameTime)
        {
            FindWires();

            // если хотя бы 1 провод заряженный, то заряжаем этот
            if(left != null)
                Powered = left.Powered;
            if (right != null)
                Powered = right.Powered;
            if (up != null)
                Powered = up.Powered;
            if (down != null)
                Powered = down.Powered;

            // меняем состояние от положения других проводов
            int x = 0;
            int y = 0;
            if (left != null || right != null)
                x++;
            if (up != null || down != null)
                y++;
            if (x == 1)
                state = WireStates.LR;
            if (y == 1)
                state = WireStates.UD;
            if (x == 1 && y == 1)
                state = WireStates.Cross;
            

            base.Update(gameTime);
        }
    }
}
