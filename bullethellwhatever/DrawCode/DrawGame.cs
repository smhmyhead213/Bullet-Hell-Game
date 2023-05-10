using bullethellwhatever.MainFiles;
using bullethellwhatever.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.UtilitySystems.Dialogue;



namespace bullethellwhatever.DrawCode
{
    public static class DrawGame
    {
        public static void DrawTheGame(GameTime gameTime)
        {          
            Drawing.HandleScreenShake();

            DialogueSystem.DrawDialogues(Main._spriteBatch);

            if (Main.activeNPCs.Count == 0) // stuff to draw while the player is not in combat 
            {
                Drawing.screenShakeObject.Magnitude = Vector2.Zero;

                Utilities.drawTextInDrawMethod("Press Q to restart the fight. If you wish to change your settings or the difficulty, click the button.", new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2), Main._spriteBatch, Main.font, Color.White);

                //Add in the title screen button. This uses TitleScreenButton as its attributes fit what I need.
                Button backToTitle = new Button(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 4 * 3), Main.startButton,
                GameState.GameStates.TitleScreen, null, new Vector2(3, 3));

                //Draw the button.
                Drawing.BetterDraw(backToTitle.Texture, backToTitle.Position, null, Color.White, 0f, backToTitle.Scale, SpriteEffects.None, 0f);
                //Main._spriteBatch.Draw(backToTitle.Texture, backToTitle.Position, null, Color.White, 0f, new Vector2(backToTitle.Texture.Width / 2, backToTitle.Texture.Height / 2), backToTitle.Scale, SpriteEffects.None, 0f);
            }

            //Calculate transparency based on the player's remaining immunity frames.

            //Utilities.drawTextInDrawMethod(Main.activeProjectiles.Count.ToString(), new Vector2(Main._graphics.PreferredBackBufferWidth / 4, Main._graphics.PreferredBackBufferHeight / 4), Main._spriteBatch, Main.font, Color.White);

            Utilities.drawTextInDrawMethod((1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString(), new Vector2(Main._graphics.PreferredBackBufferWidth / 4, Main._graphics.PreferredBackBufferHeight / 4), Main._spriteBatch, Main.font, Color.White);

            Main.player.Opacity = 4f * (1f / (Main.player.IFrames + 1f)); //to indicate iframes

            //Draw the player, accounting for immunity frame transparency.

            Drawing.BetterDraw(Main.player.Texture, Main.player.Position, null, Color.White * Main.player.Opacity, Main.player.Rotation, Main.player.Size, SpriteEffects.None, 0f);

            //Draw every active NPC.           

            //Draw every enemy projectile.

            foreach (Projectile projectile in Main.activeProjectiles)
            {
                projectile.Draw(Main._spriteBatch);
                Drawing.DrawTelegraphs(projectile);

            }

            foreach (NPC npc in Main.activeNPCs) //move this back later
            {
                Main.deathrayShader.Parameters["bossHPRatio"]?.SetValue(npc.HPRatio);

                Drawing.DrawTelegraphs(npc);
                Drawing.BetterDraw(npc.Texture, npc.Position, null, npc.Colour * npc.Opacity, npc.Rotation, npc.Size, SpriteEffects.None, 0f);

            }

            //Draw every player projectile.
            foreach (Projectile projectile in Main.activeFriendlyProjectiles)
            {
                projectile.Draw(Main._spriteBatch);
                Drawing.DrawTelegraphs(projectile);
            }

            //Draw the boss health bar. Note that active bosses will always be the first entries in the ActiveNPCs list.
            if (Main.activeNPCs.Count > 0)
            {
                UI.DrawHealthBar(Main._spriteBatch, Main.activeNPCs[0], new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 20 * 19), 120f, 3f);
            }

            //Draw the player's health bar.
            UI.DrawHealthBar(Main._spriteBatch, Main.player, new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 20), 30f, 3f);

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

            //Begin using the shader.
        }

    }
}
