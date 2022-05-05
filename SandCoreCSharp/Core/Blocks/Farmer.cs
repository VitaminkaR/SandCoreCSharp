using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core.Blocks
{
    class Farmer : ElectroMachine
    {
        private List<Land> lands;
        private List<Block> placeBlock; // все блоки в определенном радиусе(далее отсеятся в lands)



        public Farmer(Game game, Vector2 pos) : base(game, pos)
        {
            isSaving = true;
            Hardness = 0;
            IsSolid = true;
            Instrument = "";
            EnergyConsumption = 4;
            lands = new List<Land>();
            placeBlock = new List<Block>();
        }



        public override void Update(GameTime gameTime)
        {
            // ищем все грядки (или вскапываем)
            if (placeBlock.Count < 49) // пока не все блоки найдены, то ищем их
            {
                for (int i = -3; i < 4; i++)
                {
                    for (int j = -3; j < 4; j++)
                    {
                        // создаем блок
                        Block block = CreateBlock("land", Pos + new Vector2(i * Terrain.TILE_SIZE, j * Terrain.TILE_SIZE));
                        if (block != null)
                        {
                            lands.Add((Land)block);
                        }
                        else
                        { // если блок там уже есть, то находим его и проверяем грядка ли это, если да, то тоже кидаем в массив
                            Block block_other = FindBlock(Pos + new Vector2(i * Terrain.TILE_SIZE, j * Terrain.TILE_SIZE));
                            placeBlock.Add(block_other);
                        }
                    }
                }
            }
            else // если блоки найдены отсеиваем грядки
            {
                foreach (Block block in placeBlock)
                {
                    if (block.GetType() == typeof(Land))
                        lands.Add(block as Land);
                }
            }

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
