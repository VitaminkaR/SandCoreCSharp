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
        // 5 - sand
        public byte[,,] Tiles { get; set; } = new byte[16, 16, 16];

        // позиция чанка
        public Vector2 Pos { get; private set; }


        public Chunk(float x, float y) => Pos = new Vector2(x, y);


        // возвращает имя чанка
        public string GetName() => "C=" + (int)(Pos.X / Terrain.CHUNK_SIZE) + ";" + (int)(Pos.Y / Terrain.CHUNK_SIZE);
    }
}
