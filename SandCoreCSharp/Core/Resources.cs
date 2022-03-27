using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using SandCoreCSharp.Utils;

namespace SandCoreCSharp.Core
{
    // класс который хранит все ресурсы добытые игроком
    public class Resources : GameComponent
    {
        // ресурсы
        public Dictionary<string, int> Resource { get; private set; }

        public Resources(Game game) : base(game)
        {
            game.Components.Add(this);
            Resource = new Dictionary<string, int>();

            SimpleTimer saver = new SimpleTimer(5000, SaveResources, null);
        }

        public override void Initialize()
        {
            // ИНИЦИАЛИЗАИЯ РЕСУРСОВ
            // инструменты
            Resource.Add("axe", 0);
            Resource.Add("pickaxe", 0);
            Resource.Add("shovel", 0);

            // ископаемые
            Resource.Add("stone", 0);
            Resource.Add("wood", 0);
            Resource.Add("coal", 0);
            Resource.Add("raw_iron", 0);
            Resource.Add("iron", 0);
            Resource.Add("sand", 0);
            Resource.Add("quartz", 0);
            Resource.Add("raw_gold", 0);
            Resource.Add("gold", 0);
            Resource.Add("adamant", 0);
            Resource.Add("electrit", 0);

            // стандартные механизмы
            Resource.Add("furnace", 0);
            Resource.Add("mine", 0);
            Resource.Add("lumberjack", 0);

            // промышленные механизмы
            Resource.Add("wire", 0);
            Resource.Add("frame", 0);
            Resource.Add("battery", 0);

            LoadResources();

            base.Initialize();
        }

        // сохраняет инфу о ресурсах в файл
        private void SaveResources(object obj)
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
            SimpleTimer saver = new SimpleTimer(5000, SaveResources, null);
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
        }

        protected override void Dispose(bool disposing)
        {
            SaveResources(null);

            base.Dispose(disposing);
        }
    }
}
