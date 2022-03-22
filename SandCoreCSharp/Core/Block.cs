using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace SandCoreCSharp.Core
{
    public class Block : DrawableGameComponent
    {
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

        // параметры при создании блока
        // прочность блока (сколько секунд будет разрушаться) -параметр-
        public int Hardness { get; protected set; } = 1;
        // имеет ли блок коллизию
        public bool IsSolid { get; protected set; } = true;

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

            // проходим и проверяем, если там уже стоит блок, то новый не создаем
            for (int i = 0; i < Blocks.Count; i++)
            {
                Block block = Blocks[i];
                if (block.Pos == this.Pos && block != this)
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
            Vector2 pos = Pos - camera.Pos;
            if (sprite != null)
                spriteBatch.Draw(sprite, pos, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // когда блок ломается
        public virtual void Break()
        {
            Game.Components.Remove(this);
            Blocks.Remove(this);
            Rectangle collider = new Rectangle(Pos.ToPoint(), new Point(32, 32));
        }

        // столкновение с игроком
        public virtual void CollidePlayer(Hero player)
        {}
    }
}
