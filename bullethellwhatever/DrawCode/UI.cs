using bullethellwhatever.BaseClasses;
using bullethellwhatever.Buttons;
using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace bullethellwhatever.DrawCode
{
    public static class UI
    {
        public static void DrawHealthBar(SpriteBatch _spriteBatch, Entity entityToDrawHPBarFor, Vector2 positionOfBar, float BarWidth, float BarHeight) //bar width and height are SCALE FACTORS DO NOT FORGET
        {
            float healthRatio = entityToDrawHPBarFor.Health / entityToDrawHPBarFor.MaxHP;

            float emptySpaceOnLeft = BarWidth * (1 - healthRatio) / 2;

            Vector2 topLeft = new(positionOfBar.X - BarWidth / 2 * Main.player.Texture.Width, positionOfBar.Y - BarHeight / 2 * Main.player.Texture.Height);

            Rectangle HPBar = new((int)topLeft.X, (int)topLeft.Y, (int)(BarWidth * Main.player.Texture.Width), (int)(BarHeight * Main.player.Texture.Height));

            float opacity = HPBar.Intersects(Main.player.Hitbox) ? 0.2f : 1f;

            //debug key




            //HP bar foreground.
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
            TitleScreenButton startButton = new TitleScreenButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 4, Main._graphics.PreferredBackBufferHeight / 2), Main.startButton,
                GameState.GameStates.BossSelect, new Vector2(3, 3));

            TitleScreenButton settingsButton = new TitleScreenButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 4 * 2, Main._graphics.PreferredBackBufferHeight / 2), Main.settingsButton,
                GameState.GameStates.Settings, new Vector2(3, 3));

            TitleScreenButton creditsButton = new TitleScreenButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 4 * 3, Main._graphics.PreferredBackBufferHeight / 2), Main.startButton,
                GameState.GameStates.Credits, new Vector2(3, 3));

            if (Main.activeButtons.Count < 3)
                Main.activeButtons.Add(startButton);

            if (Main.activeButtons.Count < 3)
                Main.activeButtons.Add(settingsButton);

            if (Main.activeButtons.Count < 3)
                Main.activeButtons.Add(creditsButton);

            
            _spriteBatch.Draw(startButton.Texture, startButton.Position, null, Color.White, 0f, new Vector2(startButton.Texture.Width / 2, startButton.Texture.Height / 2), startButton.Scale, SpriteEffects.None, 0f);
            _spriteBatch.Draw(settingsButton.Texture, settingsButton.Position, null, Color.White, 0f, new Vector2(settingsButton.Texture.Width / 2, settingsButton.Texture.Height / 2), settingsButton.Scale, SpriteEffects.None, 0f);
            _spriteBatch.Draw(creditsButton.Texture, creditsButton.Position, null, Color.White, 0f, new Vector2(creditsButton.Texture.Width / 2, creditsButton.Texture.Height / 2), creditsButton.Scale, SpriteEffects.None, 0f);

            
        }

        public static void DrawBossSelect(SpriteBatch _spriteBatch)
        {
            BossSelectButton testBossButton = new BossSelectButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 3, Main._graphics.PreferredBackBufferHeight / 2), Main.bossButton,
                GameState.GameStates.DifficultySelect, GameState.Bosses.TestBoss, new Vector2(3, 3), true);

            BossSelectButton secondBossButton = new BossSelectButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 3 * 2, Main._graphics.PreferredBackBufferHeight / 2), Main.bossButton,
                GameState.GameStates.DifficultySelect, GameState.Bosses.SecondBoss, new Vector2(3, 3), true);

            TitleScreenButton backButton = new TitleScreenButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 5, Main._graphics.PreferredBackBufferHeight / 5), Main.backButton,
                GameState.GameStates.TitleScreen, new Vector2(3, 3));

            if (!Main.activeButtons.Contains(testBossButton))
                Main.activeButtons.Add(testBossButton);

            if (!Main.activeButtons.Contains(backButton))
                Main.activeButtons.Add(backButton);

            if (!Main.activeButtons.Contains(secondBossButton))
                Main.activeButtons.Add(secondBossButton);

            
            _spriteBatch.Draw(testBossButton.Texture, testBossButton.Position, null, Color.White, 0f, new Vector2(testBossButton.Texture.Width / 2, testBossButton.Texture.Height / 2), new Vector2(3, 3), SpriteEffects.None, 0f);
            _spriteBatch.Draw(secondBossButton.Texture, secondBossButton.Position, null, Color.White, 0f, new Vector2(secondBossButton.Texture.Width / 2, secondBossButton.Texture.Height / 2), new Vector2(3, 3), SpriteEffects.None, 0f);
            _spriteBatch.Draw(backButton.Texture, backButton.Position, null, Color.White, 0f, new Vector2(backButton.Texture.Width / 2, backButton.Texture.Height / 2), new Vector2(3, 3), SpriteEffects.None, 0f);
            
        }

        public static void DrawDifficultySelect(SpriteBatch _spriteBatch)
        { //add this to other menus

            Texture2D[] buttonTexturesToDraw = { Main.easyButton, Main.normalButton, Main.hardButton, Main.insaneButton };

            TitleScreenButton backButton = new TitleScreenButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 5, Main._graphics.PreferredBackBufferHeight / 5), Main.backButton,
                GameState.GameStates.BossSelect, new Vector2(3, 3));

            if (!Main.activeButtons.Contains(backButton))
                Main.activeButtons.Add(backButton);

            

            int counter = 1;

            foreach (Texture2D texture in buttonTexturesToDraw)
            {
                DifficultySelectButton button = new DifficultySelectButton(new Vector2(Main._graphics.PreferredBackBufferWidth / (buttonTexturesToDraw.Length + 1) * counter, Main._graphics.PreferredBackBufferHeight / 2), texture,
                    GameState.GameStates.InGame, (GameState.Difficulties)(counter - 1), new Vector2(3, 3));

                if (!Main.activeButtons.Contains(button))
                    Main.activeButtons.Add(button);

                _spriteBatch.Draw(button.Texture, button.Position, null, Color.White, 0f, new Vector2(button.Texture.Width / 2, button.Texture.Height / 2), new Vector2(3, 3), SpriteEffects.None, 0f);

                counter++;
            }

            _spriteBatch.Draw(backButton.Texture, backButton.Position, null, Color.White, 0f, new Vector2(backButton.Texture.Width / 2, backButton.Texture.Height / 2), new Vector2(3, 3), SpriteEffects.None, 0f);


            


        }

        public static void DrawSettings(SpriteBatch _spriteBatch)
        {
            SettingsControlSchemeButton numberKeysButton = new SettingsControlSchemeButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 3, Main._graphics.PreferredBackBufferHeight / 2), Main.numberKeysButton,
                new Vector2(3, 3), false);

            SettingsControlSchemeButton scrollWheelButton = new SettingsControlSchemeButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 3 * 2, Main._graphics.PreferredBackBufferHeight / 2), Main.scrollWheelButton,
                new Vector2(3, 3), true);

            //using a titlescreenbutton as it has a destination, which is what i need

            TitleScreenButton backButton = new TitleScreenButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 5, Main._graphics.PreferredBackBufferHeight / 5), Main.backButton,
                GameState.GameStates.TitleScreen, new Vector2(3, 3));

            if (Main.activeButtons.Count < 3)
                Main.activeButtons.Add(numberKeysButton);

            if (Main.activeButtons.Count < 3)
                Main.activeButtons.Add(scrollWheelButton);

            if (Main.activeButtons.Count < 3)
                Main.activeButtons.Add(backButton);

            

            Drawing.ConfirmControlSettingsChange(_spriteBatch);

            _spriteBatch.Draw(numberKeysButton.Texture, numberKeysButton.Position, null, Color.White, 0f, new Vector2(numberKeysButton.Texture.Width / 2, numberKeysButton.Texture.Height / 2), numberKeysButton.Scale, SpriteEffects.None, 0f);
            _spriteBatch.Draw(scrollWheelButton.Texture, scrollWheelButton.Position, null, Color.White, 0f, new Vector2(scrollWheelButton.Texture.Width / 2, scrollWheelButton.Texture.Height / 2), scrollWheelButton.Scale, SpriteEffects.None, 0f);
            _spriteBatch.Draw(backButton.Texture, backButton.Position, null, Color.White, 0f, new Vector2(backButton.Texture.Width / 2, backButton.Texture.Height / 2), backButton.Scale, SpriteEffects.None, 0f);

            
        }
    }
}
