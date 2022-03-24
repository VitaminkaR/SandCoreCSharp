using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SandCoreCSharp.Core.Blocks
{
    // прозрачный блок, который представляет собой камушки
    class Stones : Block
    {
        public Stones(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "STONES";
            IsSolid = false;
            Hardness = 0;
            isSaving = false;
        }

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("Stones");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // если игрок слишком далек от камня, то удаляем камень, для новой генерации
            Vector2 playerPos = player.Pos;
            float r = MathF.Sqrt(MathF.Pow(playerPos.X - Pos.X, 2) + MathF.Pow(playerPos.Y - Pos.Y, 2)); // ищем расстояние
            if (r > 4096)
                base.Break();

            base.Update(gameTime);
        }

        public override void Break()
        {
            // когда нас собирает игрок, то реурс камня возрастает
            Resources resources = (Game as SandCore).resources;
            resources.AddResource("stone", 1);

            base.Break();
        }
    }
}
