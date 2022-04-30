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
        const float TILE_SIZE = 0.04f;
        const float CHUNK_SIZE = TILE_SIZE * 16;

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

        // графика
        private Graphics graphics;


        public Terrain(Game game) : base(game)
        {
            game.Components.Add(this);
            content = game.Content;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            graphics = new Graphics(game.GraphicsDevice);
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
            graphics.Drawing();
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 CamPos = SandCore.camera.Pos; // берем позицию камеры

            //генерация
            //if (Chunks.Count < 17)
            {
                int ofx = (int)(CamPos.X / CHUNK_SIZE );
                int ofy = (int)(CamPos.Y / CHUNK_SIZE);
                if (CamPos.X < 0) ofx--;
                if (CamPos.Y < 0) ofy--;
                for (int i = -2; i < 3; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        Generate((ofx + i) * CHUNK_SIZE, (ofy + j) * CHUNK_SIZE, ofx + i, ofy + j);
                    }
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
                    Block.UnloadChunk(chunk);
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
                        @new.Tiles[x, y, heights[x, y]] = 5;// песок


                    // под землей должны быть камни
                    for (int i = 0; i < heights[x, y]; i++)
                    {
                        @new.Tiles[x, y, i] = 3;
                    }
                }
            }

            GenerateVertex(@new);

            // добавляем в рисуемые чанки
            Chunks.Add(@new);

            // начальная генерация
            string chunkName = @new.GetName();
            Random rand = new Random();
            bool exist = new FileInfo($"maps\\{SandCore.map}\\blocks\\" + chunkName).Exists;
            if (!exist)
            {
                float x = @new.Pos.X + rand.Next(16) * 32;
                float y = @new.Pos.Y + rand.Next(16) * 32;
                Vector2 pos = new Vector2(x, y);
                if (GetTile(pos).ID == 2)
                    Block.CreateBlock("wood", pos);
            } // загрука блоков
            else
            {
                Block.LoadChunks(@new);
            }
        }

        public void GenerateVertex(Chunk chunk)
        {
            int x = (int)(chunk.Pos.X / CHUNK_SIZE);
            int y = (int)(chunk.Pos.Y / CHUNK_SIZE);
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Color color = Color.Black;

                    int id = 0;
                    int height = 0;

                    for (int z = 15; z > -1; z--)
                    {
                        if (chunk.Tiles[i, j, z] != 0)
                        {
                            id = chunk.Tiles[i, j, z];
                            height = z;
                            break;
                        }
                    }

                    if (id == 2)
                        color = Color.Green;
                    if (id == 3)
                        color = Color.Gray;
                    if (id == 4)
                        color = Color.Blue;
                    if (id == 5)
                        color = Color.Yellow;

                    color = new Color(color.R - 100, color.G - 100, color.B - 100);

                    color = new Color(color.R + height * 5, color.G + height * 5, color.B + height * 5);

                    DrawRect((float)(i + x * 16) / (1 / TILE_SIZE), (float)(j + y * 16) / (1 / TILE_SIZE), color);
                }
            }
        }

        private void DrawRect(float x, float y, Color color)
        {
            graphics.Vertices.Add(new VertexPositionColor(new Vector3(x, y, 0), color));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColor(new Vector3(x + TILE_SIZE, y, 0), color));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColor(new Vector3(x + TILE_SIZE, y - TILE_SIZE, 0), color));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColor(new Vector3(x, y - TILE_SIZE, 0), color));
            graphics.Indices.Add(graphics.Vertices.Count - 1);

            graphics.Indices.Add(graphics.Vertices.Count - 4);
            graphics.Indices.Add(-1);
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
