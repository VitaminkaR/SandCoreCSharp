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
        // позиция игрока
        public Vector2 Pos { get; private set; }
        public int Height { get; private set; }

        // скорость игрока
        private float speed = 0;

        // спрайт игрока
        private Texture2D texture;

        // камера
        Camera camera;
        // офсет для камеры
        Vector2 offset;

        // мировые параметры
        // блок на котором стоит персонаж
        public byte BlockId { get; private set; }
        // позиция в чанке
        public int[] ChunkPos { get; private set; }
        // сам чанк
        public Chunk Chunk { get; private set; }

        // здоровье
        public float Health { get; private set; }

        // механизмы с которыми рядом игрок
        // механизмы - блоки, они сами добавляются в список (свой тег блока), когда к ним подошел игрок
        // нужно для "требуемых" в крафте
        public List<string> Mechanisms { get; set; }


        private ContentManager content;
        private SpriteBatch spriteBatch;

        public Hero(Game game, float x, float y, Camera _camera) : base(game)
        {
            game.Components.Add(this);
            content = game.Content;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            Pos = new Vector2(x, y);
            camera = _camera;
            DrawOrder = 1;
        }

        public override void Initialize()
        {
            Mechanisms = new List<string>();
            Mechanisms.Add("");

            speed = 3;
            offset = Pos;
            Health = 100;

            Load();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = content.Load<Texture2D>("Hero");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Control();
            CameraUpdate();

            Terrain terrain = (Game as SandCore).terrain;
            Chunk = terrain.GetChunkExistPlayer();
            ChunkPos = terrain.GetChunkPosPlayer();
            BlockId = terrain.GetBlockIdPlayerPlace(Chunk, ChunkPos);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, -camera.Pos + Pos, new Color(40 + Height * 13, 40 + Height * 13, 40 + Height * 13)); // отрисовываем относительно камеры

            // ui
            spriteBatch.DrawString(SandCore.font, Health.ToString(), new Vector2(32, SandCore.HEIGHT - 64), Color.DarkRed, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
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
                speed = 5;
            else
                speed = 3;

            PlayerInWater();

            //  движение
            if (ks.IsKeyDown(Keys.W) && CheckCollison(new Vector2(0, -speed)))
                Pos += new Vector2(0, -speed);
            if (ks.IsKeyDown(Keys.S) && CheckCollison(new Vector2(0, speed)))
                Pos += new Vector2(0, speed);
            if (ks.IsKeyDown(Keys.D) && CheckCollison(new Vector2(speed, 0)))
                Pos += new Vector2(speed, 0);
            if (ks.IsKeyDown(Keys.A) && CheckCollison(new Vector2(-speed, 0)))
                Pos += new Vector2(-speed, 0);
        }

        // присоединение камеры
        private void CameraUpdate()
        {
            camera.Pos = Pos - offset;
        }

        // проверка на воду
        private void PlayerInWater()
        {
            if (BlockId == 4)
                speed -= 2;
        }

        // проверка на столкновения true - если столкновений нет
        private bool CheckCollison(Vector2 direction)
        {
            Rectangle collider = new Rectangle((Pos + direction).ToPoint(), new Point(32, 32));
            for (int i = 0; i < Block.Blocks.Count; i++)
            {
                Block block = Block.Blocks[i];
                if (collider.Intersects(block.collider) && block.IsSolid)
                {
                    block.CollidePlayer(this);
                    return false;
                }
            }
            return true;
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

                        int x = Convert.ToInt32(line.Split('|')[0]);
                        int y = Convert.ToInt32(line.Split('|')[1]);
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
