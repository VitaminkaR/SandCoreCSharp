﻿using Microsoft.Xna.Framework;
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
            // ИНИЦИАЛИЗАЦИЯ КРАФТОВ Recipes.Add("ресурс", "компонент|кол-во+компонент|кол-во требуемое устройства");
            // топорик
            Recipes.Add("axe", "iron|15+wood|25 ");
            // кирка
            Recipes.Add("pickaxe", "iron|20+wood|25 ");
            // лопата
            Recipes.Add("shovel", "iron|10+wood|25 ");
            // железо
            Recipes.Add("iron", "raw_iron|2+coal|1 FURNACE");
            // печь
            Recipes.Add("furnace", "stone|50 ");
            // шахта
            Recipes.Add("mine", "stone|50+wood|100+iron|30 ");
            // лесорубка
            Recipes.Add("lumberjack", "wood|120+iron|15 ");
            // корпус механизма
            Recipes.Add("frame", "iron|50 ");

            base.Initialize();
        }

        // осуществление крафта
        public void Craft(string item)
        {
            Resources res = SandCore.game.resources;
            string[] craftMarkup = Recipes[item].Split(' ');
            string[] components = craftMarkup[0].Split('+');

            if (!MayCraft(item))
                return;

            for (int i = 0; i < components.Length; i++)
            {
                string component = components[i].Split('|')[0];
                int count = Convert.ToInt32(components[i].Split('|')[1]);
                res.AddResource(component, -count);
            } // удаляем их

            // добавляем нужный предмет
            res.AddResource(item, 1);
        }

        // проверка можно ли скрафтить
        public bool MayCraft(string item)
        {
            Resources res = SandCore.game.resources;
            Hero hero = SandCore.game.hero;

            string[] craftMarkup = Recipes[item].Split(' ');
            string[] components = craftMarkup[0].Split('+');

            for (int i = 0; i < components.Length; i++)
            {
                string component = components[i].Split('|')[0];
                int count = Convert.ToInt32(components[i].Split('|')[1]);
                if (res.Resource[component] < count)
                    return false;
                if (!hero.Mechanisms.Contains(craftMarkup[1]))
                    return false;
            }
            return true;
        }
    }
}
