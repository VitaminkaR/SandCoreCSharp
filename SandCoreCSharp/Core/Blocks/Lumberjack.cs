using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SandCoreCSharp.Utils;
using System.Threading;

namespace SandCoreCSharp.Core.Blocks
{
    class Lumberjack : Mechanism
    {
        public Lumberjack(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "lumberjack";
            isSaving = true;
            Hardness = 0;
            IsSolid = true;
            Instrument = "";

            SimpleTimer timer = new SimpleTimer(1500, Felling, null);
        }

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("Lumberjack");

            base.LoadContent();
        }

        // добыча
        private void Felling(object obj)
        {
            Resources res = SandCore.game.resources;
            res.AddResource("wood", 20);
            SimpleTimer timer = new SimpleTimer(1000, Felling, null);
        }
    }
}
