using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace SandCoreCSharp.Core
{
    // класс, генерирующий и отрисовывающий чанки
    class Terrain : DrawableGameComponent
    {
        private ContentManager content;
        private SpriteBatch spriteBatch;

        // sprites tiles
        // 0 - tile
        private Texture2D[] sprites;

        // view chunks
        private List<Chunk> chunks;

        public Terrain(Game game) : base(game)
        {
            game.Components.Add(this);
            content = game.Content;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        // init
        public override void Initialize()
        {
            chunks = new List<Chunk>();
            sprites = new Texture2D[255];

            base.Initialize();
        }

        // load sprites (ADD SPRITES)
        protected override void LoadContent()
        {
            sprites[0] = content.Load<Texture2D>("Mud");
            sprites[1] = content.Load<Texture2D>("Grass");

            base.LoadContent();
        }

        // draw chunks
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            for (int i = 0; i < chunks.Count; i++)
            {
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            byte id = chunks[i].Tiles[x, y, z]; // получаем id
                            if (id == 0) // если воздух то идем далее
                                continue;

                            // отрисовываем блок с текстурой для его id и делаем более темным в зависимости от его высоты
                            spriteBatch.Draw(sprites[id - 1], new Vector2(x * 32, y * 32), new Color(255 - z * 8, 255 - z * 8, 255 - z * 8));
                            break; // если блок отрисован, то нет смысла рисовать ниже (оптимизация)
                        }
                    }
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        // generate chunk (debug method)
        public void Generate()
        {
            Chunk @new = new Chunk(0, 0);
            // заполняем воздухом
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        @new.Tiles[x, y, z] = 0;
                    }
                }
            }
            
             // генерируем землю на высоте 8 
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    @new.Tiles[x, y, 8] = 1;
                }
            }

            // добавляем в рисуемые чанки (потому все с камерой будет связано)
            chunks.Add(@new);
        }
    }
}
