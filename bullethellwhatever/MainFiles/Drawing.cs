using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

using bullethellwhatever.Buttons;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.UtilitySystems;
using System.Diagnostics.Contracts;
using bullethellwhatever.Projectiles.TelegraphLines;

namespace bullethellwhatever.MainFiles
{
    public static class Drawing
    {
        public static bool AreButtonsDrawn;
        public static ScreenShakeObject screenShakeObject;
        public static Vector2 ScreenShakeMagnitude;
        public static int ScreenShakeDuration;
        public static int ScreenShakeTimer;
        public static bool IsScreenShaking;
        public static int Timer;
        
        public static void DrawGame()
        {
            Main._spriteBatch.Begin();

            HandleScreenShake();

            DialogueSystem.DrawDialogues(Main._spriteBatch);

            if (Main.activeNPCs.Count == 0) // stuff to draw while the player is not in combat 
            {
                Utilities.drawTextInDrawMethod("Press Q to restart the fight. If you wish to change your settings or the difficulty, click the button.", new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2), Main._spriteBatch, Main.font, Color.White);

                //Add in the title screen button. This uses TitleScreenButton as its attributes fit what I need.
                TitleScreenButton backToTitle = new TitleScreenButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 4 * 3), Main.startButton,
                GameState.GameStates.TitleScreen, new Vector2(3, 3));

                //Only add the button to the list of active buttons if it has not yet been added to prevent it being added infinitely.
                if (Main.activeButtons.Count < 1)
                    Main.activeButtons.Add(backToTitle);

