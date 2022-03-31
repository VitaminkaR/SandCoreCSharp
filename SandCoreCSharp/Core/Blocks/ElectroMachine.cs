using Microsoft.Xna.Framework;
using SandCoreCSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SandCoreCSharp.Core.Blocks
{
    public class ElectroMachine : Block
    {
        // размер буфера энергии (увеличивается вместе с созданием батарей)
        static public int MaxEnergy { get; protected set; } = 0;

        // заряжена ли машина
        public bool Powered { get; set; }

        // сколько тратит энергии (свойство)
        public int EnergyConsumption { get; protected set; } = 1;

        protected Resources res;

        public override void Update(GameTime gameTime)
        {
            if (res.Energy > EnergyConsumption)
                Powered = true;
            else
                Powered = false;
            
            if (Powered)
                res.Energy -= EnergyConsumption;

            // проверяем не перебор ли с энергий
            if (res.Energy > MaxEnergy)
                res.Energy = MaxEnergy;

            base.Update(gameTime);
        }

        public ElectroMachine(Game game, Vector2 pos) : base(game, pos)
        {
            res = SandCore.game.resources;
        }
    }
}
