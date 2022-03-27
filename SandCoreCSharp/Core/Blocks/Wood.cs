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

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("Wood");

            base.LoadContent();
        }

        public override void Break()
        {
            Resources resources = (Game as SandCore).resources;
            if(resources.Resource["axe"] > 0) // если есть топорик есть, то в общем добудем 15 а не 5 дерева
                resources.AddResource("wood", 10);
            resources.AddResource("wood", 5);

            base.Break();
        }
    }
}
