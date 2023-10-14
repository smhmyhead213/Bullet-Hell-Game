global using static System.MathF;
global using static bullethellwhatever.UtilitySystems.InputSystem;
global using static bullethellwhatever.MainFiles.GameInfoMethods;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using bullethellwhatever.Buttons;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Player;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.DrawCode;
using Microsoft.Xna.Framework.Audio;
using bullethellwhatever.UtilitySystems.SoundSystems;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System;

namespace bullethellwhatever.MainFiles
{
    //to do 26th march 2023: fix those hitboxes 
    public class Main : Game
    {
        public static Main MainInstance
        {
            get;
            private set;
        }

        public float ZoomFactor = 1f;


        public static GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;

        public static Dictionary<string, Texture2D> Assets = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Effect> Shaders = new Dictionary<string, Effect>();
        public static Dictionary<string, SoundEffect> Music = new Dictionary<string, SoundEffect>();
        public static Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();

        public static SpriteFont font;

        public static GameStateHandler gameStateHandler = new GameStateHandler();
        public static GameState gameState = new GameState();

        public static List<NPC> activeNPCs = new List<NPC>();
        public static List<NPC> activeFriendlyNPCs = new List<NPC>();
        public static List<Projectile> activeProjectiles = new List<Projectile>();
        public static List<Projectile> activeFriendlyProjectiles = new List<Projectile>();
        public static List<Projectile> enemyProjectilesToAddNextFrame = new List<Projectile>();
        public static List<Projectile> friendlyProjectilesToAddNextFrame = new List<Projectile>();
        public static List<NPC> NPCsToAddNextFrame = new List<NPC>();

        public static MusicSystem musicSystem = new MusicSystem();

        public static SoundEffectInstance musicInstance;

        public static Player player = new Player();

        public static Vector2 RawScreenArea;

        public static int ScreenHeight;
        public static int ScreenWidth;

        public static int IdealScreenHeight = 1080;
        public static int IdealScreenWidth = 1920;

        public static int GameTime;

        public static List<Button> activeButtons = new List<Button>();
        public Main() : base()
        {
            MainInstance = this;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            ScreenWidth = _graphics.PreferredBackBufferWidth;
            ScreenHeight = _graphics.PreferredBackBufferHeight;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //_graphics.IsFullScreen = true;
        }
        //public Matrix GamePerspective
        //{
        //    get
        //    {
        //        RawScreenArea = new(MainInstance.GraphicsDevice.Viewport.Width, MainInstance.GraphicsDevice.Viewport.Height);

        //        Vector2 zoom = RawScreenArea / new Vector2(IdealScreenWidth, IdealScreenHeight) * ZoomFactor;
        //        return Matrix.CreateScale(zoom.X, zoom.Y, 1f);
        //    }
        //}

        protected override void LoadContent()
        {
            string[] files = Directory.GetFiles("Content", "", SearchOption.AllDirectories);

            int numberOfSections = 8; // how many sections to split list into

            int filesPerTask = files.Length / numberOfSections;

            Task[] tasks = new Task[numberOfSections + 1]; // extra section for remainder

            for (int i = 0; i < tasks.Length - 1; i++)
            {
                int index = i;

                Action loadInSection = () =>
                {
                    for (int j = files.Length * index; j < files.Length * (index + 1); j++)
                    {
                        AssetLoader.LoadIn(files[j]);
                    }
                };

                tasks[i] = new Task(loadInSection);
            }

            Action dealWithRemainder = () =>
            {
                for (int i = (files.Length / numberOfSections) * numberOfSections; i < files.Length; i++)// the i initialisation takes advantage of integer division to get the first index of remainder
                {
                    AssetLoader.LoadIn(files[i]);
                }
            };

            tasks[tasks.Length - 1] = new Task(dealWithRemainder);

            Task.WaitAll(tasks, 3000);

            font = Content.Load<SpriteFont>("font");
        }
        
        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            GameState.State = GameState.GameStates.TitleScreen;

            Drawing.Initialise();

            GameTime = 0;

            base.Initialize();
        }

        

        protected override void Update(GameTime gameTime)
        {

            //MousePosition = MousePosition * (RawScreenArea / new Vector2(IdealScreenWidth, IdealScreenHeight));

            GameTime++;

            UpdateInputSystem();

            if (MainInstance.IsActive)
            {
                gameStateHandler.HandleGame();

                if (musicSystem.ActiveSong is not null)
                    musicSystem.ActiveSong.Play();

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
            }
            else
            {
                if (musicSystem.ActiveSong is not null)
                    musicSystem.ActiveSong.Pause();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Drawing.Timer++;

            _spriteBatch.Begin();

            _spriteBatch.DrawString(font, MousePosition.ToString(), new Vector2(ScreenWidth / 3, ScreenHeight / 3 + 10), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            switch (GameState.State)
            {
                case GameState.GameStates.TitleScreen:
                    UI.DrawTitleScreen(_spriteBatch);
                    break;
                case GameState.GameStates.BossSelect:
                    UI.DrawBossSelect(_spriteBatch);
                    break;
                case GameState.GameStates.DifficultySelect:
                    UI.DrawDifficultySelect(_spriteBatch);
                    break;
                case GameState.GameStates.Settings:
                    UI.DrawSettings(_spriteBatch);
                    break;
                case GameState.GameStates.InGame:
                    DrawGame.DrawTheGame(gameTime);
                    break;
                case GameState.GameStates.Credits:
                    Credits.CreditSequence(_spriteBatch);
                    break;
            }

            UI.DrawButtons(_spriteBatch);

            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}