using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SandCoreCSharp.Core.Blocks
{
    // печь
    class Furnace : Mechanism
    {
        public Furnace(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "FURNACE";
            IsSolid = true;
            Hardness = 0;
            isSaving = true;
        }

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("Furnace");

            base.LoadContent();
        }

        public override void Break()
        {
            Resources resources = (Game as SandCore).resources;
            resources.AddResource("furnace", 1);

            base.Break();
        }
    }
}
