using Microsoft.Xna.Framework.Audio;
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

            float transparency = 4f * (1f / (Main.player.IFrames + 1f)); //to indicate iframes

            Main._spriteBatch.Draw(Main.player.Texture, Main.player.Position, null, Color.White * transparency, Main.player.Rotation, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0f);


            foreach (NPC npc in Main.activeNPCs)
            {
                Main._spriteBatch.Draw(Main.player.Texture, npc.Position, null, npc.Colour(), npc.Rotation, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(npc.Size, npc.Size), SpriteEffects.None, 0f);
            }

            foreach (Projectile projectile in Main.activeProjectiles)
            {
                Main._spriteBatch.Draw(Main.player.Texture, projectile.Position, null, projectile.Colour(), projectile.Rotation, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0f);
            }

            foreach (Projectile projectile in Main.activeFriendlyProjectiles)
            {
                Main._spriteBatch.Draw(Main.player.Texture, projectile.Position, null, projectile.Colour(), projectile.Rotation, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0f);
            }
            if (Main.activeNPCs.Count > 0)
            {
                DrawHealthBar(Main._spriteBatch, Main.activeNPCs[0], new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 20 * 19), 120f, 3f);
            }

            DrawHealthBar(Main._spriteBatch, Main.player, new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 20), 30f, 3f);

            switch (Main.player.ActiveWeapon)
            {
                case Player.Weapons.Sharpshooter:
                    Utilities.drawTextInDrawMethod("Current weapon: " + Main.player.ActiveWeapon.ToString() + " , use the scroll wheel to switch weapons.", new Vector2(Main._graphics.PreferredBackBufferWidth / 20, Main._graphics.PreferredBackBufferHeight / 20), Main._spriteBatch, Main.font, Color.Yellow);
                    break;
                case Player.Weapons.MachineGun:
                    Utilities.drawTextInDrawMethod("Current weapon: " + Main.player.ActiveWeapon.ToString() + " , use the scroll wheel to switch weapons.", new Vector2(Main._graphics.PreferredBackBufferWidth / 20, Main._graphics.PreferredBackBufferHeight / 20), Main._spriteBatch, Main.font, Color.SkyBlue);
                    break;
                case Player.Weapons.Homing:
                    Utilities.drawTextInDrawMethod("Current weapon: " + Main.player.ActiveWeapon.ToString() + " , use the scroll wheel to switch weapons.", new Vector2(Main._graphics.PreferredBackBufferWidth / 20, Main._graphics.PreferredBackBufferHeight / 20), Main._spriteBatch, Main.font, Color.LimeGreen);
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
            _spriteBatch.Draw(Main.player.Texture, positionOfBar, null, Color.LimeGreen * opacity, 0f, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(BarWidth, BarHeight), SpriteEffects.None, 0f);

            _spriteBatch.Draw(Main.player.Texture, new Vector2(positionOfBar.X - emptySpaceOnLeft * Main.player.Texture.Width, positionOfBar.Y), null, Color.Red * opacity, 0f, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(BarWidth * healthRatio, BarHeight), SpriteEffects.None, 0f);
        }

        public static void DrawTitleScreen(SpriteBatch _spriteBatch)
        {
            TitleScreenButton startButton = new TitleScreenButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2), Main.startButton,
                GameState.GameStates.BossSelect, new Vector2(3,3));


            if (!Main.activeButtons.Contains(startButton))
                Main.activeButtons.Add(startButton);

            _spriteBatch.Begin();
            _spriteBatch.Draw(startButton.Texture, startButton.Position, null, Color.White, 0f, new Vector2(startButton.Texture.Width / 2, startButton.Texture.Height / 2), new Vector2(3,3), SpriteEffects.None, 0f);
            _spriteBatch.End();
        }

        public static void DrawBossSelect(SpriteBatch _spriteBatch)
        {
            BossSelectButton testBossButton = new BossSelectButton(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2), Main.bossButton,
                GameState.GameStates.DifficultySelect, GameState.Bosses.TestBoss, new Vector2(3, 3));
            

            if (!Main.activeButtons.Contains(testBossButton))
                Main.activeButtons.Add(testBossButton);

            _spriteBatch.Begin();
            _spriteBatch.Draw(testBossButton.Texture, testBossButton.Position, null, Color.White, 0f, new Vector2(testBossButton.Texture.Width / 2, testBossButton.Texture.Height / 2), new Vector2(3, 3), SpriteEffects.None, 0f);
            _spriteBatch.End();
        }

        public static void DrawDifficultySelect(SpriteBatch _spriteBatch)
        { //add this to other menus

            Texture2D[] buttonTexturesToDraw = {Main.easyButton, Main.normalButton, Main.hardButton, Main.insaneButton};

            _spriteBatch.Begin();

            int counter = 1;

            foreach (Texture2D texture in buttonTexturesToDraw)
            {
                DifficultySelectButton button = new DifficultySelectButton(new Vector2(Main._graphics.PreferredBackBufferWidth /  (buttonTexturesToDraw.Length + 1) * counter , Main._graphics.PreferredBackBufferHeight / 2), texture,
                    GameState.GameStates.InGame, (GameState.Difficulties)(counter - 1), new Vector2(3, 3));

                Main.activeButtons.Add(button);

                _spriteBatch.Draw(button.Texture, button.Position, null, Color.White, 0f, new Vector2(button.Texture.Width / 2, button.Texture.Height / 2), new Vector2(3, 3), SpriteEffects.None, 0f);

                counter = counter + 1;

            }

            _spriteBatch.End();
            
            
        }
    }
}