                //Draw the button.
                BetterDraw(backToTitle.Texture, backToTitle.Position, null, Color.White, 0f, backToTitle.Scale, SpriteEffects.None, 0f);
                //Main._spriteBatch.Draw(backToTitle.Texture, backToTitle.Position, null, Color.White, 0f, new Vector2(backToTitle.Texture.Width / 2, backToTitle.Texture.Height / 2), backToTitle.Scale, SpriteEffects.None, 0f);
            }

            //Calculate transparency based on the player's remaining immunity frames.

            float transparency = 4f * (1f / (Main.player.IFrames + 1f)); //to indicate iframes

            //Draw the player, accounting for immunity frame transparency.

            BetterDraw(Main.player.Texture, Main.player.Position, null, Color.White * transparency, Main.player.Rotation, Vector2.One, SpriteEffects.None, 0f);

            //Draw every active NPC.           

            //Draw every enemy projectile.

            foreach (Projectile projectile in Main.activeProjectiles)
            {
                projectile.Draw(Main._spriteBatch);
                DrawTelegraphs(projectile);

            }

            foreach (NPC npc in Main.activeNPCs) //move this back later
            {
                Main.deathrayShader.Parameters["bossHPRatio"]?.SetValue(npc.HPRatio);

                DrawTelegraphs(npc);
                BetterDraw(npc.Texture, npc.Position, null, npc.Colour, npc.Rotation, npc.Size, SpriteEffects.None, 0f);
                
            }

            //Draw every player projectile.
            foreach (Projectile projectile in Main.activeFriendlyProjectiles)
            {
                projectile.Draw(Main._spriteBatch);
                DrawTelegraphs(projectile);
            }

            //Draw the boss health bar. Note that active bosses will always be the first entries in the ActiveNPCs list.
            if (Main.activeNPCs.Count > 0)
            {
                DrawHealthBar(Main._spriteBatch, Main.activeNPCs[0], new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 20 * 19), 120f, 3f);
            }

            //Draw the player's health bar.
            DrawHealthBar(Main._spriteBatch, Main.player, new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 20), 30f, 3f);

            //Select a control indicator based on the currently selected control scheme.
            string ControlInstruction = GameState.WeaponSwitchControl ? " , use the scroll wheel to switch weapons." : " , use the number keys 1, 2 and 3 to switch weapons";

            //Write different text depending on which weapon is active
            switch (Main.player.ActiveWeapon)
            {
                case Player.Weapons.Laser:
                    Utilities.drawTextInDrawMethod("Current weapon: " + Main.player.ActiveWeapon.ToString() + ControlInstruction, new Vector2(Main._graphics.PreferredBackBufferWidth / 20, Main._graphics.PreferredBackBufferHeight / 20), Main._spriteBatch, Main.font, Color.Yellow);
                    break;
                case Player.Weapons.MachineGun:
                    Utilities.drawTextInDrawMethod("Current weapon: " + Main.player.ActiveWeapon.ToString() + ControlInstruction, new Vector2(Main._graphics.PreferredBackBufferWidth / 20, Main._graphics.PreferredBackBufferHeight / 20), Main._spriteBatch, Main.font, Color.SkyBlue);
                    break;
                case Player.Weapons.Homing:
                    Utilities.drawTextInDrawMethod("Current weapon: " + Main.player.ActiveWeapon.ToString() + ControlInstruction, new Vector2(Main._graphics.PreferredBackBufferWidth / 20, Main._graphics.PreferredBackBufferHeight / 20), Main._spriteBatch, Main.font, Color.LimeGreen);
                    break;
            }



            Main._spriteBatch.End();

            //Begin using the shader.

            
        }

        public static void BetterDraw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 scale, SpriteEffects spriteEffects, float layerDepth)
        {
            //This method exists so that one does not have to repeat the same paraemters for stuff like origin offsets and screenshake offset.

            //Draw the item at the position, moved by the amount the screen is shaking.

            if (screenShakeObject.Timer > 0)
            { 
                Vector2 positionWithScreenShake = new(position.X + screenShakeObject.Magnitude.X, position.Y + screenShakeObject.Magnitude.Y);

                Main._spriteBatch.Draw(texture, positionWithScreenShake, sourceRectangle, color, rotation, new Vector2(texture.Width / 2, texture.Height / 2), scale, spriteEffects, layerDepth);
            }

            else Main._spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, new Vector2(texture.Width / 2, texture.Height / 2), scale, spriteEffects, layerDepth);
        }

        public static void DrawTelegraphs(Entity entity)
        {
            foreach (TelegraphLine telegraphLine in entity.activeTelegraphs)
            {
                telegraphLine.Draw(Main._spriteBatch);
            }
        }
        public static void ScreenShake(int magnitude, int duration) 
        {
            if (magnitude > screenShakeObject.Magnitude.X) //always apply strongest screen shake
                screenShakeObject = new ScreenShakeObject(magnitude, duration);
        }

        private static void HandleScreenShake() //under the hood screen shaking
        {
            screenShakeObject.TickDownDuration();

            Random rng = new Random();

            screenShakeObject.Magnitude = new(rng.Next((int)screenShakeObject.MaxMagnitude.X), rng.Next((int)screenShakeObject.MaxMagnitude.Y));
        }

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
            TitleScreenButton startButton = new TitleScreenButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 3, Main._graphics.PreferredBackBufferHeight / 2), Main.startButton,
                GameState.GameStates.BossSelect, new Vector2(3, 3));

            TitleScreenButton settingsButton = new TitleScreenButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 3 * 2, Main._graphics.PreferredBackBufferHeight / 2), Main.settingsButton,
                GameState.GameStates.Settings, new Vector2(3, 3));

            if (Main.activeButtons.Count < 2)
                Main.activeButtons.Add(startButton);

            if (Main.activeButtons.Count < 2)
                Main.activeButtons.Add(settingsButton);

            _spriteBatch.Begin();
            _spriteBatch.Draw(startButton.Texture, startButton.Position, null, Color.White, 0f, new Vector2(startButton.Texture.Width / 2, startButton.Texture.Height / 2), startButton.Scale, SpriteEffects.None, 0f);
            _spriteBatch.Draw(settingsButton.Texture, settingsButton.Position, null, Color.White, 0f, new Vector2(settingsButton.Texture.Width / 2, settingsButton.Texture.Height / 2), settingsButton.Scale, SpriteEffects.None, 0f);

            _spriteBatch.End();
        }

        public static void DrawBossSelect(SpriteBatch _spriteBatch)
        {
            BossSelectButton testBossButton = new BossSelectButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 3, Main._graphics.PreferredBackBufferHeight / 2), Main.bossButton,
                GameState.GameStates.DifficultySelect, GameState.Bosses.TestBoss, new Vector2(3, 3), true);

            BossSelectButton secondBossButton = new BossSelectButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 3 * 2, Main._graphics.PreferredBackBufferHeight / 2), Main.bossButton,
                GameState.GameStates.DifficultySelect, GameState.Bosses.SecondBoss, new Vector2(3, 3), true);


            if (!Main.activeButtons.Contains(testBossButton))
                Main.activeButtons.Add(testBossButton);

            if (!Main.activeButtons.Contains(secondBossButton))
                Main.activeButtons.Add(secondBossButton);

            _spriteBatch.Begin();
            _spriteBatch.Draw(testBossButton.Texture, testBossButton.Position, null, Color.White, 0f, new Vector2(testBossButton.Texture.Width / 2, testBossButton.Texture.Height / 2), new Vector2(3, 3), SpriteEffects.None, 0f);
            _spriteBatch.Draw(secondBossButton.Texture, secondBossButton.Position, null, Color.White, 0f, new Vector2(secondBossButton.Texture.Width / 2, secondBossButton.Texture.Height / 2), new Vector2(3, 3), SpriteEffects.None, 0f);
            _spriteBatch.End();
        }

        public static void DrawDifficultySelect(SpriteBatch _spriteBatch)
        { //add this to other menus

            Texture2D[] buttonTexturesToDraw = { Main.easyButton, Main.normalButton, Main.hardButton, Main.insaneButton };

            _spriteBatch.Begin();

            int counter = 1;

            foreach (Texture2D texture in buttonTexturesToDraw)
            {
                DifficultySelectButton button = new DifficultySelectButton(new Vector2(Main._graphics.PreferredBackBufferWidth / (buttonTexturesToDraw.Length + 1) * counter, Main._graphics.PreferredBackBufferHeight / 2), texture,
                    GameState.GameStates.InGame, (GameState.Difficulties)(counter - 1), new Vector2(3, 3));

                if (Main.activeButtons.Count < buttonTexturesToDraw.Length)
                    Main.activeButtons.Add(button);

                _spriteBatch.Draw(button.Texture, button.Position, null, Color.White, 0f, new Vector2(button.Texture.Width / 2, button.Texture.Height / 2), new Vector2(3, 3), SpriteEffects.None, 0f);

                counter = counter + 1;

            }

            _spriteBatch.End();


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

            _spriteBatch.Begin();

            ConfirmControlSettingsChange(_spriteBatch);

            _spriteBatch.Draw(numberKeysButton.Texture, numberKeysButton.Position, null, Color.White, 0f, new Vector2(numberKeysButton.Texture.Width / 2, numberKeysButton.Texture.Height / 2), numberKeysButton.Scale, SpriteEffects.None, 0f);
            _spriteBatch.Draw(scrollWheelButton.Texture, scrollWheelButton.Position, null, Color.White, 0f, new Vector2(scrollWheelButton.Texture.Width / 2, scrollWheelButton.Texture.Height / 2), scrollWheelButton.Scale, SpriteEffects.None, 0f);
            _spriteBatch.Draw(backButton.Texture, backButton.Position, null, Color.White, 0f, new Vector2(backButton.Texture.Width / 2, backButton.Texture.Height / 2), backButton.Scale, SpriteEffects.None, 0f);

            _spriteBatch.End();
        }

        public static void ConfirmControlSettingsChange(SpriteBatch spriteBatch)
        {
            string ControlChangedTo = "";

            if (GameState.HasASettingBeenChanged)
            {
                if (GameState.WeaponSwitchControl)
                {
                    ControlChangedTo = "Weapon switch control switched to scroll wheel.";
                }

                else
                {
                    ControlChangedTo = "Weapon switch control switched to number keys.";
                }
            }
            Utilities.drawTextInDrawMethod(ControlChangedTo, new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 10 * 7), Main._spriteBatch, Main.font, Color.White);


        }

        
    }

    
}
