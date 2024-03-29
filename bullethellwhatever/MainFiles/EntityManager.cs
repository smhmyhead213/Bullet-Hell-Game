﻿using Microsoft.Xna.Framework;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Player;
using bullethellwhatever.Bosses;
using bullethellwhatever.Bosses.TestBoss;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.TelegraphLines;
using System.Collections.Generic;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Bosses.CrabBoss;
using bullethellwhatever.Bosses.EyeBoss;

namespace bullethellwhatever.MainFiles
{
    public class EntityManager
    {
        public static List<Particle> ParticlesToRemove;
        public static List<Particle> ParticlesToAdd;
        public static void RemoveEntities()
        {
            activeNPCs.RemoveAll(NPC => NPC.ShouldRemoveOnEdgeTouch && Entity.touchingAnEdge(NPC));
            activeNPCs.RemoveAll(NPC => NPC.DeleteNextFrame && NPC.IsDesperationOver == true);

            //foreach (Projectile projectile in activeProjectiles)
            //{
            //    if (projectile.ShouldRemoveOnEdgeTouch && Entity.touchingAnEdge(projectile) && projectile.TimeOutsidePlayArea > 10)
            //    {
            //        projectile.Die(); // do the die behaviour if getting removed
            //    }
            //}

            //foreach (Projectile projectile in activeFriendlyProjectiles)
            //{
            //    if (projectile.DeleteNextFrame || (projectile.ShouldRemoveOnEdgeTouch && Entity.touchingAnEdge(projectile) && projectile.TimeOutsidePlayArea > 60 && projectile.AITimer > 5))
            //    {
            //        projectile.Die(); // do the die behaviour if getting removed
            //    }
            //}

            activeProjectiles.RemoveAll(projectile => projectile.DeleteNextFrame);
            activeProjectiles.RemoveAll(projectile => projectile.ShouldRemoveOnEdgeTouch && Entity.touchingAnEdge(projectile) && projectile.TimeOutsidePlayArea > projectile.MercyTimeBeforeRemoval);
            activeFriendlyProjectiles.RemoveAll(projectile => projectile.ShouldRemoveOnEdgeTouch && Entity.touchingAnEdge(projectile) && projectile.TimeOutsidePlayArea > projectile.MercyTimeBeforeRemoval && projectile.AITimer > 5);
            activeFriendlyProjectiles.RemoveAll(projectile => projectile.DeleteNextFrame);

            //Main.activeDialogues.RemoveAll(DialogueObject => DialogueObject.DeleteNextFrame);
        }

        public static void AddEntitiesNextFrame()
        {
            foreach (NPC npc in NPCsToAddNextFrame)
                activeNPCs.Add(npc);

            foreach (Projectile projectile in enemyProjectilesToAddNextFrame)
            {
                activeProjectiles.Add(projectile);
            }

            foreach (Projectile projectile in friendlyProjectilesToAddNextFrame)
            {
                activeFriendlyProjectiles.Add(projectile);
            }
        }

        public static void RunAIs()
        {
            List<TelegraphLine> toRemove = new List<TelegraphLine>();

            player.AI();
            player.UpdateHitbox();

            if (player.dialogueSystem.dialogueObject is not null)
                player.dialogueSystem.dialogueObject.DoDialogue();

            foreach (NPC npc in activeNPCs)
            {
                for (int i = 0; i < npc.Updates; i++)
                {
                    npc.AI();
                    npc.UpdateHitbox();
                    npc.CheckForHits();
                    npc.Update();
                }

                if (npc.dialogueSystem.dialogueObject is not null)
                    npc.dialogueSystem.dialogueObject.DoDialogue();

                foreach (TelegraphLine telegraphLine in npc.activeTelegraphs)
                {
                    telegraphLine.AI();
                    if (telegraphLine.DeleteNextFrame)
                    {
                        toRemove.Add(telegraphLine);
                    }
                }

                foreach (TelegraphLine t in toRemove)
                {
                    npc.activeTelegraphs.Remove(t);
                }
            }

            toRemove.Clear(); // we can just reuse this list

            foreach (Projectile projectile in activeProjectiles)
            {
                for (int i = 0; i < projectile.Updates; i++)
                {
                    projectile.AI();
                    projectile.UpdateHitbox();
                    projectile.CheckForHits();
                    projectile.Update();
                }

                foreach (TelegraphLine telegraphLine in projectile.activeTelegraphs)
                {
                    telegraphLine.AI();
                    if (telegraphLine.DeleteNextFrame)
                    {
                        toRemove.Add(telegraphLine);
                    }
                }

                foreach (TelegraphLine t in toRemove)
                {
                    projectile.activeTelegraphs.Remove(t);
                }
            }

            toRemove.Clear();

            foreach (Projectile projectile in activeFriendlyProjectiles)
            {
                for (int i = 0; i < projectile.Updates; i++)
                {
                    projectile.AI();
                    projectile.UpdateHitbox();
                    projectile.CheckForHits();
                    projectile.Update();
                }

                foreach (TelegraphLine telegraphLine in projectile.activeTelegraphs)
                {
                    telegraphLine.AI();
                    if (telegraphLine.DeleteNextFrame)
                    {
                        toRemove.Add(telegraphLine);
                    }
                }

                foreach (TelegraphLine t in toRemove)
                {
                    projectile.activeTelegraphs.Remove(t);
                }
            }

            foreach (Particle p in activeParticles)
            {
                p.AI();
                p.Update();
            }
        }

        public static void SpawnBoss()
        {
            Boss toSpawn = new Boss();

            switch (GameState.Boss)
            {
                case GameState.Bosses.TestBoss:
                    toSpawn = new TestBoss(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 20), new Vector2(2, 0), Assets["box"]);
                    break;
                case GameState.Bosses.SecondBoss:
                    toSpawn = new SecondBoss(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2), new Vector2(0, 0));
                    break;
                case GameState.Bosses.CrabBoss:
                    toSpawn = new CrabBoss();
                    break;
                case GameState.Bosses.EyeBoss:
                    toSpawn = new EyeBoss();
                    break;
            }

            toSpawn.InitialiseBoss();

            Drawing.ScreenShakeTimer = 0;

            Drawing.screenShakeObject = new UtilitySystems.ScreenShakeObject(0, 0);

            toSpawn.CreateNPC(toSpawn.Position, toSpawn.Velocity, 1, toSpawn.Texture, toSpawn.Size, toSpawn.Health, 200, toSpawn.Colour, false, true);
            toSpawn.PrepareNPCButDontAddToListYet();
            activeNPCs.Add(toSpawn);
        }

    }
}
