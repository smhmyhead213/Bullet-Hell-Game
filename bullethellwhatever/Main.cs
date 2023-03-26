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
        public static SpriteFont font; 


        public static List<NPC> activeNPCs = new List<NPC>();
        public static List<Projectile> activeProjectiles = new List<Projectile>();
        public static List<FriendlyProjectile> activeFriendlyProjectiles = new List<FriendlyProjectile>();

        public static Player player = new Player();
        public Main() : base()
        {
            MainInstance = this;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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


            // TODO: Add your initialization logic here
            //player.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            //player.Velocity = new Vector2(100, 100);
            EntityManager.SpawnBoss();
            player.Spawn(new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), new Vector2(5, 5), 10, playerTexture);
            
            base.Initialize();
        }

        

        protected override void Update(GameTime gameTime)
        {
            if (MainInstance.IsActive)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                EntityManager.RemoveEntites(); //remove all entities queued for deletion
                EntityManager.RunAIs();

                // TODO: Add your update logic here
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            //player drawing

            Drawing.DrawGame();

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}