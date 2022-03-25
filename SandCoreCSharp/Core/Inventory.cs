using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace SandCoreCSharp.Core
{
    class Inventory : DrawableGameComponent
    {
        // кнопка на которую открывается инвентарь
        const Keys inventoryKey = Keys.E;

        private ContentManager content;

        // ui objects
        private Texture2D background;

        // открыт ли инвентарь
        private bool inventory;

        // keyboard block
        bool block;

        // font
        SpriteFont font;

        private SpriteBatch spriteBatch;

        public Inventory(Game game) : base(game)
        {
            game.Components.Add(this);
            content = Game.Content;
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            DrawOrder = 1;
        }

        protected override void LoadContent()
        {
            background = content.Load<Texture2D>("UI\\Screen");
            font = content.Load<SpriteFont>("UI\\UIFont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(inventoryKey) && !block) // открытие инвентаря
            {
                inventory = !inventory;
                block = true;
            }
                
            if (ks.IsKeyUp(inventoryKey))
                block = false;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Resources res = SandCore.game.resources;
            CraftManager manager = SandCore.game.craftManager;

            int count = 3; // для отрисовки

            if (inventory) // Отрисовываем инвентарь
            {
                spriteBatch.Begin();

                // отрисовка
                spriteBatch.Draw(background, new Rectangle(0, 0, SandCore.WIDTH, SandCore.HEIGHT), Color.White);

                //  отрисовка ресурсов
                spriteBatch.DrawString(font, "INVENTORY", new Vector2(16, 16), Color.White);
                foreach (var resource in res.Resource)
                {
                    spriteBatch.DrawString(font, resource.Key + " = " + resource.Value, new Vector2(16, 16 * count), Color.White);
                    count++;
                }

                count = 3;

                // отрисовка крафтов
                spriteBatch.DrawString(font, "CRAFTING", new Vector2(SandCore.WIDTH / 2 + 16, 16), Color.White);
                foreach (var craft in manager.Recipes)
                {
                    spriteBatch.DrawString(font, craft.Key + " = " + craft.Value, new Vector2(SandCore.WIDTH / 2 + 16, 16 * count), manager.MayCraft(craft.Key) ? Color.Green : Color.Red); ;
                    count++;
                }

                // отрисовка инструментов
                spriteBatch.DrawString(font, "Pickaxe", new Vector2(160, 16), res.Resource["pickaxe"] > 0 ? Color.Green : Color.Red);
                spriteBatch.DrawString(font, "Axe", new Vector2(256, 16), res.Resource["axe"] > 0 ? Color.Green : Color.Red);
                spriteBatch.DrawString(font, "Shovel", new Vector2(310, 16), res.Resource["shovel"] > 0 ? Color.Green : Color.Red);

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
