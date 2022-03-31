﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Core.Blocks;
using System;
using System.Collections.Generic;
using System.IO;

namespace SandCoreCSharp.Core
{
    public class Block : DrawableGameComponent
    {
        const int LoaderDistance = 1024;

        // все блоки
        public static List<Block> Blocks { get; private set; } = new List<Block>();

        // глобальная позиция
        public Vector2 Pos { get; protected set; }

        protected ContentManager content;
        protected SpriteBatch spriteBatch;

        // спрайты
        protected Texture2D sprite;

        // взаимодействия
        protected Hero player;
        protected Camera camera;
        protected Terrain terrain;

        // Коллайдер
        public Rectangle collider;

        // тэг для определения типа (ОБЯЗАТЕЛЬНЫЙ ПРИ СОЗДАНИИ НОВОГО БЛОКА)
        public string Type { get; protected set; } = "block";

        // будет ли блок сохранятся
        public bool isSaving = true;

        // параметры при создании блока
        // прочность блока (сколько секунд будет разрушаться) -параметр-
        public int Hardness { get; protected set; } = 1;
        // имеет ли блок коллизию
        public bool IsSolid { get; protected set; } = true;
        // какой инструмент нужен для добычи // маленькими буквами
        public string Instrument { get; protected set; } = null;

        public Block(Game game, Vector2 pos) : base(game)
        {
            content = Game.Content;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            Game.Components.Add(this);
            Blocks.Add(this);

            SandCore sandCoreGame = (game as SandCore);
            player = sandCoreGame.hero;
            camera = sandCoreGame.camera;
            terrain = sandCoreGame.terrain;

            Pos = pos;

            collider = new Rectangle(Pos.ToPoint(), new Point(32, 32));
        }

        public override void Update(GameTime gameTime)
        {
            Hero hero = SandCore.game.hero;
            Vector2 pos = hero.Pos;
            float r = MathF.Sqrt(MathF.Pow(pos.X - Pos.X, 2) + MathF.Pow(pos.Y - Pos.Y, 2)); // ищем расстояние
            if (r > LoaderDistance)
                Unload();

            base.Update(gameTime);
        }

        // отрисовка спрайта в позиции
        public override void Draw(GameTime gameTime)
        {
            Color color = Color.White;

            Cursor cursor = (Game as SandCore).cursor;
            // добыча блока
            if (cursor.Pos == Pos)
                color = Color.Green;
            // если игрок не может сломать
            if (cursor.Pos == Pos && Mouse.GetState().LeftButton == ButtonState.Pressed)
                color = Color.Black;
            // добыча блока
            if (cursor.Pos == Pos && cursor.breaking)
                color = Color.Red;

            spriteBatch.Begin();
            Vector2 pos = Pos - camera.Pos;
            if (sprite != null)
                spriteBatch.Draw(sprite, pos, color);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // когда блок ломается
        public virtual void Break()
        {
            Resources resources = (Game as SandCore).resources;
            resources.AddResource(Type, 1);

            DeleteBlock();
            Unload();
            Rectangle collider = new Rectangle(Pos.ToPoint(), new Point(32, 32));
        }

        // столкновение с игроком
        public virtual void CollidePlayer(Hero player)
        {}

        // чанк в котором блок
        public Chunk GetChunk() => terrain.GetChunk(Pos.X, Pos.Y);

        // раньше было методом break, но этот метод един для всех блоков
        public void Unload()
        {
            Game.Components.Remove(this);
            Blocks.Remove(this);
        }

        // сохранение блока
        public void SaveBlock()
        {
            string data = $".{Pos.X}.{Pos.Y}.{Type}";
            File.Create("maps\\" + SandCore.map + "\\blocks\\" + data);
        }
        
        // удаление этого блока
        public void DeleteBlock()
        {
            try
            {
                string data = $".{Pos.X}.{Pos.Y}.{Type}";
                File.Delete("maps\\" + SandCore.map + "\\blocks\\" + data);
            }
            catch { }
        }
        



        // загружает блоки
        static public void LoadBlocks()
        {
            Hero hero = SandCore.game.hero;
            Vector2 pos = hero.Pos;
            
            // получаем все файлы с блоками
            string[] files = Directory.GetFiles("maps\\" + SandCore.map + "\\blocks");
            // проходимся по всем файлам
            for (int i = 0; i < files.Length; i++)
            {
                // получаем полноценную инфу о блоке
                string file = files[i];
                string[] data = file.Split('.');
                int x = Convert.ToInt32(data[1]);
                int y = Convert.ToInt32(data[2]);
                string type = data[3];

                // проверяем расстояние
                float r = MathF.Sqrt(MathF.Pow(pos.X - x, 2) + MathF.Pow(pos.Y - y, 2)); // ищем расстояние
                if (r < LoaderDistance) // если оно меньше указанного, то создаем блок
                    CreateBlock(type, new Vector2(x, y), true);
            }
        }

        // регистрирует новые блоки (с параметром isSaving = true)
        static public void CreateBlock(string type, Vector2 pos, bool loader = false)
        {
            // проходим и проверяем, если там уже стоит блок, то новый не создаем
            for (int i = 0; i < Blocks.Count; i++)
            {
                Block b = Blocks[i];
                if (b.Pos == pos)
                    return;
            }

            Block block = null;

            if (type == "wood")
                block = new Wood(SandCore.game, pos);
            if (type == "furnace")
                block = new Furnace(SandCore.game, pos);
            if (type == "mine")
                block = new Mine(SandCore.game, pos);
            if (type == "lumberjack")
                block = new Lumberjack(SandCore.game, pos);
            if (type == "battery")
                block = new Battery(SandCore.game, pos);
            if (type == "coal_generator")
                block = new CoalGenerator(SandCore.game, pos);
            if (type == "quarry")
                block = new Quarry(SandCore.game, pos);
            if (type == "induction_furnace")
                block = new InductionFurnace(SandCore.game, pos);

            if (!loader)
                block.SaveBlock();
        }


        // ищет блок по позиции
        static public Block FindBlock(Vector2 pos) => Blocks.Find(block => block.Pos == pos);
    }
}
