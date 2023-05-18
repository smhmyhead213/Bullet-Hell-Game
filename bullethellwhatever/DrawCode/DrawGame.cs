using bullethellwhatever.MainFiles;
using bullethellwhatever.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.UtilitySystems.Dialogue;
using System.Collections.Generic;
using bullethellwhatever.Projectiles;
using bullethellwhatever.Projectiles.TelegraphLines;

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
                Button backToTitle = new Button(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 4 * 3), "StartButton",
                GameState.GameStates.TitleScreen, null, new Vector2(3, 3));

                //Draw the button.
                Drawing.BetterDraw(backToTitle.Texture, backToTitle.Position, null, Color.White, 0f, backToTitle.Scale, SpriteEffects.None, 0f);
                //Main._spriteBatch.Draw(backToTitle.Texture, backToTitle.Position, null, Color.White, 0f, new Vector2(backToTitle.Texture.Width / 2, backToTitle.Texture.Height / 2), backToTitle.Scale, SpriteEffects.None, 0f);
            }

            Utilities.drawTextInDrawMethod((1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString(), new Vector2(Main._graphics.PreferredBackBufferWidth / 4, Main._graphics.PreferredBackBufferHeight / 4), Main._spriteBatch, Main.font, Color.White);

            List<Entity> ProjectilestoDrawWithoutShader = new List<Entity>();
            List<Entity> ProjectilestoDrawWithShader = new List<Entity>();

            List<Entity> NPCstoDrawWithoutShader = new List<Entity>();
            List<Entity> NPCstoDrawWithShader = new List<Entity>();

            //Populate lists with entites to draw with and without shaders.

            // This whole thing can yes be done in one spriteBatch restart, but doing everything in this order fixes layering issues
            // Everything must be looped through in the Immediate sprite mode as all telegraph lines use a shader.

            if (Main.player is IDrawsShader)
            {
                NPCstoDrawWithShader.Add(Main.player);
            }
            else NPCstoDrawWithoutShader.Add(Main.player);

            foreach (Projectile projectile in Main.activeProjectiles)
            {
                if (projectile is IDrawsShader)
                {
                    ProjectilestoDrawWithShader.Add(projectile);
                }
                else ProjectilestoDrawWithoutShader.Add(projectile);
            }

            foreach (NPC npc in Main.activeNPCs)
            {
                if (npc is IDrawsShader)
                {
                    NPCstoDrawWithShader.Add(npc);
                }
                else NPCstoDrawWithoutShader.Add(npc);
            }

            foreach (Projectile projectile in Main.activeFriendlyProjectiles)
            {
                if (projectile is IDrawsShader)
                {
                    ProjectilestoDrawWithShader.Add(projectile);
                }
                else ProjectilestoDrawWithoutShader.Add(projectile);
            }

            // Draw all projectiles without shaders first.

            foreach (Entity entity in ProjectilestoDrawWithoutShader)
            {
                    entity.Draw(Main._spriteBatch);
            }

            Main._spriteBatch.End();
            Main._spriteBatch.Begin(SpriteSortMode.Immediate);

            //Draw all projectiles that do use a shader.

            foreach (Entity entity in ProjectilestoDrawWithShader)
            {
                entity.Draw(Main._spriteBatch);

                foreach (TelegraphLine t in entity.activeTelegraphs)
                {
                    t.Draw(Main._spriteBatch);
                }
            }

            foreach (Entity entity in ProjectilestoDrawWithoutShader)
            {
                foreach (TelegraphLine t in entity.activeTelegraphs)
                {
                    t.Draw(Main._spriteBatch);
                }
            }

            foreach (Entity entity in NPCstoDrawWithShader)
            {
                entity.Draw(Main._spriteBatch);

                foreach (TelegraphLine t in entity.activeTelegraphs) 
                {
                    t.Draw(Main._spriteBatch);
                }
            }

            foreach (Entity entity in NPCstoDrawWithoutShader)
            {
                foreach (TelegraphLine t in entity.activeTelegraphs)
                {
                    t.Draw(Main._spriteBatch);
                }
            }
            
            Main._spriteBatch.End();
            Main._spriteBatch.Begin(SpriteSortMode.Deferred);

            foreach (Entity entity in NPCstoDrawWithoutShader)
            {
                entity.Draw(Main._spriteBatch);
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
