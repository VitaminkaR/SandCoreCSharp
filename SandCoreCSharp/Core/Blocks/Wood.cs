using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SandCoreCSharp.Core.Blocks
{
    class Wood : Block
    {
        public Wood(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "wood";
            IsSolid = true;
            Hardness = 1;
            Instrument = "";
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
