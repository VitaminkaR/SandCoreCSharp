using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SandCoreCSharp.Core
{
    // класс отвечающий за осуществление крафтинга
    public class CraftManager : GameComponent
    {
        // recipes
        public Dictionary<string, string> Recipes { get; private set; }

        public CraftManager(Game game) : base(game)
        {
            Game.Components.Add(this);
            Recipes = new Dictionary<string, string>();
        }

        public override void Initialize()
        {
            // ИНИЦИАЛИЗАЦИЯ КРАФТОВ ""
            // топорик
            Recipes.Add("axe", "iron|15+wood|25");

            base.Initialize();
        }

        // осуществление крафта
        public void Craft(string item)
        {
            Resources res = SandCore.game.resources;
            string craftMarkup = Recipes[item];
            string[] components = craftMarkup.Split('+');
            for (int i = 0; i < components.Length; i++)
            {
                string component = components[i].Split('|')[0];
                int count = Convert.ToInt32(components[i].Split('|')[1]);
                if (res.Resource[component] < count)
                    return;
            } // проверяем есть ли необходмые компоненты

            for (int i = 0; i < components.Length; i++)
            {
                string component = components[i].Split('|')[0];
                int count = Convert.ToInt32(components[i].Split('|')[1]);
                res.AddResource(component, -count);
            } // удаляем их

            // добавляем нужный предмет
            res.AddResource(item, 1);
        }
    }
}
