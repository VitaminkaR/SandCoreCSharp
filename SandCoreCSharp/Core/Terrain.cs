using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;
using SandCoreCSharp.Utils;
using System.Diagnostics;
using System;
using SandCoreCSharp.Core.Blocks;
using System.IO;

namespace SandCoreCSharp.Core
{
    // класс, генерирующий и отрисовывающий чанки
    public class Terrain : DrawableGameComponent
    {
        private ContentManager content;
        private SpriteBatch spriteBatch;

        // sprites tiles
        // 0 - mud
        // 1 - grass
        // 2 - stone
        // 3 - water
        private Texture2D[] sprites;

        // view chunks
        private List<Chunk> chunks;

        public Terrain(Game game) : base(game)
        {
            game.Components.Add(this);
            content = game.Content;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }


        // init
        public override void Initialize()
        {
            chunks = new List<Chunk>();
            sprites = new Texture2D[255];
            GenerateStructure(null);

            base.Initialize();
        }

        // load sprites (ADD SPRITES)
        protected override void LoadContent()
        {
            sprites[0] = content.Load<Texture2D>("Mud");
            sprites[1] = content.Load<Texture2D>("Grass");
            sprites[2] = content.Load<Texture2D>("Stone");
            sprites[3] = content.Load<Texture2D>("Water");

            base.LoadContent();
        }

        // draw chunks
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // проходим пов сем чанкам
            for (int i = 0; i < chunks.Count; i++)
            {
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        for (int z = 15; z > -1; z--)
                        {
                            byte id = chunks[i].Tiles[x, y, z]; // получаем id
                            if (id == 0) // если воздух то идем далее
                                continue;

                            Vector2 camPos = (Game as SandCore).camera.Pos; // берем позицию камеры
                            Vector2 chunkPos = chunks[i].Pos;

                            // отрисовываем блок в позиции относительно камеры и относительно координат чанка
                            // с текстурой для его id
                            // делаем более темным в зависимости от его высоты
                            spriteBatch.Draw(sprites[id - 1], new Vector2(-camPos.X + x * 32 + chunkPos.X, -camPos.Y + y * 32 + chunkPos.Y), new Color(40 + z * 13, 40 + z * 13, 40 + z * 13));

                            // для дебага
                            if (SandCore.debugChunks && (x == 0 || x == 15 || y == 0 || y == 15))
                                spriteBatch.Draw(sprites[id - 1], new Vector2(-camPos.X + x * 32 + chunkPos.X, -camPos.Y + y * 32 + chunkPos.Y), new Color(255, 0, 0));
                            break; // если блок отрисован, то нет смысла рисовать ниже (оптимизация)
                        }
                    }
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 camPos = (Game as SandCore).camera.Pos; // берем позицию камеры
            Vector2 camBor = (Game as SandCore).camera.Borders; // получаем края камеры

            // генерация
            int ofx = (int)(camPos.X / 512);
            int ofy = (int)(camPos.Y / 512);
            for (int i = ofx - 1; i < ofx + 4; i++)
            {
                for (int j = ofy - 1; j < ofy + 3; j++)
                {
                    Generate(i * 512, j * 512, i + 1, j + 1);
                }
            }

            // проверка на видимость 
            for (int i = 0; i < chunks.Count; i++)
            {
                Chunk chunk = chunks[i]; // получаем чанк
                // проверяем входит ли чанк в границы камеры, если нет, то удаляем его из отрисовываемых
                // и генерируем новый чанк
                if (chunk.Pos.X > camBor.X + SandCore.WIDTH || chunk.Pos.Y > camBor.Y + SandCore.WIDTH ||
                    (chunk.Pos.X + 512) < camPos.X - SandCore.HEIGHT || (chunk.Pos.Y + 512) < camPos.Y - SandCore.HEIGHT)
                {
                    chunks.Remove(chunk);
                }
            }

            base.Update(gameTime);
        }



