global using static System.MathF;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

using bullethellwhatever.Buttons;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Player;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.DrawCode;
using Microsoft.Xna.Framework.Audio;
using bullethellwhatever.UtilitySystems.SoundSystems;
using Microsoft.Xna.Framework.Media;

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

        public static Vector2 MousePosition;

        public static Vector2 RawScreenArea;

        public static int ScreenHeight;
        public static int ScreenWidth;

        public static int IdealScreenHeight = 1080;
        public static int IdealScreenWidth = 1920;

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

            for (int i = 0; i < files.Length; i++)
            {
                string toSaveAs = files[i];

                while (toSaveAs.Contains("\\")) //remove all slashes
                {
                    int indexOfSlash = toSaveAs.IndexOf("\\");

                    int startIndex = indexOfSlash + 1; //+ 1 accounts for double slash, there's two slashes in the IndexOf cos the first one is to character escape the second

                    toSaveAs = toSaveAs.Substring(startIndex, toSaveAs.Length - startIndex); //only take everything after the double slash
                }

                int indexOfDot = toSaveAs.IndexOf(".");

                string fileExtension = toSaveAs.Substring(indexOfDot, toSaveAs.Length - indexOfDot); // remove file extension

                toSaveAs = toSaveAs.Substring(0, indexOfDot);

                if (fileExtension == ".xnb") //only do this for .xnbs, when this was written credits.txt would crash it as a .txt doesnt work
                {
                    toSaveAs = toSaveAs.Substring(0, indexOfDot); //remove file extension

                    if (toSaveAs.Contains("Shader")) // This system only works if you use a naming convention that matches this
                    {
                        if (!(Shaders.ContainsKey(toSaveAs)))
                            Shaders.Add(toSaveAs, Content.Load<Effect>("Shaders/" + toSaveAs));
                    }
                    else if (toSaveAs.Contains("Music"))
                    {
                        if (!(Music.ContainsKey(toSaveAs)))
                            Music.Add(toSaveAs, Content.Load<SoundEffect>("Music/" + toSaveAs));
                    }
                    else if (toSaveAs.Contains("Sound"))
                    {
                        if (!(Sounds.ContainsKey(toSaveAs)))
                            Sounds.Add(toSaveAs, Content.Load<SoundEffect>(toSaveAs));
                    }
                    else if (toSaveAs != "font")
                    {
                        if (!(Assets.ContainsKey(toSaveAs)))
                            Assets.Add(toSaveAs, Content.Load<Texture2D>(toSaveAs));
                    }
                }
            }

            font = Content.Load<SpriteFont>("font");
        }
        
        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            GameState.State = GameState.GameStates.TitleScreen;

            Drawing.Initialise();

            base.Initialize();
        }

        

        protected override void Update(GameTime gameTime)
        {
            MousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            //MousePosition = MousePosition * (RawScreenArea / new Vector2(IdealScreenWidth, IdealScreenHeight));

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