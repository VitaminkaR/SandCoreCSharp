using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Core.Blocks;

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
                Pos = (new Vector2(Tile.Position[0] * 32, Tile.Position[1] * 32) + Chunk.Pos) - camera.Pos;
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
                mouseBlock = false;



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
            Vector2 camPos = (Game as SandCore).camera.Pos;
            spriteBatch.Begin();
            if (Active)
                spriteBatch.Draw(texture, Pos, Color.White);
            else
                spriteBatch.Draw(texture, Pos, Color.Black);
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
                    if (block.GetPosition() == positionBlockCursor)
                        block.Break();
            }
        }

        // нажатие на правую кнопку мыши
        private void Use()
        {
            new Wood(Game, Chunk, new Point(Tile.Position[0], Tile.Position[1]));
        }
    }
}
