using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace SandCoreCSharp.Core
{
    class Hero : DrawableGameComponent
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

        private ContentManager content;
        private SpriteBatch spriteBatch;

        public Hero(Game game, float x, float y, Camera _camera) : base(game)
        {
            game.Components.Add(this);
            content = game.Content;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            Pos = new Vector2(x, y);
            camera = _camera;
        }

        public override void Initialize()
        {
            speed = 3;
            offset = Pos;

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

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, -camera.Pos + Pos, new Color(40 + Height * 13, 40 + Height * 13, 40 + Height * 13)); // отрисовываем относительно камеры
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // управление игроком
        private void Control()
        {
            // измерение высоты
            Height = (Game as SandCore).terrain.GetBlockPlayerPlace()[2];

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
            if (ks.IsKeyDown(Keys.W))
                Pos += new Vector2(0, -speed);
            if (ks.IsKeyDown(Keys.S))
                Pos += new Vector2(0, speed);
            if (ks.IsKeyDown(Keys.D))
                Pos += new Vector2(speed, 0);
            if (ks.IsKeyDown(Keys.A))
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
            Terrain terrain = (Game as SandCore).terrain;
            byte id = terrain.GetBlockIdPlayerPlace();
            if (id == 4)
                speed -= 2;
        }
    }
}
