using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SandCoreCSharp.Utils;
using System.Threading;

namespace SandCoreCSharp.Core.Blocks
{
    // шахта добывающая игроку ресурсы
    class Mine : Block
    {
        public Mine(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "mine";
            isSaving = true;
            Hardness = 0;
            IsSolid = true;
            Instrument = "";
        }

        public override void Update(GameTime gameTime)
        {
            Resources res = SandCore.resources;
            res.AddResource("stone", 0.1f);
            res.AddResource("coal", 0.05f);
            res.AddResource("raw_iron", 0.01f);

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("Mine");

            base.LoadContent();
        }
    }
}
