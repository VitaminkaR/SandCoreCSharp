using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;
using SandCoreCSharp.Utils;
using System.Diagnostics;

namespace SandCoreCSharp.Core
{
    // класс, генерирующий и отрисовывающий чанки
    class Terrain : DrawableGameComponent
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
                            if(SandCore.debugChunks && (x == 0 || x == 15 || y == 0 || y == 15))
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
                if (chunk.Pos.X > camBor.X + 600 || chunk.Pos.Y > camBor.Y + 600 ||
                    (chunk.Pos.X + 512) < camPos.X - 600 || (chunk.Pos.Y + 512) < camPos.Y - 600)
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
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    // изменение блоков от высоты
                    if (heights[x, y] > 14)
                    {
                        @new.Tiles[x, y, heights[x, y]] = 3;
                        continue;
                    }  // горы
                    if (heights[x, y] < 4)
                    {
                        @new.Tiles[x, y, heights[x, y]] = 4;
                        continue;
                    } // вода
                    @new.Tiles[x, y, heights[x, y]] = 2; // земля

                    // под землей должны быть камушки
                    for (int i = 0; i < heights[x, y]; i++)
                    {
                        @new.Tiles[x, y, i] = 3;
                    }
                }
            }

            // добавляем в рисуемые чанки (потому все с камерой будет связано)
            chunks.Add(@new);
        }

        // возвращает чанк по координате
        public Chunk GetChunk(float x, float y)
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                Vector2 pos = chunks[i].Pos;
                if (x >= pos.X && x <= pos.X + 512
                    && y >= pos.Y && y <= pos.Y + 512)
                    return chunks[i];
            }
            return chunks[0];
        }

        // возвращает чанк по индексу
        public Chunk GetChunk(int ix, int iy)
        {
            float x = ix * 512;
            float y = iy * 512;
            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].Pos == new Vector2(x, y))
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

        // возвращает  id блока на котором стоит игрок
        public byte GetBlockIdPlayerPlace(Chunk chunk, int[] pos) => chunk.Tiles[pos[0], pos[1], pos[2]];

        // возвращает блок по позиции
        public Tile GetTile(Vector2 pos)
        {
            int tx = (int)(pos.X / 32);
            int ty = (int)(pos.Y / 32);
            return GetTile(tx, ty);
        }

        // возвращает блок по индексу (система для удобного взаимодейсвтия  блоками)
        public Tile GetTile(int ix, int iy)
        {
            int[] chunk = new int[2] { ix / 16, iy / 16 };
            int x = 0;
            int y = 0;
            if (ix >= 0) x = ix - chunk[0] * 16;
            if (iy >= 0) y = iy - chunk[1] * 16;
            if (ix < 0) x = (ix - 1) - (chunk[0] - 1) * 16;
            if (iy < 0) y = (iy - 1) - (chunk[1] - 1) * 16;
            if (x > 15 || y > 15 || x < 0 || y < 0)
                throw new System.Exception();
            return new Tile(x, y, GetChunk(chunk[0], chunk[1]));
        }
    }
}
