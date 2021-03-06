using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System;
using System.Collections.Generic;

namespace SandCoreCSharp.Core
{
    public class Hero : DrawableGameComponent
    {
        const float PLAYER_SIZE = Terrain.TILE_SIZE;

        // позиция игрока
        public Vector2 Pos { get; private set; }
        public int Height { get; private set; }

        // скорость игрока
        private float speed = 0;

        // спрайт игрока
        private Texture2D texture;
        private Graphics graphics;

        // камера
        Camera camera;

        // мировые параметры
        // тайл на котором стоит персонаж
        public Tile Tile { get; private set; }
        // позиция в чанке
        public int[] ChunkPos { get; private set; }
        // сам чанк
        public Chunk Chunk { get; private set; }

        // здоровье
        public float Health { get; private set; }

        private ContentManager content;
        private SpriteBatch spriteBatch;

        public Hero(Game game, float x, float y, Camera _camera) : base(game)
        {
            game.Components.Add(this);
            content = game.Content;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            Pos = new Vector2(x, y);
            camera = _camera;
            graphics = new Graphics(game.GraphicsDevice);
        }

        public override void Initialize()
        {
            Health = 100;
            Load();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = content.Load<Texture2D>("tile");
            graphics.Texture = texture;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Control();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Terrain terrain = SandCore.terrain;
            Tile = terrain.GetTile(Pos);
            Chunk = Tile.Chunk;

            DrawRect(Pos.X, Pos.Y);
            graphics.Drawing();

            // ui
            spriteBatch.Begin();
            spriteBatch.DrawString(SandCore.font, Health.ToString(), new Vector2(32, SandCore.HEIGHT - 64), Color.DarkRed, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawRect(float x, float y)
        {
            graphics.Vertices = new List<VertexPositionColorTexture>();
            graphics.Indices = new List<int>();

            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x, y, 0), Color.Red, new Vector2(0, 0)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x + PLAYER_SIZE, y, 0), Color.Red, new Vector2(1, 0)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x + PLAYER_SIZE, y - PLAYER_SIZE, 0), Color.Red, new Vector2(1, 1)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x, y - PLAYER_SIZE, 0), Color.Red, new Vector2(0, 1)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);

            graphics.Indices.Add(graphics.Vertices.Count - 4);
            graphics.Indices.Add(-1);
        }

        // управление игроком
        private void Control()
        {
            // измерение высоты
            if (ChunkPos != null)
                Height = ChunkPos[2];

            KeyboardState ks = Keyboard.GetState();

            if (ks.GetPressedKeys().Length == 0) // если клавиши не нажаты, то далее не проверяем
                return;

            // бег
            if (ks.IsKeyDown(Keys.LeftShift))
                speed = 0.01f;
            else
                speed = 0.005f;

            //  движение
            if (ks.IsKeyDown(Keys.W) && CheckCollisonWithBlocks(new Vector2(0, speed)))
                Pos += new Vector2(0, speed);
            if (ks.IsKeyDown(Keys.S) && CheckCollisonWithBlocks(new Vector2(0, -speed)))
                Pos += new Vector2(0, -speed);
            if (ks.IsKeyDown(Keys.D) && CheckCollisonWithBlocks(new Vector2(speed, 0)))
                Pos += new Vector2(speed, 0);
            if (ks.IsKeyDown(Keys.A) && CheckCollisonWithBlocks(new Vector2(-speed, 0)))
                Pos += new Vector2(-speed, 0);

            camera.Pos = Pos;
        }

        // проверка на столкновения true - если столкновений нет
        private bool CheckCollisonWithBlocks(Vector2 direction)
        {
            for (int i = 0; i < Block.Blocks.Count; i++)
            {
                Block block = Block.Blocks[i];
                Vector2 pos = Pos + direction;
                if ((pos.X >= block.Pos.X && pos.X <= block.Pos.X + PLAYER_SIZE && pos.Y >= block.Pos.Y && pos.Y <= block.Pos.Y + PLAYER_SIZE ||
                    pos.X + PLAYER_SIZE >= block.Pos.X && pos.X + PLAYER_SIZE <= block.Pos.X + PLAYER_SIZE && pos.Y + PLAYER_SIZE >= block.Pos.Y && pos.Y + PLAYER_SIZE <= block.Pos.Y + PLAYER_SIZE ||
                    pos.X >= block.Pos.X && pos.X <= block.Pos.X + PLAYER_SIZE && pos.Y + PLAYER_SIZE >= block.Pos.Y && pos.Y + PLAYER_SIZE <= block.Pos.Y + PLAYER_SIZE ||
                    pos.X + PLAYER_SIZE >= block.Pos.X && pos.X + PLAYER_SIZE <= block.Pos.X + PLAYER_SIZE && pos.Y >= block.Pos.Y && pos.Y <= block.Pos.Y + PLAYER_SIZE) &&
                    block.IsSolid)
                {
                    block.CollidePlayer(this);
                    return false;
                }
            }
            return true;
        }

        // проверяет на столкновение
        public bool Collision(Vector2 _pos)
        {
            if (Pos.X >= _pos.X && Pos.X <= _pos.X + PLAYER_SIZE && Pos.Y >= _pos.Y && Pos.Y <= _pos.Y + PLAYER_SIZE ||
                    Pos.X + PLAYER_SIZE >= _pos.X && Pos.X + PLAYER_SIZE <= _pos.X + PLAYER_SIZE && Pos.Y + PLAYER_SIZE >= _pos.Y && Pos.Y + PLAYER_SIZE <= _pos.Y + PLAYER_SIZE ||
                    Pos.X >= _pos.X && Pos.X <= _pos.X + PLAYER_SIZE && Pos.Y + PLAYER_SIZE >= _pos.Y && Pos.Y + PLAYER_SIZE <= _pos.Y + PLAYER_SIZE ||
                    Pos.X + PLAYER_SIZE >= _pos.X && Pos.X + PLAYER_SIZE <= _pos.X + PLAYER_SIZE && Pos.Y >= _pos.Y && Pos.Y <= _pos.Y + PLAYER_SIZE)
            {
                return true;
            }
            return false;
        }

        // при загрузке карты
        public void SetPos(Vector2 pos) => Pos = pos;



        // сохранения и загрузка позиция
        // сохраняет инфу о позиции
        private void Save()
        {
            string data = Pos.X.ToString() + '|' + Pos.Y.ToString();

            using (StreamWriter sr = new StreamWriter("maps\\" + SandCore.map + "\\player_position"))
            {
                sr.Write(data);
            }
        }

        // загружает позицию
        private void Load()
        {
            if (new FileInfo("maps\\" + SandCore.map + "\\player_position").Exists)
            {
                using (StreamReader sr = new StreamReader("maps\\" + SandCore.map + "\\player_position"))
                {
                    string line = "";
                    while (true)
                    {
                        line = sr.ReadLine();

                        if (line == null)
                            break;

                        float x = (float)Convert.ToDouble(line.Split('|')[0]);
                        float y = (float)Convert.ToDouble(line.Split('|')[1]);
                        Pos = new Vector2(x, y);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            Save();

            base.Dispose(disposing);
        }
    }
}
