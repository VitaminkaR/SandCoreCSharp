using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SandCoreCSharp.Core.Blocks
{
    class Wood : Block
    {
        public Wood(Game game, Vector2 pos) : base(game, pos)
        {
            IsSolid = true;
            Hardness = 1;
            Instrument = Instruments.none;
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
