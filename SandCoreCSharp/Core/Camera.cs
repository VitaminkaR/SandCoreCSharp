using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SandCoreCSharp.Core
{
    public class Camera : GameComponent
    {
        public Vector2 Pos { get; internal set; } // позиция камеры
        public Vector2 Borders { get; private set; } // границы для проверки входит ли чанк в них

        private Vector2 borderSize; // размеры экрана (константы экрана из класса игры)

        private float speed; // скорость перемещения в пикселях



        public Camera(Game game) : base(game) => game.Components.Add(this);



        public override void Initialize()
        {
            borderSize = new Vector2(SandCore.WIDTH, SandCore.HEIGHT);

            Pos = new Vector2();
            Borders = Pos + borderSize;       

            speed = 10;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // меняем границы для новых позиций
            Borders = Pos + borderSize;

            //Control();

            base.Update(gameTime);
        }

        // управление камерой (пока нет персонажа)
        private void Control()
        {
            KeyboardState ks = Keyboard.GetState();

            if (ks.GetPressedKeys().Length == 0) // если клавиши не нажаты, то далее не проверяем
                return;

            if (ks.IsKeyDown(Keys.W))
                Pos += new Vector2(0, -speed);
            if (ks.IsKeyDown(Keys.S))
                Pos += new Vector2(0, speed);
            if (ks.IsKeyDown(Keys.D))
                Pos += new Vector2(speed, 0);
            if (ks.IsKeyDown(Keys.A))
                Pos += new Vector2(-speed, 0);
        }
    }
}
