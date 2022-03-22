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

                        // чанк в котором он был
                        string[] _chunk = data[2].Split(new char[3] { ':', '}', ' ' });
                        Chunk chunk = game.terrain.GetChunk(Convert.ToInt32(_chunk[1]), Convert.ToInt32(_chunk[3]));

                        // его позиция в чанке
                        string[] pos = data[3].Split(new char[3] { ':', '}', ' ' });
                        Point point = new Point(Convert.ToInt32(pos[1]), Convert.ToInt32(pos[3]));

                        // ЗДЕСЬ ПРОПИСЫВАТЬ ТИПЫ БЛОКОВ
                        if (type == "SandCoreCSharp.Core.Blocks.Wood")
                            Block.Blocks.Add(new Wood(game, chunk, point));
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
            Stream stream = File.Open($"maps/{mapName}", FileMode.OpenOrCreate);
            List<Block> blocks = Block.Blocks;

            string data = "";

            for (int i = 0; i < blocks.Count; i++)
            {
                Block block = blocks[i];
                string type = block.GetType().ToString();
                string chunk = block.Chunk.Pos.ToString();
                string pos = block.Position.ToString();

                data += "block|" + type + '|' + chunk + '|' + pos +  "\n";
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
