using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace SandCoreCSharp.Core
{
    // класс, который представляет собой структуру для хранения информации о блоках
    public class Chunk
    {
        // хранение блоков с помощью id в byte типе (256 типов блоков)
        // общий размер чанка 16*16*16 = 2 kbyte
        // id
        // 0 - air
        // 1 - mud
        // 2 - grass
        // 3 - stone
        // 4 - water
        public byte[,,] Tiles { get; set; } = new byte[16, 16, 16];

        // позиция чанка
        public Vector2 Pos { get; private set; }


        public Chunk(float x, float y) => Pos = new Vector2(x, y);


        // возвращает имя чанка
        public string GetName() => "C=" + (int)(Pos.X / 512) + ";" + (int)(Pos.Y / 512);

        // вызвается, когда генерируется чанк
        public void LoadChunk()
        {
            if(new FileInfo("maps\\" + SandCore.map + "\\chunks\\" + GetName()).Exists)
            {
                using (StreamReader sr = new StreamReader("maps\\" + SandCore.map + "\\chunks\\" + GetName()))
                {
                    string line = "";
                    while (true)
                    {
                        line = sr.ReadLine();

                        if (line == null)
                            break;

                        Vector2 vec = new Vector2(Convert.ToInt32(line.Split('|')[2]), Convert.ToInt32(line.Split('|')[3]));
                        Block.CreateBlock(line.Split('|')[1], vec);
                    }
                }
            }
        }

        // вызывается когда удаляется чанк
        public void UnloadChunk()
        {
            var blocks = Block.Blocks;
            string data = "";

            for (int i = 0; i < blocks.Count; i++) // проходим по всем блокам
            {
                Block block = blocks[i];
                if(block.GetChunk() == this && block.isSaving) // если блок находится в этом чанке, то сохраняем его
                {
                    // текст записи
                    data += "BLOCK|" + block.Type + '|' + block.Pos.X + '|' + block.Pos.Y + '\n';
                    SandCore.game.Components.Remove(block);
                    Block.Blocks.Remove(block);
                }
            }

            using (StreamWriter sw = new StreamWriter($"maps\\" + SandCore.map + "\\chunks\\" + GetName()))
            {
                sw.Write(data);
            }
        }

    }
}
