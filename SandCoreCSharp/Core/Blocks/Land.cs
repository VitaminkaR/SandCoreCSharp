using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Utils;
using System;

namespace SandCoreCSharp.Core.Blocks
{
    // класс грядки (основа земледелия)
    // ! спрайт растение берется по такой структуре "название_растения(такое же как у ресурса растения)" + "_" + "стадия_роста(цифра)" 
    class Land : Block
    {
        private int counter;

        public Land(Game game, Vector2 pos) : base(game, pos)
        {
            IsSolid = false;
            Hardness = 0;
            Instrument = "";

            Tags[0] = "f";
            Tags[1] = "none";
            Tags[2] = "0";

            
        }



        public override void Draw(GameTime gameTime)
        {
            // таймер (зависит от кадро, но мне абсолютно похер)
            if (Tags[1] != "none")
            {
                counter++;
                if (counter == 1000)
                {
                    Ripen();
                    counter = 0;
                }
            }

            // если земля влажная
            if (Tags[0] == "t")
                sprite = Sprites["mud"];

            // если посажены семена и 1 стадия роста
            if (Tags[1] != "none")
            {
                if (Tags[2] == "0")
                    sprite = Sprites["mud_with_seeds"];
                else
                    sprite = Sprites[Tags[1] + '_' + Tags[2]];
            }


            // если земля сухая
            if (Tags[0] == "f")
                sprite = Sprites["land"];

            base.Draw(gameTime);
        }

        protected override void Using()
        {
            Inventory inventory = SandCore.inventory;

            // если сухая, ничего не растет, есть ведро и вода, то поливаем грядку
            if (inventory.choosenBlock == "bucket")
            {
                Wet();
            }

            // если мокрая, ничего не растет, выбраны семена, то сажаем семена (рандомное в будущем растение)
            if (inventory.choosenBlock == "seed")
            {
                Plant();
            }

            base.Using();
        }

        // когда растение поспевает
        private void Ripe()
        {
            SandCore.resources.AddResource(Tags[1], 1);
            SandCore.resources.AddResource("seed", 2);
            Tags[0] = "f";
            Tags[1] = "none";
            Tags[2] = "0";
            SaveTags();
        }

        // рост растения, повышении стадии и завершение роста(созревание для каждого вида растения)
        private void Ripen()
        {
            int stage = Convert.ToInt32(Tags[2]);
            Tags[2] = (stage + 1).ToString();
            SaveTags();

            // Стадия на которой созревает растение
            if (Tags[1] == "wheat" && stage == 2)
                Ripe();
            if (Tags[1] == "cotton" && stage == 3)
                Ripe();
        }

        //  полить грядку
        public void Wet()
        {
            Resources res = SandCore.resources;

            if (Tags[0] == "f" && Tags[1] == "none" && res.Resource["water"] > 0)
            {
                Tags[0] = "t";
                res.AddResource("water", -1);
            }
        }

        // посадить что-то
        public void Plant()
        {
            Resources res = SandCore.resources;

            if (Tags[0] == "t" && Tags[1] == "none" && res.Resource["seed"] > 0)
            {
                res.AddResource("seed", -1);

                // тут рандомно выбираем растения (растение называть как название ресурса)
                int id = new Random().Next(2);
                if (id == 0)
                    Tags[1] = "wheat";
                if (id == 1)
                    Tags[1] = "cotton";
            }
        }
    }
}
