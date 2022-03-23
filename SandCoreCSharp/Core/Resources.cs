﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SandCoreCSharp.Core
{
    // класс который хранит все ресурсы добытые игроком
    public class Resources : GameComponent
    {
        // инструменты которые есть у игрока
        public List<Instruments> Instruments { get; private set; }

        // ресурсы
        public Dictionary<string, int> Resourse { get; set; }

        public Resources(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public override void Initialize()
        {
            Instruments = new List<Instruments>();
            Instruments.Add(Core.Instruments.none);

            // ИНИЦИАЛИЗАИЯ РЕСУРСОВ
            Resourse = new Dictionary<string, int>();
            Resourse.Add("stone", 0);

            base.Initialize();
        }
    }
}
