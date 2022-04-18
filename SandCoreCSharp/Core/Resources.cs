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
        public Dictionary<string, float> Resource { get; private set; }

        // кол-во энергии
        public int Energy { get; internal set; } = 0;
        // размер буфера энергии (увеличивается вместе с созданием батарей)
        static public int MaxEnergy { get; protected set; } = 0;

        public Resources(Game game) : base(game)
        {
            game.Components.Add(this);
            Resource = new Dictionary<string, float>();
        }

        public override void Update(GameTime gameTime)
        {
            MaxEnergy = (int)(Resource["battery"] * 15000);

            base.Update(gameTime);
        }

        public override void Initialize()
        {
            // ИНИЦИАЛИЗАИЯ РЕСУРСОВ
            // инструменты
            Resource.Add("axe", 0);
            Resource.Add("pickaxe", 0);
            Resource.Add("shovel", 0);
            Resource.Add("hoe", 0);
            Resource.Add("bucket", 0);

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
            Resource.Add("frame", 0);
            Resource.Add("battery", 0);
            Resource.Add("coalgenerator", 0);
            Resource.Add("quarry", 0);
            Resource.Add("inductionfurnace", 0);
            Resource.Add("farmer", 0);

            // жидкости
            Resource.Add("water", 0);

            // растения
            Resource.Add("seed", 0);
            Resource.Add("wheat", 0);
            Resource.Add("cotton", 0);

            // другое
            Resource.Add("string", 0);

            LoadResources();
            LoadResourceEnergy();

            Resource["pickaxe"] = 1;

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
                        float value = Convert.ToSingle(line.Split('|')[1]);
                        Resource[res] = value;
                    }
                }
            }
        }

        // изменяет ресурс (добавляет)
        public void AddResource(string type, float value)
        {
            if(Resource.ContainsKey(type))
                Resource[type] += value;
        }

        // сохраняет инфу о ресурсах в файл
        public void SaveResourceEnergy()
        {
            string data = Energy.ToString();

            using (StreamWriter sr = new StreamWriter("maps\\" + SandCore.map + "\\energy"))
            {
                sr.Write(data);
            }
        }

        // загружает ресурсы
        public void LoadResourceEnergy()
        {
            if (new FileInfo("maps\\" + SandCore.map + "\\energy").Exists)
            {
                using (StreamReader sr = new StreamReader("maps\\" + SandCore.map + "\\energy"))
                {
                    string str = sr.ReadLine();
                    Energy = Convert.ToInt32(str);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            SaveResources(null);
            SaveResourceEnergy();

            base.Dispose(disposing);
        }
    }
}
