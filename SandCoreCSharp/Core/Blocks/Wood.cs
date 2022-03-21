using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SandCoreCSharp.Core.Blocks
{
    class Wood : Block
    {
        public Wood(Game game, Chunk _chunk, Point _position) : base(game, _chunk, _position)
        {
            Solid();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("Wood");

            base.LoadContent();
        }
    }
}
