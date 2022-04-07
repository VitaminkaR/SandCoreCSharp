using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SandCoreCSharp.Core.Blocks
{
    // печь
    class Furnace : Block
    {
        public Furnace(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "furnace";
            IsSolid = true;
            Hardness = 0;
            isSaving = true;
        }

        public override void Update(GameTime gameTime)
        {
            Resources res = SandCore.resources;

            if (res.Resource["raw_iron"] > 0)
            {
                res.AddResource("iron", 0.05f);
                res.AddResource("raw_iron", -0.05f);
            }

            base.Update(gameTime);
        }
    }
}
