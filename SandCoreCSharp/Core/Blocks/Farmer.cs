using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core.Blocks
{
    class Farmer : ElectroMachine
    {
        private List<Land> lands;

        public Farmer(Game game, Vector2 pos) : base(game, pos)
        {
            isSaving = true;
            Hardness = 0;
            IsSolid = true;
            Instrument = "";
            EnergyConsumption = 4;

            lands = new List<Land>();
            for (int i = -3; i < 4; i++)
            {
                for (int j = -3; j < 4; j++)
                {
                    // создаем блок
                    Block block = CreateBlock("land", Pos + new Vector2(i * 32, j * 32));
                    if (block != null) 
                        lands.Add((Land)block);
                    else
                    { // если блок там уже есть, то находим его и проверяем грядка ли это, если да, то тоже кидаем в массив
                        Block block_other = FindBlock(Pos + new Vector2(i * 32, j * 32));
                        if(block_other.GetType() == typeof(Land))
                            lands.Add((Land)block_other);
                    } 
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // поливаем и засеиваем все грядки
            for (int i = 0; i < lands.Count; i++)
            {
                lands[i].Wet();
                lands[i].Plant();
            }

            base.Update(gameTime);
        }

        public override void Break()
        {
            for (int i = 0; i < lands.Count; i++)
            {
                lands[i].Break();
            }

            base.Break();
        }
    }
}
