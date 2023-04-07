﻿using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace bullethellwhatever
{
    public static class Drawing
    {
        public static bool AreButtonsDrawn;
        public static void DrawGame()
        {
            Main._spriteBatch.Begin();

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
                Main._spriteBatch.Draw(backToTitle.Texture, backToTitle.Position, null, Color.White, 0f, new Vector2(backToTitle.Texture.Width / 2, backToTitle.Texture.Height / 2), backToTitle.Scale, SpriteEffects.None, 0f);
            }
            else
            {
                Utilities.drawTextInDrawMethod(Main.activeNPCs[0].Velocity.Length().ToString(), new Vector2(Main._graphics.PreferredBackBufferWidth / 5, Main._graphics.PreferredBackBufferHeight / 5), Main._spriteBatch, Main.font, Color.White);
            }

            //Calculate transparency based on the player's remaining immunity frames.
            float transparency = 4f * (1f / (Main.player.IFrames + 1f)); //to indicate iframes

            //Draw the player, accounting for immunity frame transparency.
            Main._spriteBatch.Draw(Main.player.Texture, Main.player.Position, null, Color.White * transparency, Main.player.Rotation, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0f);

            //Draw every active NPC.
            foreach (NPC npc in Main.activeNPCs)
            {
                Main._spriteBatch.Draw(Main.player.Texture, npc.Position, null, npc.Colour(), npc.Rotation, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), npc.Size, SpriteEffects.None, 0f);
            }

            //Draw every enemy projectile.
            foreach (Projectile projectile in Main.activeProjectiles)
            {
                Main._spriteBatch.Draw(Main.player.Texture, projectile.Position, null, projectile.Colour(), projectile.Rotation, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), projectile.Size, SpriteEffects.None, 0f);
            }

            //Draw every player projectile.
            foreach (Projectile projectile in Main.activeFriendlyProjectiles)
            {
                Main._spriteBatch.Draw(Main.player.Texture, projectile.Position, null, projectile.Colour(), projectile.Rotation, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), projectile.Size, SpriteEffects.None, 0f);
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
                case Player.Weapons.Sharpshooter:
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
            BossSelectButton testBossButton = new BossSelectButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2), Main.bossButton,
                GameState.GameStates.DifficultySelect, GameState.Bosses.TestBoss, new Vector2(3, 3), true);


            if (!Main.activeButtons.Contains(testBossButton))
                Main.activeButtons.Add(testBossButton);

            _spriteBatch.Begin();
            _spriteBatch.Draw(testBossButton.Texture, testBossButton.Position, null, Color.White, 0f, new Vector2(testBossButton.Texture.Width / 2, testBossButton.Texture.Height / 2), new Vector2(3, 3), SpriteEffects.None, 0f);
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