        // generate chunk
        public void Generate(float _x, float _y, int px, int py)
        {
            if (chunks.Any(obj => obj.Pos.X == _x && obj.Pos.Y == _y)) // проверяем если такой чанк есть, то не создаем такой же
                return;

            Chunk @new = new Chunk(_x, _y);

            var heights = SimplexNoise.GetNoise(px * 16, py * 16, 0.01f); // находим высоты
            // генерация тайлов
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    @new.Tiles[x, y, heights[x, y]] = 2; // земля
                    // изменение блоков от высоты
                    if (heights[x, y] > 14)
                        @new.Tiles[x, y, heights[x, y]] = 3; // горы
                    if (heights[x, y] < 4)
                        @new.Tiles[x, y, heights[x, y]] = 4;// вода


                    // под землей должны быть камни
                    for (int i = 0; i < heights[x, y]; i++)
                    {
                        @new.Tiles[x, y, i] = 3;
                    }
                }
            }

            // добавляем в рисуемые чанки (потому все с камерой будет связано)
            chunks.Add(@new);

            // единичная генерация
            Random rand = new Random();
            if (!new FileInfo("maps\\" + SandCore.map + "\\chunks\\" + @new.GetName()).Exists)
            {
                int x = rand.Next(16);
                int y = rand.Next(16);
                Vector2 pos = new Vector2(x * 32 + @new.Pos.X, y * 32 + @new.Pos.Y);
                if (GetTile(pos).ID != 4)
                    Block.CreateBlock("wood", new Vector2(x * 32 + @new.Pos.X, y * 32 + @new.Pos.Y));
            }
        }



        // возвращает чанк по координате
        public Chunk GetChunk(float x, float y)
        {
            int ix = (int)(x / 512);
            int iy = (int)(y / 512);
            if (x < 0) ix--;
            if (y < 0) iy--;
            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].Pos == new Vector2(ix * 512, iy * 512))
                    return chunks[i];
            }

            return chunks[0];
        }

        // возвращает чанк в котором игрок
        public Chunk GetChunkExistPlayer() => GetChunk((Game as SandCore).hero.Pos.X, (Game as SandCore).hero.Pos.Y);

        // возвращает блок на котором стоит игрок
        public int[] GetChunkPosPlayer()
        {
            Tile tile = GetTile((Game as SandCore).hero.Pos);

            int oz = 0;

            for (int i = 15; i > -1; i--)
            {
                if (tile.Chunk.Tiles[tile.Position[0], tile.Position[1], i] != 0)
                {
                    oz = i;
                    break;
                }
            }

            return new int[] { tile.Position[0], tile.Position[1], oz };
        }

        // возвращает плитку
        public Tile GetTile(Vector2 pos)
        {
            // ищем чанк
            Chunk chunk = GetChunk(pos.X, pos.Y);
            // находим координаты относительно чанка
            Vector2 p1 = pos - chunk.Pos - new Vector2(1, 1);
            // находим индексы тайла
            int x = (int)(p1.X / 32);
            int y = (int)(p1.Y / 32);

            if (x > 15 || y > 15)
                return new Tile(0, 0, chunk, 0);

            // ищем верхний блок
            int z = 0;
            for (int i = 0; i < 16; i++)
            {
                if (chunk.Tiles[x, y, i] == 0)
                    continue;
                else
                    z = i;
            }

            return new Tile(x, y, chunk, chunk.Tiles[x, y, z]);
        }

        // возвращает  id блока на котором стоит игрок
        public byte GetBlockIdPlayerPlace(Chunk chunk, int[] pos) => chunk.Tiles[pos[0], pos[1], pos[2]];

        // генеарция камушков и т д
        private void GenerateStructure(object obj)
        {
            Random rand = new Random();

            int stonesCount = 0;
            for (int i = 0; i < Block.Blocks.Count; i++)
            {
                Block block = Block.Blocks[i];
                if ((block as Stones) != null)
                    stonesCount++;
            }

            // генерация камушков

            if (stonesCount < 10)
            {
                Hero player = (Game as SandCore).hero;
                rand = new Random();
                Vector2 p1 = player.Pos + new Vector2(rand.Next(-1024, 1024), rand.Next(-1024, 1024));
                Vector2 p2 = new Vector2((int)(p1.X / 32) * 32, (int)(p1.Y / 32) * 32);
                new Stones(Game, p2);
            }

            SimpleTimer timer = new SimpleTimer(15000, GenerateStructure, null);
        }
    }
}
