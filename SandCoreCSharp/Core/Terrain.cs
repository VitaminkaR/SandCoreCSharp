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
        public const float TILE_SIZE = 0.04f;
        public const float CHUNK_SIZE = TILE_SIZE * 16;

        private ContentManager content;

        // view chunks
        public List<Chunk> Chunks { get; private set; }

        // графика
        private Graphics graphics;


        public Terrain(Game game) : base(game)
        {
            game.Components.Add(this);
            content = game.Content;
            graphics = new Graphics(game.GraphicsDevice);
        }


        // init
        public override void Initialize()
        {
            Chunks = new List<Chunk>();

            base.Initialize();
        }

        // load sprites (ADD SPRITES)
        protected override void LoadContent()
        {
            graphics.Texture = content.Load<Texture2D>("tile");

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
            if (Chunks.Count < 40)
                PredGenerate();

            // проверка на видимость 
            Vector2 ca = CamPos - new Vector2(2.5f, 2.5f);
            bool removes = false;
            for (int i = 0; i < Chunks.Count; i++)
            {
                Chunk chunk = Chunks[i];
                Vector2 pos = chunk.Pos;
                // проверяем входит ли чанк в границы камеры, если нет, то удаляем его из отрисовываемых
                if (!(pos.X >= ca.X && pos.X <= ca.X + 5 && pos.Y >= ca.Y && pos.Y <= ca.Y + 5))
                {
                    Block.UnloadChunk(chunk);
                    removes = true;
                    Chunks.Remove(chunk);
                }
            }
            if (removes)
                DeleteVertex();

            base.Update(gameTime);
        }



        private void PredGenerate()
        {
            Vector2 CamPos = SandCore.camera.Pos; // берем позицию камеры
            int ofx = (int)(CamPos.X / CHUNK_SIZE);
            int ofy = (int)(CamPos.Y / CHUNK_SIZE);
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    Generate((ofx + i) * CHUNK_SIZE, (ofy + j) * CHUNK_SIZE, ofx + i, ofy + j);
                }
            }
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
                float x = @new.Pos.X + rand.Next(16) * TILE_SIZE;
                float y = @new.Pos.Y + rand.Next(16) * TILE_SIZE;
                Vector2 pos = new Vector2(x, y);
                //if (GetTile(pos).ID == 2)
                    //Block.CreateBlock("wood", pos, true);
            } // загрука блоков
            else
            {
                //Block.LoadChunks(@new);
            }
        }

        // отрисовывает чанк
        private void GenerateVertex(Chunk chunk)
        {
            float x = chunk.Pos.X / CHUNK_SIZE;
            float y = chunk.Pos.Y / CHUNK_SIZE;

            Color bdColor = Color.Black;
            if (SandCore.debugChunks)
            {
                Random r = new Random();
                bdColor = new Color(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
            }

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

                    // затемнение
                    color = new Color(color.R - 100, color.G - 100, color.B - 100);
                    color = new Color(color.R + height * 5, color.G + height * 5, color.B + height * 5);

                    if (SandCore.debugChunks)
                        color = bdColor;

                    DrawRect((float)(i + x * 16) / (1 / TILE_SIZE), (float)(j + y * 16) / (1 / TILE_SIZE), color);
                }
            }
        }

        // рисует квадрат
        private void DrawRect(float x, float y, Color color)
        {
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x, y, 0), color, new Vector2(0, 0)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x + TILE_SIZE, y, 0), color, new Vector2(1, 0)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x + TILE_SIZE, y - TILE_SIZE, 0), color, new Vector2(1, 1)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x, y - TILE_SIZE, 0), color, new Vector2(0, 1)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);

            graphics.Indices.Add(graphics.Vertices.Count - 4);
            graphics.Indices.Add(-1);
        }
        
        // удаляет графику чанков (перерисовка чанков)
        private void DeleteVertex()
        {
            graphics.Vertices = new List<VertexPositionColorTexture>();
            graphics.Indices = new List<int>();

            for (int i = 0; i < Chunks.Count; i++)
            {
                GenerateVertex(Chunks[i]);
            }
        }




        // методы для получения чанков или тайлов

        // найти чанк по позиции
        public Chunk GetChunk(Vector2 pos) => 
            Chunks.Find((Chunk chunk) => chunk.Pos.X <= pos.X && chunk.Pos.X + CHUNK_SIZE >= pos.X && chunk.Pos.Y <= pos.Y && chunk.Pos.Y + CHUNK_SIZE >= pos.Y);

        // найти тайл по позиции
        public Tile GetTile(Vector2 pos)
        {
            Chunk chunk = GetChunk(pos);
            int x = 0;
            int y = 0;
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    float tx = i * TILE_SIZE;
                    float ty = j * TILE_SIZE;
                    Vector2 tp = new Vector2(tx, ty) + chunk.Pos;
                    if(tp.X <= pos.X && tp.X + TILE_SIZE >= pos.X && tp.Y <= pos.Y && tp.Y + TILE_SIZE >= pos.Y)
                    {
                        x = i;
                        y = j;
                    }
                }
            }
            return new Tile(x, y, chunk, 0);
        }
    }
}
