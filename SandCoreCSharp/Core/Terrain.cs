using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;
using SandCoreCSharp.Utils;

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
        
        // возвращает чанк
        public Chunk GetChunk(float x, float y)
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                Vector2 pos = chunks[i].Pos;
                if (x > pos.X && x < pos.X + 512
                    && y > pos.Y && y < pos.Y + 512)
                    return chunks[i];
            }
            return null;
        }

        // возвращает чанк в котором игрок
        public Chunk GetChunkExistPlayer()
        {
            Hero hero = (Game as SandCore).hero; // получаем игрока
            Vector2 pos = hero.Pos; // находим его позицию (для удобства) 
            int ox = (int)(pos.X / 512); // чанк по x
            int oy = (int)(pos.Y / 512); // чанк по y

            // для правильного вычисления отрицательных чанков
            if (pos.X < 0) 
                ox -= 1;
            if (pos.Y < 0)
                oy -= 1;

            // находим этот чанк и возвращаем
            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].Pos.X == ox * 512 && chunks[i].Pos.Y == oy * 512)
                    return chunks[i];
            }
            return null;
        }

        // возвращает блок на котором стоит игрок
        public int[] GetChunkPosPlayer(Chunk chunk)
        {
            Hero hero = (Game as SandCore).hero; // получаем игрока
            Vector2 pos = hero.Pos; // находим его позицию (для удобства) 
            int ox = (int)((pos.X - chunk.Pos.X) / 32); // блок по x
            int oy = (int)((pos.Y - chunk.Pos.Y) / 32); // блок по y

            int oz = 0;

            if(ox > 15 || oy > 15) // для фильтра ошибок
                return new int[] { 0, 0, 0 };

            for (int i = 15; i > -1; i--)
            {
                if(chunk.Tiles[ox, oy, i] != 0)
                {
                    oz = i;
                    break;
                }
            }

            return new int[] { ox, oy, oz };
        }

        // возвращает блок на котором стоит игрок
        public byte GetBlockIdPlayerPlace(Chunk chunk, int[] pos) => chunk.Tiles[pos[0], pos[1], pos[2]];
    }
}
