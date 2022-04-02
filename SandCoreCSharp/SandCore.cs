using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Core;
using SandCoreCSharp.Core.Blocks;
using SandCoreCSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;

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
        static public SandCore game;

        // main objects
        internal Terrain terrain;
        internal Camera camera;
        internal Hero hero;
        internal Cursor cursor;
        internal Resources resources;
        internal Inventory inventory;
        internal CraftManager craftManager;

        // controls
        private bool blocking;
        private bool debugVars;
        public static bool debugChunks;

        // map loading
        static public string map;

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
            resources = new Resources(this);
            inventory = new Inventory(this);
            craftManager = new CraftManager(this);
            

            SimplexNoise.CreateSeed(Convert.ToInt32(ConfigReader.ReadParam("options.cfg", "seed")));

            // СОХРАНЕНИЯ
            map = ConfigReader.ReadParam("options.cfg", "map");
            // проверка существует ли директория с картой
            if (!Directory.Exists("maps\\" + map))
                Directory.CreateDirectory("maps\\" + SandCore.map);
            // проверка существует ли директория с чанками
            if (!Directory.Exists("maps\\" + map + "\\blocks"))
                Directory.CreateDirectory("maps\\" + map + "\\blocks");

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
            {
                Exit();
            }

            // нажатие клавиш
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.F3) && !blocking)
            {
                debugVars = !debugVars;
                blocking = true;
            }
            if (ks.IsKeyDown(Keys.F4) && !blocking)
            {
                debugChunks = !debugChunks;
                blocking = true;
            }
            if (ks.GetPressedKeyCount() == 0)
                blocking = false;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // эти методы в draw, потому что draw вызывается меньшее кол-во раз, чем update,
            // для этих методов каждый кадр не играет роль
            // загрузка
            Block.LoadBlocks(terrain);

            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);

            if (debugVars)
            {
                string info = $"[Debug]\n" +
                    $"Player Position: [{hero.Pos.X};{hero.Pos.Y};{hero.Height}]\n" +
                    $"Camera Position: [{camera.Pos.X};{camera.Pos.Y}]\n\n" +
                    $"Player Chunk: {terrain.GetChunkExistPlayer().GetName()}\n" +
                    $"Player Position In Chunk: [{hero.ChunkPos[0]};{hero.ChunkPos[1]};{hero.ChunkPos[2]}]\n" +
                    $"Player Block Place: {hero.BlockId}\n\n" +
                    $"Mouse Chunk: {cursor.Chunk.GetName()}\n" +
                    $"Mouse Tile ID: {cursor.Tile.ID}\n" +
                    $"Mouse Block: {cursor.Tile.Position[0]}; {cursor.Tile.Position[1]}\n\n";
                _spriteBatch.Begin();
                _spriteBatch.DrawString(font, info, new Vector2(0, 0), Color.White);
                _spriteBatch.End();
            }
        }
    }
}
