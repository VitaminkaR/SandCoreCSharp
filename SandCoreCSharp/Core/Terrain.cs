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
        // 4 - sand
        private Texture2D[] sprites;

        // view chunks
        public List<Chunk> Chunks { get; private set; }

        public Terrain(Game game) : base(game)
        {
            game.Components.Add(this);
            content = game.Content;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }


        // init
        public override void Initialize()
        {
            Chunks = new List<Chunk>();
            sprites = new Texture2D[255];

            base.Initialize();
        }

        // load sprites (ADD SPRITES)
        protected override void LoadContent()
        {
            sprites[0] = content.Load<Texture2D>("Mud");
            sprites[1] = content.Load<Texture2D>("Grass");
            sprites[2] = content.Load<Texture2D>("Stone");
            sprites[3] = content.Load<Texture2D>("Water");
            sprites[4] = content.Load<Texture2D>("Sand");

            base.LoadContent();
        }

        // draw chunks
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // проходим пов сем чанкам
            for (int i = 0; i < Chunks.Count; i++)
            {
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        for (int z = 15; z > -1; z--)
                        {
                            byte id = Chunks[i].Tiles[x, y, z]; // получаем id
                            if (id == 0) // если воздух то идем далее
                                continue;

                            Vector2 camPos =SandCore.camera.Pos; // берем позицию камеры
                            Vector2 chunkPos = Chunks[i].Pos;

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
            Vector2 CamPos = SandCore.camera.Pos; // берем позицию камеры

            // генерация
            Vector2 CenterCameraPos = CamPos + new Vector2(SandCore.WIDTH / 2, SandCore.HEIGHT / 2);
            int ofx = (int)(CenterCameraPos.X / 512);
            int ofy = (int)(CenterCameraPos.Y / 512);
            if (CenterCameraPos.X < 0) ofx--;
            if (CenterCameraPos.Y < 0) ofy--;
            for (int i = -2; i < 3; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    Generate((ofx + i) * 512, (ofy + j) * 512, ofx + i, ofy + j);
                }
            }

            // проверка на видимость 
            Rectangle CameraBorders = new Rectangle(CamPos.ToPoint(), new Point(SandCore.WIDTH, SandCore.HEIGHT));
            for (int i = 0; i < Chunks.Count; i++)
            {
                Chunk chunk = Chunks[i];
                Rectangle ChunkBorders = new Rectangle((chunk.Pos - new Vector2(512, 512)).ToPoint(), new Point(1536, 1536));
                // проверяем входит ли чанк в границы камеры, если нет, то удаляем его из отрисовываемых
                if (!CameraBorders.Intersects(ChunkBorders))
                {
                    Block.loadChunks.Remove(chunk.GetName());
                    Chunks.Remove(chunk); 
                }
            }

            base.Update(gameTime);
        }



        // generate chunk
        public void Generate(float _x, float _y, int px, int py)
        {
            if (Chunks.Any(obj => obj.Pos.X == _x && obj.Pos.Y == _y)) // проверяем если такой чанк есть, то не создаем такой же
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
                    if (heights[x, y] == 4 || heights[x, y] == 5)
                        @new.Tiles[x, y, heights[x, y]] = 5;// вода


                    // под землей должны быть камни
                    for (int i = 0; i < heights[x, y]; i++)
                    {
                        @new.Tiles[x, y, i] = 3;
                    }
                }
            }

            // добавляем в рисуемые чанки
            Chunks.Add(@new);

            // генерация деревьев
            string chunkName = @new.GetName();
            Random rand = new Random();
            if (!new DirectoryInfo("maps\\" + SandCore.map + "\\blocks\\" + chunkName).Exists)
            {
                float x = @new.Pos.X + rand.Next(16) * 32;
                float y = @new.Pos.Y + rand.Next(16) * 32;
                Vector2 pos = new Vector2(x, y);
                if (GetTile(pos).ID == 2)
                    Block.CreateBlock("wood", pos);
            }
        }



        // возвращает чанк по координате
        public Chunk GetChunk(float x, float y)
        {
            int ix = (int)(x / 512);
            int iy = (int)(y / 512);
            if (x < 0) ix--;
            if (y < 0) iy--;
            for (int i = 0; i < Chunks.Count; i++)
            {
                if (Chunks[i].Pos == new Vector2(ix * 512, iy * 512))
                    return Chunks[i];
            }

            return Chunks[0];
        }

        // возвращает чанк в котором игрок
        public Chunk GetChunkExistPlayer() => GetChunk(SandCore.hero.Pos.X, SandCore.hero.Pos.Y);

        // возвращает блок на котором стоит игрок
        public int[] GetChunkPosPlayer()
        {
            Tile tile = GetTile(SandCore.hero.Pos);

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
            int x = (int)(MathF.Abs(p1.X / 32));
            int y = (int)(MathF.Abs(p1.Y / 32));

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
    }
}
