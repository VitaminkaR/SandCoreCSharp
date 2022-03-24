using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace SandCoreCSharp.Core
{
    // класс который хранит все ресурсы добытые игроком
    public class Resources : GameComponent
    {
        // инструменты которые есть у игрока
        public List<Instruments> Instruments { get; private set; }

        // ресурсы
        public Dictionary<string, int> Resource { get; private set; }

        public Resources(Game game) : base(game)
        {
            game.Components.Add(this);
            Resource = new Dictionary<string, int>();
            Instruments = new List<Instruments>();
        }

        public override void Initialize()
        {
            Instruments.Add(Core.Instruments.none);

            // ИНИЦИАЛИЗАИЯ РЕСУРСОВ
            Resource.Add("stone", 0);
            Resource.Add("wood", 0);
            Resource.Add("coal", 0);
            Resource.Add("iron", 0);

            LoadResources();

            base.Initialize();
        }

        // сохраняет инфу о ресурсах в файл
        private void SaveResources()
        {
            string data = "";

            foreach (var resource in Resource)
            {
                data += resource.Key + "|" + resource.Value + '\n';
            }

            using (StreamWriter sr = new StreamWriter("maps\\" + SandCore.map + "\\resources"))
            {
                sr.Write(data);
            }
        }

        // загружает ресурсы
        private void LoadResources()
        {
            if (new FileInfo("maps\\" + SandCore.map + "\\resources").Exists)
            {
                using (StreamReader sr = new StreamReader("maps\\" + SandCore.map + "\\resources"))
                {
                    string line = "";
                    while (true)
                    {
                        line = sr.ReadLine();

                        if (line == null)
                            break;

                        string res = line.Split('|')[0];
                        int value = Convert.ToInt32(line.Split('|')[1]);
                        Resource[res] = value;
                    }
                }
            }
        }

        // изменяет ресурс (добавляет)
        public void AddResource(string type, int value)
        {
            Resource[type] += value;
            SaveResources();
        }
    }
}
