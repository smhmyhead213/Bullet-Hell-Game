using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;

namespace bullethellwhatever
{
    //to do 26th march 2023: fix those hitboxes 
    public class Main : Game
    {
 
        public static Main MainInstance
        {
            get;
            private set;
        }

        public static GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;

        public static Texture2D playerTexture;
        public static Texture2D startButton;
        public static Texture2D easyButton;
        public static Texture2D normalButton;
        public static Texture2D hardButton;
        public static Texture2D insaneButton;
        public static Texture2D bossButton;
        public static SpriteFont font;

        public static GameStateHandler gameStateHandler = new GameStateHandler();
        public static GameState gameState = new GameState();

        public static List<NPC> activeNPCs = new List<NPC>();
        public static List<Projectile> activeProjectiles = new List<Projectile>();
        public static List<FriendlyProjectile> activeFriendlyProjectiles = new List<FriendlyProjectile>();

        public static Player player = new Player();

        public static List<Button> activeButtons = new List<Button>();
        public Main() : base()
        {
            MainInstance = this;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //_graphics.IsFullScreen = true;
        }

        
        protected override void LoadContent()
        {
            
            // TODO: use this.Content to load your game content here
        }
        
        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
            playerTexture = Content.Load<Texture2D>("box");
            easyButton = Content.Load<Texture2D>("EasyButton");
            normalButton = Content.Load<Texture2D>("NormalButton");
            hardButton = Content.Load<Texture2D>("HardButton");
            insaneButton = Content.Load<Texture2D>("InsaneButton");
            bossButton = Content.Load<Texture2D>("BossButton");
            startButton = Content.Load<Texture2D>("StartButton");

            GameState.State = GameState.GameStates.TitleScreen;

            // TODO: Add your initialization logic here
            //player.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            //player.Velocity = new Vector2(100, 100);
            
            
            base.Initialize();
        }

        

        protected override void Update(GameTime gameTime)
        {
            

            if (MainInstance.IsActive)
            {
                gameStateHandler.HandleGame();

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                

                // TODO: Add your update logic here
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (GameState.State)
            {
                case GameState.GameStates.TitleScreen:
                    Drawing.DrawTitleScreen(_spriteBatch);
                    break;
                case GameState.GameStates.BossSelect:
                    Drawing.DrawBossSelect(_spriteBatch);
                    break;
                case GameState.GameStates.DifficultySelect:
                    Drawing.DrawDifficultySelect(_spriteBatch);
                    break;
                case GameState.GameStates.InGame:
                    Drawing.DrawGame();
                    break;
            }
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}