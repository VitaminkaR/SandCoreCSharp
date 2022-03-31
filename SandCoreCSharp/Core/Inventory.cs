using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Core.Blocks;

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

        // выбранный рецепт 
        string choosenRecipe;

        // выбранный ресурс
        public string choosenBlock;

        private SpriteBatch spriteBatch;

        public Inventory(Game game) : base(game)
        {
            game.Components.Add(this);
            content = Game.Content;
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            DrawOrder = 2;
        }

        protected override void LoadContent()
        {
            background = content.Load<Texture2D>("UI\\Screen");
            font = content.Load<SpriteFont>("UI\\UIFont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            CraftManager manager = SandCore.game.craftManager;
            Resources res = SandCore.game.resources;
            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            if (ks.IsKeyDown(inventoryKey) && !block) // открытие инвентаря
            {
                inventory = !inventory;
                block = true;
            }

            if (ks.IsKeyUp(inventoryKey) && ms.LeftButton == ButtonState.Released)
                block = false;

            // когда инвентарь открыт
            if (inventory)
            {
                // выбор рецепта крафта
                if (ms.X > SandCore.WIDTH / 2)
                {
                    int number = ms.Position.Y / 16 - 4; // позиция по счету (как он отрисовывается)
                    int counter = 0;
                    foreach (var recipe in manager.Recipes) // перебираем рецепты
                    {
                        if (counter == number)
                        {
                            choosenRecipe = recipe.Key;
                            break;
                        }

                        counter++;
                    }

                    // если мышка вышла за края выбора крафтов(т е не наведена ни на один крафт), то убираем выделение
                    if (number < 0 || number >= manager.Recipes.Count + 2)
                        choosenRecipe = "";
                }else
                    choosenRecipe = "";

                //выбор ресурса (чтоб ставить блоки)
                if (ms.X < SandCore.WIDTH / 2 && ms.LeftButton == ButtonState.Pressed)
                {
                    int number = ms.Position.Y / 16 - 4; // позиция по счету (как он отрисовывается)
                    int counter = 0;
                    foreach (var resource in res.Resource) // перебираем ресурсы
                    {
                        if (counter == number)
                        {
                            choosenBlock = resource.Key;
                            break;
                        }

                        counter++;
                    }

                    // если мышка вышла за края выбора ресусров(т е не наведена ни на один крафт), то убираем выделение
                    if (number < 0 || number >= res.Resource.Count + 2 || ms.X > SandCore.WIDTH / 2)
                        choosenBlock = "";
                }

                // МАГИЯ КРАФТА
                if (ms.LeftButton == ButtonState.Pressed && !block)
                {
                    block = true;
                    if(choosenRecipe != "" && choosenRecipe != null)
                        manager.Craft(choosenRecipe);
                }
            }

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
                    Color color = Color.White; // если не выбран
                    if (choosenBlock == resource.Key) // если предмет выбран 
                        color = Color.Green;

                    spriteBatch.DrawString(font, resource.Key + " = " + resource.Value, new Vector2(16, 16 * count), color);
                    count++;
                }

                count = 3;

                // отрисовка крафтов
                spriteBatch.DrawString(font, "CRAFTING", new Vector2(SandCore.WIDTH / 2 + 16, 16), Color.White);
                foreach (var craft in manager.Recipes)
                {
                    Color color = Color.Red; // если нельзя скрафтить и не выбран
                    if (manager.MayCraft(craft.Key)) // если предмет можно скрафтить
                        color = Color.Green;
                    if (craft.Key == choosenRecipe) // если крафт предмета выбран
                        color = Color.White;

                    spriteBatch.DrawString(font, craft.Key + " = " + craft.Value, new Vector2(SandCore.WIDTH / 2 + 16, 16 * count), color);
                    count++;
                }

                // отрисовка инструментов
                spriteBatch.DrawString(font, "Pickaxe", new Vector2(160, 16), res.Resource["pickaxe"] > 0 ? Color.Green : Color.Red);
                spriteBatch.DrawString(font, "Axe", new Vector2(256, 16), res.Resource["axe"] > 0 ? Color.Green : Color.Red);
                spriteBatch.DrawString(font, "Shovel", new Vector2(310, 16), res.Resource["shovel"] > 0 ? Color.Green : Color.Red);

                // отрисовка энергии
                if(ElectroMachine.MaxEnergy > 0)
                    spriteBatch.DrawString(font, res.Energy.ToString() + "/" + ElectroMachine.MaxEnergy, new Vector2(452, 16), Color.Yellow);

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
