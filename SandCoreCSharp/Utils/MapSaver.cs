using System;
using SandCoreCSharp.Core;
using SandCoreCSharp.Core.Blocks;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace SandCoreCSharp.Utils
{
    static public class MapSaver
    {
        static public void LoadMap(string mapName, SandCore game)
        {
            Stream stream = File.Open($"maps/{mapName}", FileMode.OpenOrCreate);
            using (StreamReader reader = new StreamReader(stream))
            {
                string line = "";
                do
                {
                    line = reader.ReadLine();

                    if (line == null)
                        break;

                    string[] data = line.Split('|');

                    // создаем блоки
                    if(data[0] == "block")
                    {
                        // ЗАПИСЬ БЛОКОВ
                        string type = data[1]; // тип

                        // его позиция в чанке
                        string[] posd = data[2].Split(new char[3] { ':', '}', ' ' });
                        Vector2 pos = new Vector2(Convert.ToInt32(posd[1]), Convert.ToInt32(posd[3]));

                        // ЗДЕСЬ ПРОПИСЫВАТЬ ТИПЫ БЛОКОВ
                        if (type == "SandCoreCSharp.Core.Blocks.Wood")
                            new Wood(game, pos);
                    }

                    // позиция игрока
                    if (data[0] == "ppos")
                    {
                        string[] ppos = data[1].Split(new char[3] { ':', '}', ' ' });
                        Vector2 pos = new Vector2(Convert.ToInt32(ppos[1]), Convert.ToInt32(ppos[3]));
                        game.hero.SetPos(pos);
                    }

                } while (line != "");
            }
        }

        static public void SaveMap(string mapName, SandCore game)
        {
            // для перезаписи
            FileInfo info = new FileInfo($"maps/{mapName}");
            if (info.Exists)
                info.Delete();

            Stream stream = File.Open($"maps/{mapName}", FileMode.OpenOrCreate);
            List<Block> blocks = Block.Blocks;

            string data = "";

            for (int i = 0; i < blocks.Count; i++)
            {
                Block block = blocks[i];
                string type = block.GetType().ToString();
                string pos = block.Pos.ToString();

                data += "block|" + type + '|' + pos +  "\n";
            }
            string playerPos = game.hero.Pos.ToString();
            data += "ppos|" + playerPos + "\n";

            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(data);
            }
        }

    }
}
