using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Core.Blocks;
using SandCoreCSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace SandCoreCSharp.Core
{
    public class Block : DrawableGameComponent
    {
        const int LoaderDistance = 4096;

        // все блоки
        public static List<Block> Blocks { get; private set; } = new List<Block>();
        // спрайты блоков
        static protected Dictionary<string, Texture2D> Sprites { get; private set; } = new Dictionary<string, Texture2D>();

        // глобальная позиция
        public Vector2 Pos { get; protected set; }

        protected ContentManager content;
        protected SpriteBatch spriteBatch;

        // спрайты
        protected Texture2D sprite;

        // взаимодействия
        protected Hero player;
        protected Camera camera;
        protected Terrain terrain;

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

        // загруженые чанки
        static public List<string> loadChunks = new List<string>();

        // для работы с загрузками
        protected string path; // сам путь до файла,  в котором можно хранить, что требуется
        protected string directory; // директория (название чанка блока)
        protected FileInfo file; // информация о файлах

        // информация, которая сохраняется в файл блока
        protected string Tags { get; set; }

        public Block(Game game, Vector2 pos) : base(game)
        {
            Type = this.GetType().Name.ToLower();

            content = Game.Content;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            Game.Components.Add(this);
            Blocks.Add(this);

            player = SandCore.hero;
            camera = SandCore.camera;
            terrain = SandCore.terrain;

            Pos = pos;

            collider = new Rectangle(Pos.ToPoint(), new Point(32, 32));

            // для работы с загрузками
            // находим index чанка
            Chunk chunk = terrain.GetChunk(Pos.X, Pos.Y);
            directory = "maps\\" + SandCore.map + "\\blocks" + $"\\{chunk.GetName()}";
            string data = $"\\.{Pos.X}.{Pos.Y}.{Type}";
            path = directory + data;
            file = new FileInfo(path);
        }



        protected override void LoadContent()
        {
            sprite = Sprites[Type];

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // выгрузка блоков
            Hero hero = SandCore.hero;
            Vector2 pos = hero.Pos;
            float r = MathF.Sqrt(MathF.Pow(pos.X - Pos.X, 2) + MathF.Pow(pos.Y - Pos.Y, 2)); // ищем расстояние
            if (r > LoaderDistance)
                Unload();

            // если нажата правая кнопка на блоке
            Vector2 CursorCollider = SandCore.cursor.Pos;
            if (Pos == CursorCollider && Mouse.GetState().RightButton == ButtonState.Pressed)
                Using();

            base.Update(gameTime);
        }

        // отрисовка спрайта в позиции
        public override void Draw(GameTime gameTime)
        {
            Color color = Color.White;

            Cursor cursor = SandCore.cursor;
            // добыча блока
            if (cursor.Pos == Pos)
                color = Color.Green;
            // если игрок не может сломать
            if (cursor.Pos == Pos && Mouse.GetState().LeftButton == ButtonState.Pressed)
                color = Color.Black;
            // добыча блока
            if (cursor.Pos == Pos && cursor.breaking)
                color = Color.Red;

            spriteBatch.Begin();
            Vector2 pos = Pos - camera.Pos;
            if (sprite != null)
                spriteBatch.Draw(sprite, pos, color);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // когда блок ломается
        public virtual void Break()
        {
            Resources resources = SandCore.resources;
            resources.AddResource(Type, 1);

            DeleteBlock();
            Unload();
            Rectangle collider = new Rectangle(Pos.ToPoint(), new Point(32, 32));
        }

        // столкновение с игроком
        public virtual void CollidePlayer(Hero player)
        { }

        // чанк в котором блок
        public Chunk GetChunk() => SandCore.terrain.GetChunk(Pos.X, Pos.Y);

        // раньше было методом break, но этот метод един для всех блоков
        public void Unload()
        {
            Game.Components.Remove(this);
            Blocks.Remove(this);
        }

        // сохранение блока
        public void SaveBlock()
        {
            Directory.CreateDirectory(directory);
            File.Create(path);
            SaveTags();
        }

        // удаление этого блока
        public void DeleteBlock()
        {
            try
            {
                File.Delete(path);
            }
            catch { }
        }

        // сохранение тэгов
        protected void SaveTags() => FileWork.Write(path, Tags);

        // загрузка тэгов
        protected void LoadTags()
        {
            string[] msg = FileWork.Read(path);
            if (msg.Length > 0)
                Tags = msg[0];
            else
                SaveTags();
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
            Sprites["induction_furnace"] = content.Load<Texture2D>("InductionFurnace");
            Sprites["furnace"] = content.Load<Texture2D>("Furnace");
            Sprites["coal_generator"] = content.Load<Texture2D>("CoalGenerator");
            Sprites["land"] = content.Load<Texture2D>("Land");
            Sprites["mud"] = content.Load<Texture2D>("Mud");
            Sprites["mud_with_seeds"] = content.Load<Texture2D>("MudWithSeeds");
        }

        // загружает блоки
        static public void LoadBlocks(Terrain terrain)
        {
            for (int i = 0; i < terrain.Chunks.Count; i++)
            {
                Chunk chunk = terrain.Chunks[i];
                if (loadChunks.Contains(chunk.GetName()))
                    continue;
                loadChunks.Add(chunk.GetName());

                string[] files = new string[0];

                if (new DirectoryInfo("maps\\" + SandCore.map + "\\blocks" + $"\\{chunk.GetName()}").Exists)
                    files = Directory.GetFiles("maps\\" + SandCore.map + "\\blocks" + $"\\{chunk.GetName()}");

                for (int j = 0; j < files.Length; j++)
                {
                    string file = files[j];
                    string[] data = file.Split('.');
                    int x = Convert.ToInt32(data[1]);
                    int y = Convert.ToInt32(data[2]);
                    string type = data[3];

                    Block block = CreateBlock(type, new Vector2(x, y), true);
                    block?.LoadTags();
                }
            }
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

            Block block = null;

            if (type == "wood")
                block = new Wood(SandCore.game, pos);
            if (type == "furnace")
                block = new Furnace(SandCore.game, pos);
            if (type == "mine")
                block = new Mine(SandCore.game, pos);
            if (type == "lumberjack")
                block = new Lumberjack(SandCore.game, pos);
            if (type == "coal_generator")
                block = new CoalGenerator(SandCore.game, pos);
            if (type == "quarry")
                block = new Quarry(SandCore.game, pos);
            if (type == "induction_furnace")
                block = new InductionFurnace(SandCore.game, pos);
            if (type == "land")
                block = new Land(SandCore.game, pos);

            if(block != null)
            {
                return block;

                Resources resources = SandCore.resources;
                resources.AddResource(block.Type, -1);

                if (!loader)
                    block.SaveBlock();
            }

            return null;
        }

        // ищет блок по позиции
        static public Block FindBlock(Vector2 pos) => Blocks.Find(block => block.Pos == pos);
    }
}
