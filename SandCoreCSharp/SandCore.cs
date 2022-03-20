using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Core;
using SandCoreCSharp.Utils;
using System;

namespace SandCoreCSharp
{
    public class SandCore : Game
    {
        public const int WIDTH = 1280;
        public const int HEIGHT = 720;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        static public SpriteFont font;

        // singleton
        static public Game game;

        // main objects
        internal Terrain terrain;
        internal Camera camera;
        internal Hero hero;
        internal Cursor cursor;

        public SandCore()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = WIDTH;
            _graphics.PreferredBackBufferHeight = HEIGHT;
        }

        protected override void Initialize()
        {
            game = this;

            terrain = new Terrain(this);
            camera = new Camera(this);
            hero = new Hero(this, WIDTH / 2 - 16, HEIGHT / 2 - 16, camera);
            cursor = new Cursor(this, hero);

            SimplexNoise.CreateSeed(Convert.ToInt32(ConfigReader.ReadParam("seed")));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            string info = $"[Debug]\n" +
                $"Player Position: [{hero.Pos.X};{hero.Pos.Y};{hero.Height}]\n" +
                $"Camera Position: [{camera.Pos.X};{camera.Pos.Y}]\n" +
                $"Player Chunk: {terrain.GetChunkExistPlayer().GetName()}\n" +
                $"Player Position In Chunk: [{hero.ChunkPos[0]};{hero.ChunkPos[1]};{hero.ChunkPos[2]}]\n" +
                $"Player Block Place: {hero.BlockId}\n" +
                $"Mouse Chunk: {cursor.Chunk.GetName()}\n" +
                $"Mouse Block: {cursor.Block[0]}; {cursor.Block[1]}";

            base.Draw(gameTime);

            _spriteBatch.Begin();
            if(Keyboard.GetState().IsKeyDown(Keys.F3))
                _spriteBatch.DrawString(font, info, new Vector2(0, 0), Color.White);
            _spriteBatch.End();
        }
    }
}
