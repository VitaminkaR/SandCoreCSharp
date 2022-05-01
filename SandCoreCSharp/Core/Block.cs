using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SandCoreCSharp.Core
{
    public class Block : DrawableGameComponent
    {
        public const float BLOCK_SIZE = Terrain.TILE_SIZE;

        // все блоки
        public static List<Block> Blocks { get; private set; } = new List<Block>();
        // спрайты блоков
        static protected Dictionary<string, Texture2D> Sprites { get; private set; } = new Dictionary<string, Texture2D>();

        // глобальная позиция
        public Vector2 Pos { get; protected set; }

        protected ContentManager content;

        // спрайты
        protected Texture2D sprite;
        protected Graphics graphics;

        // взаимодействия
        protected Hero player;
        protected Camera camera;
        protected Terrain terrain;

        // чанк в котором зарегестрирован блок
        private Chunk thisChunk;

        // Коллайдер
        public Rectangle collider;

        // тэг для определения типа (ОБЯЗАТЕЛЬНЫЙ ПРИ СОЗДАНИИ НОВОГО БЛОКА)
        public string Type { get; protected set; } = "example";

        // будет ли блок сохранятся
        public bool isSaving = true;

        // параметры при создании блока
        // прочность блока (сколько секунд будет разрушаться) -параметр-
        public int Hardness { get; protected set; } = 1;
        // имеет ли блок коллизию
        public bool IsSolid { get; protected set; } = true;
        // какой инструмент нужен для добычи // маленькими буквами
        public string Instrument { get; protected set; } = null;
        // информация, которая сохраняется в файл блока
        public string[] Tags { get; set; } = new string[8];



        // фабрика блоков
        static private BlockFabric fabric = new BlockFabric();



        public Block(Game game, Vector2 pos) : base(game)
        {
            Type = this.GetType().Name.ToLower();

            content = Game.Content;
            graphics = new Graphics(game.GraphicsDevice);
            Game.Components.Add(this);
            Blocks.Add(this);  

            player = SandCore.hero;
            camera = SandCore.camera;
            terrain = SandCore.terrain;

            Pos = pos;

            collider = new Rectangle(Pos.ToPoint(), new Point(32, 32));

            DrawRect(pos.X, pos.Y);
        }


        protected override void LoadContent()
        {
            sprite = Sprites[Type];
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            thisChunk = terrain.GetChunk(Pos);

            // если нажата правая кнопка на блоке
            Vector2 CursorCollider = SandCore.cursor.Pos;
            if (Pos == CursorCollider && Mouse.GetState().RightButton == ButtonState.Pressed)
                Using();

            base.Update(gameTime);
        }

        // отрисовка спрайта в позиции
        public override void Draw(GameTime gameTime)
        {
            graphics.Texture = sprite;
            graphics.Drawing();

            base.Draw(gameTime);
        }

        private void DrawRect(float x, float y)
        {
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x, y, 0), Color.White, new Vector2(0, 0)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x + BLOCK_SIZE, y, 0), Color.White, new Vector2(1, 0)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x + BLOCK_SIZE, y - BLOCK_SIZE, 0), Color.White, new Vector2(1, 1)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);
            graphics.Vertices.Add(new VertexPositionColorTexture(new Vector3(x, y - BLOCK_SIZE, 0), Color.White, new Vector2(0, 1)));
            graphics.Indices.Add(graphics.Vertices.Count - 1);

            graphics.Indices.Add(graphics.Vertices.Count - 4);
            graphics.Indices.Add(-1);
        }

        // когда блок ломается
        public virtual void Break()
        {
            Resources resources = SandCore.resources;
            resources.AddResource(Type, 1);

            Unload();
            Rectangle collider = new Rectangle(Pos.ToPoint(), new Point(32, 32));
        }

        // столкновение с игроком
        public virtual void CollidePlayer(Hero player)
        { }

        // раньше было методом break, но этот метод един для всех блоков
        public void Unload()
        {
            Game.Components.Remove(this);
            Blocks.Remove(this);
        }



        // когда на блок нажали правой кнопко мыши 
        protected virtual void Using()
        {

        }



        // загружает текстуры
        static public void LoadContents(ContentManager content)
        {
            Sprites["wood"] = content.Load<Texture2D>("Wood");
            Sprites["quarry"] = content.Load<Texture2D>("Quarry");
            Sprites["lumberjack"] = content.Load<Texture2D>("Lumberjack");
            Sprites["mine"] = content.Load<Texture2D>("Mine");
            Sprites["inductionfurnace"] = content.Load<Texture2D>("InductionFurnace");
            Sprites["furnace"] = content.Load<Texture2D>("Furnace");
            Sprites["coalgenerator"] = content.Load<Texture2D>("CoalGenerator");
            Sprites["land"] = content.Load<Texture2D>("Land");
            Sprites["farmer"] = content.Load<Texture2D>("Farmer");
            Sprites["mud"] = content.Load<Texture2D>("Mud");
            Sprites["mud_with_seeds"] = content.Load<Texture2D>("MudWithSeeds");
            Sprites["wheat_1"] = content.Load<Texture2D>("wheat_1");
            Sprites["wheat_2"] = content.Load<Texture2D>("wheat_2");
            Sprites["cotton_1"] = content.Load<Texture2D>("cotton_1");
            Sprites["cotton_2"] = content.Load<Texture2D>("cotton_2");
            Sprites["cotton_3"] = content.Load<Texture2D>("cotton_3");
        }

        // регистрирует новые блоки (с параметром isSaving = true)
        static public Block CreateBlock(string type, Vector2 pos, bool loader = false)
        {
            // проходим и проверяем, если там уже стоит блок, то новый не создаем
            for (int i = 0; i < Blocks.Count; i++)
            {
                Block b = Blocks[i];
                if (b.Pos == pos)
                    return null;
            }

            Block block = fabric.Create(type, pos);

            if(block != null)
            {
                if (!loader)
                {
                    Resources resources = SandCore.resources;
                    resources.AddResource(block.Type, -1);
                }

                return block;
            }

            return null;
        }

        // ищет блок по позиции
        static public Block FindBlock(Vector2 pos) => Blocks.Find(block => block.Pos == pos);



        // запускается когда выгружается чанк, для сохранения блоков в этом чанке
        static public void UnloadChunk(Chunk chunk)
        {
            List<Block> blocks = Blocks.FindAll((Block block) => block.thisChunk == chunk);

            string data = "";
            for (int i = 0; i < blocks.Count; i++)
            {
                data += $"{blocks[i].Type}|{blocks[i].Pos.X}|{blocks[i].Pos.Y}|{JsonSerializer.Serialize(blocks[i].Tags, typeof(string[]))}\n";
                blocks[i].Unload();
            }
                

            string path = $"maps\\{SandCore.map}\\blocks\\{chunk.GetName()}";
            // можно заменить на using конструкцию
            StreamWriter sw = new StreamWriter(path, false);
            sw.Write(data);
            sw.Close();
        }

        // запускается при генерации чанка, загружает блоки
        static public void LoadChunks(Chunk chunk)
        {
            string path = $"maps\\{SandCore.map}\\blocks\\{chunk.GetName()}";
            bool exist = new FileInfo(path).Exists;
            string[] data = new string[0];
            if (exist)
            {
                StreamReader sr = new StreamReader(path);
                data = sr.ReadToEnd().Split('\n');
                sr.Close();
            }

            if (data.Length == 0)
                return;

            for (int i = 0; i < data.Length; i++)
            {
                string[] info = data[i].Split('|');
                if(data[i] != "")
                {
                    Block block = fabric.Create(info[0], new Vector2((float)Convert.ToDouble(info[1]), (float)Convert.ToDouble(info[2])));
                    // загружаем тэги
                    block.Tags = (string[])JsonSerializer.Deserialize(info[3], typeof(string[]));
                } 
            }
        }
    }
}
