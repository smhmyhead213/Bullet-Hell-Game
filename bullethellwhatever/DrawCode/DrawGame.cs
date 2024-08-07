﻿using bullethellwhatever.MainFiles;
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
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.NPCs;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.DrawCode.UI.Player;

namespace bullethellwhatever.DrawCode
{
    public static class DrawGame
    {
        public static PlayerHUD PlayerHUD;
        public static void DrawTheGame(GameTime gameTime, SpriteBatch s)
        {
            Drawing.HandleScreenShake();

            DialogueSystem.DrawDialogues(s);

            // FPS counter.
            Utilities.drawTextInDrawMethod((1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString(), new Vector2(IdealScreenWidth / 4, IdealScreenHeight / 4), Main._spriteBatch, Main.font, Color.White);

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

            if (player.Shader is not null)
            {
                NPCstoDrawWithShader.Add(player);
            }
            else NPCstoDrawWithoutShader.Add(player);

            foreach (Projectile projectile in activeProjectiles)
            {
                if (projectile.Shader is not null)
                {
                    ProjectilestoDrawWithShader.Add(projectile);
                }
                else ProjectilestoDrawWithoutShader.Add(projectile);
            }

            foreach (NPC npc in Main.activeNPCs)
            {
                if (npc.Shader is not null)
                {
                    NPCstoDrawWithShader.Add(npc);
                }
                else NPCstoDrawWithoutShader.Add(npc);
            }

            foreach (Projectile projectile in Main.activeFriendlyProjectiles)
            {
                if (projectile.Shader is not null)
                {
                    FriendlyProjectilesToDrawWithShader.Add(projectile);
                }
                else FriendlyProjectilesToDrawWithoutShader.Add(projectile);
            }

            foreach (Particle p in activeParticles)
            {
                if (p.Shader is not null)
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


            Drawing.RestartSpriteBatchForShaders(s);

            foreach (Entity entity in FriendlyProjectilesToDrawWithShader)
            {
                entity.Draw(s);
            }

            Drawing.RestartSpriteBatchForNotShaders(s);

            foreach (Entity entity in ProjectilestoDrawWithoutShader)
            {
                    entity.Draw(Main._spriteBatch);
            }

            Drawing.RestartSpriteBatchForShaders(s);

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

            Drawing.RestartSpriteBatchForNotShaders(s);

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
            
            PlayerHUD.Draw(_spriteBatch);

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
    }
}
