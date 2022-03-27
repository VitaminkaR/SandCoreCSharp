using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SandCoreCSharp.Utils;
using System.Threading;

namespace SandCoreCSharp.Core.Blocks
{
    // шахта добывающая игроку ресурсы
    class Mine : Mechanism
    {
        public Mine(Game game, Vector2 pos) : base(game, pos)
        {
            Type = "mine";
            isSaving = true;
            Hardness = 0;
            IsSolid = true;
            Instrument = "";

            SimpleTimer timer = new SimpleTimer(1000, Mining, null);
        }

        protected override void LoadContent()
        {
            sprite = content.Load<Texture2D>("Mine");

            base.LoadContent();
        }

        public override void Break()
        {
            Resources resources = (Game as SandCore).resources;
            resources.AddResource("mine", 1);

            base.Break();
        }

        // добыча
        private void Mining(object obj)
        {
            Resources res = SandCore.game.resources;
            res.AddResource("stone", 10);
            res.AddResource("coal", 5);
            res.AddResource("raw_iron", 1);
            SimpleTimer timer = new SimpleTimer(1000, Mining, null);
        }
    }
}
