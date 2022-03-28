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
        // провода
        static public List<Wire> Wires { get; protected set; } = new List<Wire>();

        // кол-во энергии
        static public int Energy { get; protected set; } = 0;

        // размер буфера энергии (увеличивается вместе с созданием батарей)
        static public int MaxEnergy { get; protected set; } = 5000;

        // заряжена ли машина
        public bool Powered { get; set; }

        // сколько тратит энергии (свойство)
        public int EnergyConsumption { get; protected set; } = 1;

        protected Wire left;
        protected Wire right;
        protected Wire up;
        protected Wire down;

        public override void Update(GameTime gameTime)
        {
            if (Powered)
                Energy -= EnergyConsumption;

            base.Update(gameTime);
        }

        public ElectroMachine(Game game, Vector2 pos) : base(game, pos)
        {
        }

        // wire system
        static public void UpdateWires()
        {
            // проверяем не перебор ли с энергий
            if (Energy > MaxEnergy)
                Energy = MaxEnergy;

            // обнуляем все провода
            for (int i = 0; i < Wires.Count; i++)
                Wires[i].Powered = false;

            // если энергии мало, то орубаем провода
            if (Energy < Wires.Count)
                return;

            // проеряем все провода
            for (int i = 0; i < Wires.Count; i++)
            {
                Wire wire = Wires[i];
                if (FindBattery(wire.Pos)) // если есть батарея, то провдо является источником и начинает рекурсию
                    GetImpulse(wire, "none");
            }
        }

        // рекурсия передачи импульса в цепочке
        static protected void GetImpulse(Wire wire, string side) 
        {
            wire.Powered = true;
            if(wire.left != null && (side != "left" || side == "none") && !wire.left.Powered)
                GetImpulse(wire.left, "right");
            if (wire.right != null && (side != "right" || side == "none") && !wire.right.Powered)
                GetImpulse(wire.right, "left");
            if (wire.up != null && (side != "up" || side == "none") && !wire.up.Powered)
                GetImpulse(wire.up, "down");
            if (wire.down != null && (side != "down" || side == "none") && !wire.down.Powered)
                GetImpulse(wire.down, "up");
        }

        // найти провода по краям
        protected void FindWires()
        {
            left = null;
            right = null;
            up = null;
            down = null;

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

        // статический метод чтобы проверить есть ли батарея
        static protected bool FindBattery(Vector2 Pos)
        {
            // ищем батарейку
            Battery battery = null;
            int count = 0;
            battery = FindBlock(Pos + new Vector2(32, 0)) as Battery;
            if (battery != null && battery.Powered) count++;
            battery = FindBlock(Pos + new Vector2(-32, 0)) as Battery;
            if (battery != null && battery.Powered) count++;
            battery = FindBlock(Pos + new Vector2(0, 32)) as Battery;
            if (battery != null && battery.Powered) count++;
            battery = FindBlock(Pos + new Vector2(0, -32)) as Battery;
            if (battery != null && battery.Powered) count++;
            if (count > 0) return true;
            return false;
        }



        // сохраняет инфу о ресурсах в файл
        static public void SaveResourceEnergy()
        {
            string data = Energy.ToString();

            using (StreamWriter sr = new StreamWriter("maps\\" + SandCore.map + "\\energy"))
            {
                sr.Write(data);
            }
        }

        // загружает ресурсы
        static public void LoadResourceEnergy()
        {
            if (new FileInfo("maps\\" + SandCore.map + "\\energy").Exists)
            {
                using (StreamReader sr = new StreamReader("maps\\" + SandCore.map + "\\energy"))
                {
                    string str = sr.ReadLine();
                    Energy = Convert.ToInt32(str);
                }
            }
        }
    }
}
