using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SandCoreCSharp.Utils;
using System.Threading;

namespace SandCoreCSharp.Core.Blocks
{
    class Lumberjack : Block
    {
        public Lumberjack(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "lumberjack";
            isSaving = true;
            Hardness = 0;
            IsSolid = true;
            Instrument = "";
        }

        public override void Update(GameTime gameTime)
        {
            Resources res = SandCore.resources;
            res.AddResource("wood", 0.25f);

            base.Update(gameTime);
        }
    }
}
