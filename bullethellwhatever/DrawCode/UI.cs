using bullethellwhatever.BaseClasses;
using bullethellwhatever.Buttons;
using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace bullethellwhatever.DrawCode
{
    public static class UI
    {
        public static void DrawButtons(SpriteBatch spriteBatch)
        {
            foreach (Button button in Main.activeButtons)
            {
                button.DrawButton(spriteBatch);
            }

        }
        public static void DrawHealthBar(SpriteBatch _spriteBatch, Entity entityToDrawHPBarFor, Vector2 positionOfBar, float BarWidth, float BarHeight) //bar width and height are SCALE FACTORS DO NOT FORGET
        {
            float healthRatio = entityToDrawHPBarFor.Health / entityToDrawHPBarFor.MaxHP;

            float emptySpaceOnLeft = BarWidth * (1 - healthRatio) / 2;

            Texture2D texture = Assets["box"];

            Vector2 topLeft = new(positionOfBar.X - BarWidth / 2 * texture.Width, positionOfBar.Y - BarHeight / 2 * texture.Height);

            //RectangleButGood HPBar = new(topLeft.X, topLeft.Y, (BarWidth * Main.player.Texture.Width), (BarHeight * Main.player.Texture.Height));

            RotatedRectangle HPBar = new RotatedRectangle(0, BarWidth * texture.Width, BarHeight * texture.Height, positionOfBar, entityToDrawHPBarFor);
            HPBar.UpdateVertices();

            float opacity = HPBar.Intersects(player.Hitbox) ? 0.2f : 1f;

            //HP bar background.
            if (entityToDrawHPBarFor is not Player)
            {
                _spriteBatch.Draw(Main.player.Texture, positionOfBar, null, Color.LimeGreen * opacity, 0f, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(BarWidth, BarHeight), SpriteEffects.None, 0f);

                _spriteBatch.Draw(Main.player.Texture, new Vector2(positionOfBar.X - emptySpaceOnLeft * Main.player.Texture.Width, positionOfBar.Y), null, Color.Red * opacity, 0f, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(BarWidth * healthRatio, BarHeight), SpriteEffects.None, 0f);
            }

            else
            {
                _spriteBatch.Draw(Main.player.Texture, positionOfBar, null, Color.Red * opacity, 0f, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(BarWidth, BarHeight), SpriteEffects.None, 0f);

                _spriteBatch.Draw(Main.player.Texture, new Vector2(positionOfBar.X - emptySpaceOnLeft * Main.player.Texture.Width, positionOfBar.Y), null, Color.LimeGreen * opacity, 0f, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(BarWidth * healthRatio, BarHeight), SpriteEffects.None, 0f);

            }
        }

        public static void DrawTitleScreen(SpriteBatch _spriteBatch)
        {
            Utilities.drawTextInDrawMethod(
                "Controls: WASD to move, Left Click or Enter to shoot, Space to dash, Shift for precision.", new Vector2(Main.ScreenWidth / 2, Main.ScreenHeight / 5 * 2), _spriteBatch, Main.font, Color.White);

            Button startButton = new Button(new Vector2(Main._graphics.PreferredBackBufferWidth / 4, Main._graphics.PreferredBackBufferHeight / 2), "StartButton",
                GameState.GameStates.BossSelect, null, new Vector2(3, 3));

            Button settingsButton = new Button(new Vector2(Main._graphics.PreferredBackBufferWidth / 4 * 2, Main._graphics.PreferredBackBufferHeight / 2), "SettingsButton",
                GameState.GameStates.Settings, null, new Vector2(3, 3));

            Button creditsButton = new Button(new Vector2(Main._graphics.PreferredBackBufferWidth / 4 * 3, Main._graphics.PreferredBackBufferHeight / 2), "StartButton",
                GameState.GameStates.Credits, null, new Vector2(3, 3));
           
        }

        public static void DrawBossSelect(SpriteBatch _spriteBatch)
        {
            BossSelectButton testBossButton = new BossSelectButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 4, Main._graphics.PreferredBackBufferHeight / 2), "BossButton",
                GameState.GameStates.DifficultySelect, GameState.Bosses.TestBoss, new Vector2(3, 3), true);

            BossSelectButton secondBossButton = new BossSelectButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 4 * 2, Main._graphics.PreferredBackBufferHeight / 2), "BossButton",
                GameState.GameStates.DifficultySelect, GameState.Bosses.SecondBoss, new Vector2(3, 3), true);

            BossSelectButton crabBossButton = new BossSelectButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 4 * 3, Main._graphics.PreferredBackBufferHeight / 2), "BossButton",
                GameState.GameStates.DifficultySelect, GameState.Bosses.CrabBoss, new Vector2(3, 3), true);

            Button backButton = new Button(new Vector2(Main._graphics.PreferredBackBufferWidth / 5, Main._graphics.PreferredBackBufferHeight / 5), "Back",
                GameState.GameStates.TitleScreen, null, new Vector2(3, 3));
        }

        public static void DrawDifficultySelect(SpriteBatch _spriteBatch)
        { //add this to other menus

            string[] buttonTexturesToDraw = { "EasyButton", "NormalButton", "HardButton", "InsaneButton" };

            Button backButton = new Button(new Vector2(ScreenWidth / 5, Main._graphics.PreferredBackBufferHeight / 5), "Back",
                GameState.GameStates.BossSelect, null, new Vector2(3, 3));
            

            int counter = 1;

            foreach (string texture in buttonTexturesToDraw)
            {
                Button button = new Button(new Vector2(Main._graphics.PreferredBackBufferWidth / (buttonTexturesToDraw.Length + 1) * counter, Main._graphics.PreferredBackBufferHeight / 2), texture,
                    GameState.GameStates.InGame, (GameState.Difficulties)(counter - 1), new Vector2(3, 3));

                counter++;
            }          
        }

        public static void DrawSettings(SpriteBatch _spriteBatch)
        {
            SettingsControlSchemeButton numberKeysButton = new SettingsControlSchemeButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 3, Main._graphics.PreferredBackBufferHeight / 2), "NumberKeys",
                new Vector2(3, 3), false);

            SettingsControlSchemeButton scrollWheelButton = new SettingsControlSchemeButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 3 * 2, Main._graphics.PreferredBackBufferHeight / 2), "Scroll",
                new Vector2(3, 3), true);

            //using a titlescreenbutton as it has a destination, which is what i need

            Button backButton = new Button(new Vector2(Main._graphics.PreferredBackBufferWidth / 5, Main._graphics.PreferredBackBufferHeight / 5), "Back",
                GameState.GameStates.TitleScreen, null, new Vector2(3, 3));

            Drawing.ConfirmControlSettingsChange(_spriteBatch);

        }
    }
}
