using Microsoft.Xna.Framework;
using System;

namespace SandCoreCSharp.Core.Blocks
{
    // класс представлет собой блок механизма
    // механика механизма отличает от блока только тем
    // что механизм добавляет в лист механизмов у игрока на поределенном расстоянии
    public class Mechanism : Block
    {
        // когда рядом игрок
        public bool Active { get; private set; }

        public Mechanism(Game game, Vector2 pos) : base(game, pos)
        {
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 playerPos = player.Pos;
            float r = MathF.Sqrt(MathF.Pow(playerPos.X - Pos.X, 2) + MathF.Pow(playerPos.Y - Pos.Y, 2)); // ищем расстояние
            if (r < 256 && !player.Mechanisms.Contains(Type))
            {
                player.Mechanisms.Add(Type);
                Active = true;
            }
                
            if(r > 256)
            {
                player.Mechanisms.Remove(Type);
                Active = false;
            }
                

            base.Update(gameTime);
        }
    }
}
