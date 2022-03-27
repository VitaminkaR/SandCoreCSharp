using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Core.Blocks;
using System.Collections.Generic;

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

            // проходим и проверяем, если там уже стоит блок, то новый не создаем
            for (int i = 0; i < Blocks.Count; i++)
            {
                Block block = Blocks[i];
                if (block.Pos == this.Pos && block != this)
                {
                    Game.Components.Remove(block);
                    Blocks.Remove(block);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
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
            Game.Components.Remove(this);
            Blocks.Remove(this);
            Rectangle collider = new Rectangle(Pos.ToPoint(), new Point(32, 32));
        }

        // столкновение с игроком
        public virtual void CollidePlayer(Hero player)
        {}

        // чанк в котором блок
        public Chunk GetChunk() => terrain.GetChunk(Pos.X, Pos.Y);



        // регистрирует новые блоки 
        static public void CreateBlock(string type, Vector2 pos)
        {
            if (type == "wood")
                new Wood(SandCore.game, pos);
            if (type == "furnace")
                new Furnace(SandCore.game, pos);
            if (type == "mine")
                new Mine(SandCore.game, pos);
            if (type == "lumberjack")
                new Lumberjack(SandCore.game, pos);
            if (type == "wire")
                new Wire(SandCore.game, pos);
        }
    }
}
