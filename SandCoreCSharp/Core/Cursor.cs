using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Core.Blocks;
using SandCoreCSharp.Utils;

namespace SandCoreCSharp.Core
{
    // этот класс позволяет игроку взаимодействовать с блоками 
    class Cursor : ViewGameObject
    {
        private Hero player; // для взаимодействия

        public bool Active { get; private set; } // активный ли курсор

        // блок на который наведена мышь [x, y, z]
        public Tile Tile { get; private set; }
        // чанк в котором мышка
        public Chunk Chunk { get; private set; }

        // блок мыши
        private bool mouseBlock;

        // для ломания блока
        public bool breaking;


        public Cursor(Game game, Hero hero) : base(game)
        {
            player = hero;
        }


        public override void Initialize()
        {
            Pos = new Vector2();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = content.Load<Texture2D>("Cursor");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Camera camera = (Game as SandCore).camera;
            Terrain terrain = (Game as SandCore).terrain;
            MouseState state = Mouse.GetState();

            Vector2 pos = camera.Pos + state.Position.ToVector2();

            // ищем на какой блок навестись
            Chunk chunk = terrain.GetChunk(pos.X, pos.Y);
            if (chunk != null)
            {
                this.Chunk = chunk;
                // ищем блок
                Tile = terrain.GetTile(pos);
                Pos = (new Vector2(Tile.Position[0] * 32, Tile.Position[1] * 32) + Chunk.Pos);
            }


            if (state.LeftButton == ButtonState.Pressed && !mouseBlock && Active)
            {
                mouseBlock = true;
                Break();
            }
            if (state.RightButton == ButtonState.Pressed && !mouseBlock && Active)
            {
                mouseBlock = true;
                Use();
            }
            if (state.RightButton == ButtonState.Released && state.LeftButton == ButtonState.Released)
            {
                mouseBlock = false;
                breaking = false; // если отжали кнопку, то перестали ломать блок
            }



            // проверка на расстояние от игрока
            Vector2 playerPos = player.Pos;
            float r = MathF.Sqrt(MathF.Pow(playerPos.X - pos.X, 2) + MathF.Pow(playerPos.Y - pos.Y, 2)); // ищем расстояние
            if (r < 150)
                Active = true;
            else
                Active = false;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (Active)
                spriteBatch.Draw(texture, Pos - (Game as SandCore).camera.Pos, Color.White);
            else
                spriteBatch.Draw(texture, Pos - (Game as SandCore).camera.Pos, Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // нажатие на левую кнопку мыши
        private void Break()
        {
            Vector2 positionBlockCursor = new Vector2(Tile.Position[0] * 32, Tile.Position[1] * 32) + Chunk.Pos;
            for (int i = 0; i < Game.Components.Count; i++) // проход по всем блокам
            {
                Block block = (Game.Components[i] as Block);
                if (block != null)
                {
                    // блок в курсоре и есть инструмент, который может его добыть
                    if (block.Pos == positionBlockCursor)
                    {
                        if (block.Instrument != "" && block.Instrument != null)
                        {
                            if (SandCore.game.resources.Resource[block.Instrument] > 0)
                            {
                                SimpleTimer timer = new SimpleTimer(block.Hardness * 1000, Breaking, block); // ломает блок n секунд
                                breaking = true;
                                return;
                            }
                        }
                        else
                        {
                            SimpleTimer timer = new SimpleTimer(block.Hardness * 1000, Breaking, block); // ломает блок n секунд
                            breaking = true;
                            return;
                        }
                    }
                }
            }

            // ломание tile-ов
            Resources res = (Game as SandCore).resources;
            // если тайл - это камень и у игрока есть кирка
            if (Tile.ID == 3 && res.Resource["pickaxe"] > 0)
            {
                int chance = new Random().Next(101);

                // из камня можно с разным шансом добыть
                if (chance <= 5)
                    res.AddResource("raw_iron", 1); // железо
                if (chance > 5 && chance <= 20)
                    res.AddResource("coal", 1); // уголь
                if (chance > 20)
                    res.AddResource("stone", 1); // сам камень
            }
        }

        // нажатие на правую кнопку мыши
        private void Use()
        {
            Inventory inventory = SandCore.game.inventory;
            Resources resources = SandCore.game.resources;
            Hero hero = SandCore.game.hero;

            Vector2 positionBlockCursor = new Vector2(Tile.Position[0] * 32, Tile.Position[1] * 32) + Chunk.Pos;
            Rectangle collider = new Rectangle(positionBlockCursor.ToPoint(), new Point(32, 32)); // коллайдер курсора
            string block = inventory.choosenBlock;

            // чтобы блок не заспавнился в игроке
            if(block != "" && !collider.Intersects(new Rectangle(hero.Pos.ToPoint(), new Point(32, 32))))
            {
                if (resources.Resource[block] == 0)
                    return;
                // тут создаем блоки (да не автоматом)
                if (block == "furnace")
                {
                    resources.AddResource(block, -1);
                    new Furnace(Game, positionBlockCursor);
                }
                if (block == "wood")
                {
                    resources.AddResource(block, -15);
                    new Wood(Game, positionBlockCursor);
                }
                if (block == "mine")
                {
                    resources.AddResource(block, -1);
                    new Mine(Game, positionBlockCursor);
                }
                if (block == "lumberjack")
                {
                    resources.AddResource(block, -1);
                    new Lumberjack(Game, positionBlockCursor);
                }
            }
        }

        // ломание блока
        private void Breaking(object obj)
        {
            if (breaking)
            {
                breaking = false;
                Block block = (obj as Block);
                block.Break();
            }
        }
    }
}
