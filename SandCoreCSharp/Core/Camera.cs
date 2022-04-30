using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SandCoreCSharp.Core
{
    public class Camera : GameComponent
    {
        public Matrix worldMatrix { get; private set; }
        public Matrix viewMatrix { get; private set; }
        public Matrix projectionMatrix { get; private set; }

        private Vector2 pos;
        public Vector2 Pos 
        {
            get => pos;
            set
            {
                Vector2 offset = value - pos;
                pos = value;
                worldMatrix *= Matrix.CreateTranslation(offset.X, offset.Y, 0);
            }
        }
        private float speed; // скорость перемещения в пикселях



        public Camera(Game game) : base(game) => game.Components.Add(this);



        public override void Initialize()
        {
            Pos = new Vector2();
            speed = 0.005f;

            worldMatrix = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),  // 45 degree angle
                (float)Game.GraphicsDevice.Viewport.Width /
                (float)Game.GraphicsDevice.Viewport.Height,
                1.0f, 100.0f);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 1 + Mouse.GetState().ScrollWheelValue / -100), Vector3.Zero, Vector3.Up);

            Hero hero = SandCore.hero;

            base.Update(gameTime);
        }

        // debug
        private void ControlCamera()
        {
            KeyboardState ks = Keyboard.GetState();

            if (ks.GetPressedKeys().Length == 0) // если клавиши не нажаты, то далее не проверяем
                return;

            if (ks.IsKeyDown(Keys.W))
                Pos += new Vector2(0, -speed);
            if (ks.IsKeyDown(Keys.S))
                Pos += new Vector2(0, speed);
            if (ks.IsKeyDown(Keys.D))
                Pos += new Vector2(-speed, 0);
            if (ks.IsKeyDown(Keys.A))
                Pos += new Vector2(speed, 0);
        }
    }
}
