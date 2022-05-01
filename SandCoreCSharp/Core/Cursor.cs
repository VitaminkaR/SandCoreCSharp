using System;
using System.Collections.Generic;
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
        const float CURSOR_SIZE = Terrain.TILE_SIZE;

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

        private Graphics graphics;


        public Cursor(Game game, Hero hero) : base(game)
        {
            player = hero;
        }


        public override void Initialize()
        {
            Pos = new Vector2();
            graphics = new Graphics(Game.GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = content.Load<Texture2D>("tile");
            graphics.Texture = texture;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Camera camera = SandCore.camera;
            Terrain terrain = SandCore.terrain;
            MouseState state = Mouse.GetState();

            Vector3 pos = Game.GraphicsDevice.Viewport.Unproject(new Vector3(state.Position.ToVector2(), 0), camera.projectionMatrix, camera.viewMatrix, camera.worldMatrix);
            Tile tile = terrain.GetTile(new Vector2(pos.X, pos.Y + Terrain.TILE_SIZE));
            if (tile.Chunk != null)
            {
                Pos = tile.Chunk.Pos + new Vector2((tile.Position[0] * Terrain.TILE_SIZE), (tile.Position[1] * Terrain.TILE_SIZE));
                Chunk = tile.Chunk;
                Tile = tile;
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
            DrawRect(Pos.X, Pos.Y);
            graphics.Drawing();

            base.Draw(gameTime);
        }

        private void DrawRect(float x, float y)
        {
            graphics.Vertices = new List<VertexPositionColorTexture>();
            graphics.Indices = new List<int>();
            Color color = new Color(0, 128, 0, 50);

            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x, y, 0), color, new Vector2(0, 0)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x + CURSOR_SIZE, y, 0), color, new Vector2(1, 0)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x + CURSOR_SIZE, y - CURSOR_SIZE, 0), color, new Vector2(1, 1)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x, y - CURSOR_SIZE, 0), color, new Vector2(0, 1)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);

            graphics.Indices.Add(graphics.Vertices.Count - 4);
            graphics.Indices.Add(-1);
        }

        // нажатие на левую кнопку мыши
        private void Break()
        {
            Vector2 positionBlockCursor = new Vector2(Tile.Position[0] * Terrain.TILE_SIZE, Tile.Position[1] * Terrain.TILE_SIZE) + Chunk.Pos;
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
                            if (SandCore.resources.Resource[block.Instrument] > 0)
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
            Resources res = SandCore.resources;
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

            // если тайл - это земля и у игрока есть лопата
            if ((Tile.ID == 2 || Tile.ID == 5) && res.Resource["shovel"] > 0)
            {
                int chance = new Random().Next(101);

                // из земли можно с разным шансом добыть
                if (chance <= 25)
                    res.AddResource("sand", 1); // песок
                if (chance <= 15)
                    res.AddResource("seed", 1); // семена
            }

            // если тайл - вода и есть ведро
            if (Tile.ID == 4 && res.Resource["bucket"] > 0)
                res.AddResource("water", 1f * res.Resource["bucket"]);
        }

        // нажатие на правую кнопку мыши
        private void Use()
        {
            Inventory inventory = SandCore.inventory;
            Resources resources = SandCore.resources;
            Hero hero = SandCore.hero;

            Vector2 BlockPosition = new Vector2(Tile.Position[0] * Terrain.TILE_SIZE, Tile.Position[1] * Terrain.TILE_SIZE) + Chunk.Pos;
            Rectangle collider = GetCollider();
            string block = inventory.choosenBlock;

            // чтобы блок не заспавнился в игроке
            if(block != null && resources.Resource[block] != 0 && block != "" && !hero.Collision(BlockPosition))
            {
                // если есть мотыга и тайл - земля
                if (Tile.ID == 2 && block == "hoe")
                {
                    Block.CreateBlock("land", Pos);
                    return;
                }

                Block.CreateBlock(block, BlockPosition);
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



        // позволяет блоку получить инфу на него ли наведена мышь
        static public bool FocusOnBlock(Vector2 position)
        {
            Cursor cursor = SandCore.cursor;
            Rectangle collider = GetCollider();
            Rectangle colliderBlock = new Rectangle(position.ToPoint(), new Point(32, 32));
            if (collider.Intersects(colliderBlock))
                return true;
            else
                return false;
        }

        static public Rectangle GetCollider() => new Rectangle(SandCore.cursor.Pos.ToPoint(), new Point(32, 32));
    }
}
