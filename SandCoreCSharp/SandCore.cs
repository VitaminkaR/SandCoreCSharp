﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Core;
using SandCoreCSharp.Utils;
using System;
using System.Collections.Generic;

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

        // controls
        private bool blocking;
        private bool debugVars;
        public static bool debugChunks;

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

            SimplexNoise.CreateSeed(Convert.ToInt32(ConfigReader.ReadParam("options.cfg", "seed")));

            SimpleTimer timer = new SimpleTimer(500, 
            (object obj) =>
            { 
                // загрузка карты
                string mapName = ConfigReader.ReadParam("options.cfg", "map"); // имя карты
                MapSaver.LoadMap(mapName, this);
            }, 
            null);

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
                SaveMap();
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
                    $"Player Position: [{hero.Pos.X};{hero.Pos.Y};{hero.Height}]\n" +
                    $"Camera Position: [{camera.Pos.X};{camera.Pos.Y}]\n" +
                    $"Player Chunk: {terrain.GetChunkExistPlayer().GetName()}\n" +
                    $"Player Position In Chunk: [{hero.ChunkPos[0]};{hero.ChunkPos[1]};{hero.ChunkPos[2]}]\n" +
                    $"Player Block Place: {hero.BlockId}\n" +
                    $"Mouse Chunk: {cursor.Chunk.GetName()}\n" +
                    $"Mouse Block: {cursor.Tile.Position[0]}; {cursor.Tile.Position[1]}";
                _spriteBatch.Begin();
                _spriteBatch.DrawString(font, info, new Vector2(0, 0), Color.White);
                _spriteBatch.End();
            }
        }

        private void SaveMap()
        {
            string mapName = ConfigReader.ReadParam("options.cfg", "map"); // имя карты

            // сохраняем карту
            MapSaver.SaveMap(mapName, this);
        }
    }
}
