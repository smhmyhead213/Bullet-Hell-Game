global using static System.MathF;
global using static bullethellwhatever.UtilitySystems.InputSystem;
global using static bullethellwhatever.MainFiles.GameInfoMethods;
global using static bullethellwhatever.Projectiles.CommonProjectileMethods;
global using static bullethellwhatever.UtilitySystems.ScreenManager;
global using static System.Diagnostics.Debug;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.AssetManagement;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.DrawCode;
using Microsoft.Xna.Framework.Audio;
using bullethellwhatever.UtilitySystems.SoundSystems;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System;
using FMOD.Studio;
using SharpDX.WIC;
using bullethellwhatever.DrawCode.UI;
using bullethellwhatever.Projectiles;
using bullethellwhatever.NPCs;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.DrawCode.Particles;

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
        public static VertexBuffer MainVertexBuffer;
        public static SpriteBatch _spriteBatch;
        public static Camera MainCamera;

        public static SpriteFont font;

        public static List<Particle> activeParticles = new List<Particle>();

        public static List<NPC> NPCsToAddNextFrame = new List<NPC>();

        public static MusicSystem musicSystem = new MusicSystem();

        public static SoundEffectInstance musicInstance;

        public static RenderTarget2D MainRT;

        public static Player player;

        public static readonly int GameHeight = 1080;
        public static readonly int GameWidth = 1920;

        public static int GameTime;

        public Main() : base()
        {
            MainInstance = this;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            _graphics.HardwareModeSwitch = false;
            _graphics.IsFullScreen = true;

            Window.AllowUserResizing = true;
            
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            // -- set up fmod --

            SoundSystem.SetUpFMOD();

            font = Content.Load<SpriteFont>("font");
        }

        protected override void Initialize()
        {          
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            PrimitiveManager.Initialise();

            InputSystem.Initialise();

            AssetRegistry.Initialise();

            Drawing.Initialise();

            UIManager.Initialise();

            MainCamera = new Camera();

            GameState.SetGameState(GameState.GameStates.TitleScreen);

            GameTime = 0;

            ResizingView = false;
            UpdateView();
            Window.ClientSizeChanged += WindowSizeChange;
            _graphics.DeviceReset += GraphicsManager_DeviceReset;
            _graphics.DeviceCreated += GraphicsManager_DeviceCreated;

            MainRT = new RenderTarget2D(GraphicsDevice, GameWidth, GameHeight);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            SoundSystem.Update();

            GameTime++;

            UpdateInputSystem();

            AssetRegistry.Update();

            if (MainInstance.IsActive)
            {
                // call me Odie the way i bark at that garfeild

                GameStateHandler.HandleGame();
                UIManager.ManageUI();

                if (musicSystem.ActiveSong is not null)
                    musicSystem.PlayMusic();
            }
            else
            {
                if (musicSystem.ActiveSong is not null)
                    MediaPlayer.Pause();
            }

            // to do: move this to a new camera Update method if something else needs updated as well
            MainCamera.UpdateVisibleArea();

            DialogueSystem.Update();

            Drawing.UpdateDrawer();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Drawing.Timer++;

            _spriteBatch.Begin(transformMatrix: MainCamera.Matrix);

            GraphicsDevice.SetRenderTarget(MainRT);

            GraphicsDevice.Clear(Color.Black);

            switch (GameState.State)
            {
                case GameState.GameStates.Credits:
                    Credits.CreditSequence(_spriteBatch);
                    break;
            }

            DrawGame.DrawTheGame(gameTime, _spriteBatch);
            UIManager.DrawUI(_spriteBatch);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Viewport = ScreenViewport;

            _spriteBatch.Begin(transformMatrix: ScreenMatrix);

            _spriteBatch.Draw(MainRT, Vector2.Zero, Color.White);

            _spriteBatch.End();

            // 6 and a half months later and this default comment is still there

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        public static void ChangeGraphicsDeviceTexture(int index, string texture)
        {
            _graphics.GraphicsDevice.Textures[index] = AssetRegistry.GetTexture2D(texture);
        }
    }
}