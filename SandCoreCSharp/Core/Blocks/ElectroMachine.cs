using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core.Blocks
{
    public class ElectroMachine : Block
    {
        // провода
        static public List<Wire> Wires { get; protected set; } = new List<Wire>();

        // кол-во энергии
        static public int Energy { get; protected set; } = 0;

        // размер буфера энергии (увеличивается вместе с созданием батарей)
        static public int MaxEnergy { get; protected set; } = 5000;

        // заряжена ли машина
        public bool Powered { get; protected set; }

        // сколько тратит энергии (свойство)
        public int EnergyConsumption { get; protected set; } = 1;

        protected Wire left;
        protected Wire right;
        protected Wire up;
        protected Wire down;

        public override void Update(GameTime gameTime)
        {
            if (Energy - EnergyConsumption > 0)
            {
                Energy -= EnergyConsumption;
                Powered = true;
            }
            else
                Powered = false;

            base.Update(gameTime);
        }

        public ElectroMachine(Game game, Vector2 pos) : base(game, pos)
        {
        }

        // найти провода по краям
        protected void FindWires()
        {
            // находим провода по края
            for (int i = 0; i < Wires.Count; i++)
            {
                Wire wire = Wires[i];
                if (wire.Pos == Pos + new Vector2(-32, 0))
                    left = wire;
                if (wire.Pos == Pos + new Vector2(32, 0))
                    right = wire;
                if (wire.Pos == Pos + new Vector2(0, -32))
                    up = wire;
                if (wire.Pos == Pos + new Vector2(0, 32))
                    down = wire;
            }
        }
    }
}
