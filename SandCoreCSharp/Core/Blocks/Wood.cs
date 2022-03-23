using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SandCoreCSharp.Core.Blocks
{
    class Wood : Block
    {
        public Wood(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "WOOD";
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

        public override void Break()
        {
            Resources resources = (Game as SandCore).resources;
            resources.Resource["wood"] += 1;

            base.Break();
        }
    }
}
