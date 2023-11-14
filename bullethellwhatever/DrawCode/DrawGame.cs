using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.UtilitySystems.Dialogue;
using System.Collections.Generic;
using bullethellwhatever.Projectiles;
using bullethellwhatever.Projectiles.TelegraphLines;
using System;
using bullethellwhatever.DrawCode.UI.Buttons;
using bullethellwhatever.DrawCode.UI;

namespace bullethellwhatever.DrawCode
{
    public static class DrawGame
    {
        public static float WeaponIconsRotationToAdd;
        public static float PermanentIconRotation;
        public static void DrawTheGame(GameTime gameTime, SpriteBatch s)
        {
            Drawing.HandleScreenShake();

            DialogueSystem.DrawDialogues(s);

            if (activeNPCs.Count == 0) // stuff to draw while the player is not in combat 
            {
                Drawing.screenShakeObject.Magnitude = Vector2.Zero;

                Utilities.drawTextInDrawMethod("Press Q to restart the fight. If you wish to change your settings or the difficulty, click the button.", new Vector2(_graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2), Main._spriteBatch, Main.font, Color.White);

                //Add in the title screen button.
            }

            // FPS counter.
            Utilities.drawTextInDrawMethod((1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString(), new Vector2(ScreenWidth / 4, ScreenHeight / 4), Main._spriteBatch, Main.font, Color.White);

            List<Entity> ProjectilestoDrawWithoutShader = new List<Entity>();          
            List<Entity> ProjectilestoDrawWithShader = new List<Entity>();
            List<Entity> FriendlyProjectilesToDrawWithShader = new List<Entity>();
            List<Entity> FriendlyProjectilesToDrawWithoutShader = new List<Entity>();
            List<Entity> NPCstoDrawWithoutShader = new List<Entity>();
            List<Entity> NPCstoDrawWithShader = new List<Entity>();

            List<Particle> ParticlesToDrawWithoutShader = new List<Particle>();
            List<Particle> ParticlesToDrawWithShader = new List<Particle>();
            // Populate lists with entites to draw with and without shaders.

            // This whole thing can yes be done in one spriteBatch restart, but doing everything in this order fixes layering issues.

            if (player is IDrawsShader)
            {
                NPCstoDrawWithShader.Add(player);
            }
            else NPCstoDrawWithoutShader.Add(player);

            foreach (Projectile projectile in activeProjectiles)
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
                    FriendlyProjectilesToDrawWithShader.Add(projectile);
                }
                else FriendlyProjectilesToDrawWithoutShader.Add(projectile);
            }

            foreach (Particle p in activeParticles)
            {
                if (p is IDrawsShader)
                {
                    ParticlesToDrawWithShader.Add(p);
                }
                else ParticlesToDrawWithoutShader.Add(p);

            }

            // ------------------------------------------------------------------------------------------------------

            foreach (Entity entity in FriendlyProjectilesToDrawWithoutShader)
            {
                entity.Draw(s);
            }


            RestartSpriteBatchForShaders(s);

            foreach (Entity entity in FriendlyProjectilesToDrawWithShader)
            {
                entity.Draw(s);
            }

            RestartSpriteBatchForNotShaders(s);

            foreach (Entity entity in ProjectilestoDrawWithoutShader)
            {
                    entity.Draw(Main._spriteBatch);
            }

            RestartSpriteBatchForShaders(s);

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

            foreach (Particle p in ParticlesToDrawWithShader)
            {
                p.Draw(Main._spriteBatch);
            }

            foreach (Entity entity in NPCstoDrawWithoutShader)
            {
                foreach (TelegraphLine t in entity.activeTelegraphs)
                {
                    t.Draw(Main._spriteBatch);
                }
            }

            RestartSpriteBatchForNotShaders(s);

            foreach (Entity entity in NPCstoDrawWithoutShader)
            {
                entity.Draw(Main._spriteBatch);
            }

            foreach (Particle p in ParticlesToDrawWithoutShader)
            {
                p.Draw(Main._spriteBatch);
            }

            foreach (NPC npc in activeNPCs)
            {
                npc.DrawHPBar(Main._spriteBatch);
            }
            
            DrawHUD(Main._spriteBatch);

            //Select a control indicator based on the currently selected control scheme.
            //string ControlInstruction = GameState.WeaponSwitchControl ? " , use the scroll wheel to switch weapons." : " , use the number keys 1, 2 and 3 to switch weapons";

