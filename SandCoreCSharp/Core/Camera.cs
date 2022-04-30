using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SandCoreCSharp.Core
{
    public class Camera : GameComponent
    {
        public Vector2 Pos { get; internal set; } // позиция камеры
        public Matrix WorldPos { get; private set; } // графическая позиция камеры
        private float speed; // скорость перемещения в пикселях



        public Camera(Game game) : base(game) => game.Components.Add(this);



        public override void Initialize()
        {
            Pos = new Vector2();
            speed = 0.005f;
            WorldPos = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();

            if (ks.GetPressedKeys().Length == 0) // если клавиши не нажаты, то далее не проверяем
                return;

            if (ks.IsKeyDown(Keys.W))
                WorldPos *= Matrix.CreateTranslation(0, -speed, 0);
            if (ks.IsKeyDown(Keys.S))
                WorldPos *= Matrix.CreateTranslation(0, speed, 0);
            if (ks.IsKeyDown(Keys.D))
                WorldPos *= Matrix.CreateTranslation(-speed, 0, 0);
            if (ks.IsKeyDown(Keys.A))
                WorldPos *= Matrix.CreateTranslation(speed, 0, 0);

            base.Update(gameTime);
        }
    }
}
