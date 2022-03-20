using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace SandCoreCSharp.Core
{
    class ViewGameObject : DrawableGameComponent
    {
        // позиция 
        public Vector2 Pos { get; protected set; }

        // спрайт
        protected Texture2D texture;

        protected SpriteBatch spriteBatch;
        protected ContentManager content;

        public ViewGameObject(Game game) : base(game)
        {
            Game.Components.Add(this);
            content = game.Content;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }
    }
}
