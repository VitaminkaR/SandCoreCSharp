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
        static internal Terrain terrain;
        static internal Camera camera;
        static internal Hero hero;
        static internal Cursor cursor;
        static internal Resources resources;
        static internal Inventory inventory;
        static internal CraftManager craftManager;

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

            // СОХРАНЕНИЯ
            map = ConfigReader.ReadParam("options.cfg", "map");
            // проверка существует ли директория с картой
            if (!Directory.Exists("maps\\" + map))
                Directory.CreateDirectory("maps\\" + SandCore.map);
            // проверка существует ли директория с чанками
            if (!Directory.Exists("maps\\" + map + "\\blocks"))
                Directory.CreateDirectory("maps\\" + map + "\\blocks");

            terrain = new Terrain(this);
            camera = new Camera(this);
            hero = new Hero(this, 0, 0, camera);
            cursor = new Cursor(this, hero);
            resources = new Resources(this);
            inventory = new Inventory(this);
            craftManager = new CraftManager(this);

            Logger.LogTitle($"SandCore# World: {map}", this);

            SimplexNoise.CreateSeed(Convert.ToInt32(ConfigReader.ReadParam("options.cfg", "seed")));  

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
            Block.LoadContents(Content);
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
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);

            if (debugVars)
            {
                string info = $"[Debug]\n" +
                    $"FPS: {1 / gameTime.ElapsedGameTime.TotalSeconds}\n\n" +
                    $"Player Position: [{hero.Pos.X};{hero.Pos.Y};{hero.Height}]\n" +
                    $"Camera Position: [{camera.Pos.X};{camera.Pos.Y}]\n\n" +
                    $"Player Chunk: [{hero.Chunk?.GetName()}]\n" +
                    $"PlayerTile: [{hero.Tile.Position[0]};{hero.Tile.Position[1]}]" +
                    $"Chunks: {terrain.Chunks.Count}\n";
                _spriteBatch.Begin();
                _spriteBatch.DrawString(font, info, new Vector2(0, 0), Color.White);
                _spriteBatch.End();
            }
        }

        protected override void Dispose(bool disposing)
        {
            //Logger.MessageBox(0, "Saving world...", "SandCore#", 0);

            base.Dispose(disposing);
        }
    }
}