            ////Write different text depending on which weapon is active.
            //switch (Main.player.ActiveWeapon)
            //{
            //    case Player.Weapons.Laser:
            //        Utilities.drawTextInDrawMethod("Current weapon: " + Main.player.ActiveWeapon.ToString() + ControlInstruction, new Vector2(Main._graphics.PreferredBackBufferWidth / 20, Main._graphics.PreferredBackBufferHeight / 20), Main._spriteBatch, Main.font, Color.Yellow);
            //        break;
            //    case Player.Weapons.MachineGun:
            //        Utilities.drawTextInDrawMethod("Current weapon: " + Main.player.ActiveWeapon.ToString() + ControlInstruction, new Vector2(Main._graphics.PreferredBackBufferWidth / 20, Main._graphics.PreferredBackBufferHeight / 20), Main._spriteBatch, Main.font, Color.SkyBlue);
            //        break;
            //    case Player.Weapons.Homing:
            //        Utilities.drawTextInDrawMethod("Current weapon: " + Main.player.ActiveWeapon.ToString() + ControlInstruction, new Vector2(Main._graphics.PreferredBackBufferWidth / 20, Main._graphics.PreferredBackBufferHeight / 20), Main._spriteBatch, Main.font, Color.LimeGreen);
            //        break;
            //}

            //Begin using the shader.
        }

        public static void DrawHUD(SpriteBatch s)
        {
            RestartSpriteBatchForShaders(s);

            Vector2 hudPos = new Vector2(ScreenWidth / 10f, ScreenHeight / 10f);

            Texture2D hud = Assets["HUDBody"];

            RotatedRectangle hudBox = new RotatedRectangle(0, hud.Width, hud.Height, hudPos, player);
            hudBox.UpdateVertices();

            float opacity = player.Hitbox.Intersects(hudBox).Collided ? 0.2f : 1f;

            Shaders["PlayerHealthBarShader"].Parameters["hpRatio"]?.SetValue(player.Health / player.MaxHP);

            Shaders["PlayerHealthBarShader"].CurrentTechnique.Passes[0].Apply();

            //UI.DrawHealthBar(_spriteBatch, player, new Vector2(ScreenWidth / 7.6f, ScreenHeight / 8.8f), 12.6f, 0.7f);

            Drawing.BetterDraw(Assets["box"], new Vector2(IdealScreenWidth / 7.6f, IdealScreenHeight / 8.8f), null, Color.White * opacity, 0, new Vector2(12.6f, 0.7f), SpriteEffects.None, 0);

            RestartSpriteBatchForNotShaders(s);

            Drawing.BetterDraw(hud, new Vector2(IdealScreenWidth / 10f, IdealScreenHeight / 10f), null, Color.White * opacity, 0, Vector2.One, SpriteEffects.None, 1);
            
            //---------------------- handle rotating weapon icons --------------------------

            Vector2 iconRotationAxis = new Vector2(IdealScreenWidth / 15.174f, IdealScreenHeight / 10f);

            Vector2 drawDistanceFromCentre = new Vector2(0, -30f);

            //Drawing.BetterDraw(Assets["box"], iconRotationAxis, null, Color.Red, 0, Vector2.One, SpriteEffects.None, 1);

            float numberOfWeapons = 3;

            Vector2 iconSize = Vector2.One * 0.6f;

            WeaponIconsRotationToAdd = (((int)player.ActiveWeapon - (int)player.PreviousWeapon) * Tau / numberOfWeapons / player.WeaponSwitchCooldown) * player.WeaponSwitchCooldownTimer;

            PermanentIconRotation = PermanentIconRotation + WeaponIconsRotationToAdd;

            while (PermanentIconRotation > Tau)
            {
                PermanentIconRotation = PermanentIconRotation - Tau; // keep within one full turn so we dont go crazy
            }

            while (PermanentIconRotation < -Tau)
            {
                PermanentIconRotation = PermanentIconRotation + Tau; // keep within one full turn so we dont go crazy
            }

            Drawing.BetterDraw(Assets["HomingWeaponIcon"], iconRotationAxis + Utilities.RotateVectorClockwise(drawDistanceFromCentre, 0 * Tau / numberOfWeapons + PermanentIconRotation), null, Color.White * opacity, 0, iconSize, SpriteEffects.None, 1);
            Drawing.BetterDraw(Assets["MachineWeaponIcon"], iconRotationAxis + Utilities.RotateVectorClockwise(drawDistanceFromCentre, 1 * Tau / numberOfWeapons + PermanentIconRotation), null, Color.White * opacity, 0, iconSize, SpriteEffects.None, 1);
            Drawing.BetterDraw(Assets["LaserWeaponIcon"], iconRotationAxis + Utilities.RotateVectorClockwise(drawDistanceFromCentre, 2 * Tau / numberOfWeapons + PermanentIconRotation), null, Color.White * opacity, 0, iconSize, SpriteEffects.None, 1);
        }

        public static void ResetHUD()
        {
            PermanentIconRotation = 0;
            WeaponIconsRotationToAdd = 0;
        }

        public static void RestartSpriteBatchForShaders(SpriteBatch s)
        {
            s.End();
            s.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.LinearWrap);
        }

        public static void RestartSpriteBatchForNotShaders(SpriteBatch s)
        {
            s.End();
            s.Begin(sortMode: SpriteSortMode.Deferred, samplerState: null);
        }
    }
}
