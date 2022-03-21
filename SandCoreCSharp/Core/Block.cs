using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace SandCoreCSharp.Core
{
    class Block : DrawableGameComponent
    {
        public static List<Rectangle> Colliders { get; private set; } = new List<Rectangle>();

        // чанк в котором блок
        private Chunk chunk;
        // позиция чанка в чанке (у блоков нет высоты, они всегда на максимальной)
        private Point position;

        protected ContentManager content;
        protected SpriteBatch spriteBatch;

        // спрайты
        protected Texture2D sprite;

        // взаимодействия
        protected Hero player;
        protected Camera camera;
        protected Terrain terrain;

        public Block(Game game, Chunk _chunk, Point _position) : base(game)
        {
            content = Game.Content;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            Game.Components.Add(this);

            SandCore sandCoreGame = (game as SandCore);
            player = sandCoreGame.hero;
            camera = sandCoreGame.camera;
            terrain = sandCoreGame.terrain;

            chunk = _chunk;
            position = _position;

            // проходим и проверяем, если там уже стоит блок, то новый не создаем
            for (int i = 0; i < Game.Components.Count; i++)
            {
                Block block = (Game.Components[i] as Block);
                if (block != null)
                    if (block.GetPosition() == this.GetPosition() && block != this)
                        block.Break();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        // отрисовка спрайта в позиции
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            Vector2 pos = GetPosition() - camera.Pos;
            if (sprite != null)
                spriteBatch.Draw(sprite, pos, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void Break()
        {
            Game.Components.Remove(this);
            Rectangle collider = new Rectangle((int)(chunk.Pos.X + position.X * 32), (int)(chunk.Pos.Y + position.Y * 32), 32, 32);
            if (Colliders.Contains(collider))
                Colliders.Remove(collider);
        }

        // нужно вызвать, чтобы сделать блок твердым
        protected void Solid() => Colliders.Add(new Rectangle((int)(chunk.Pos.X + position.X * 32), (int)(chunk.Pos.Y + position.Y * 32), 32, 32));

        public Vector2 GetPosition() => new Vector2(chunk.Pos.X + position.X * 32, chunk.Pos.Y + position.Y * 32);
    }
}
