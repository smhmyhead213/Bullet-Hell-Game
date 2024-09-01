using Microsoft.Xna.Framework;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.Bosses;

using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.TelegraphLines;
using System.Collections.Generic;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Bosses.CrabBoss;
using bullethellwhatever.Bosses.EyeBoss;
using bullethellwhatever.Projectiles;
using bullethellwhatever.NPCs;

namespace bullethellwhatever.MainFiles
{
    public class EntityManager
    {

        public static List<NPC> activeNPCs = new List<NPC>();
        public static List<NPC> activeFriendlyNPCs = new List<NPC>();
        public static List<Projectile> activeProjectiles = new List<Projectile>();
        public static List<Projectile> activeFriendlyProjectiles = new List<Projectile>();
        public static List<Projectile> enemyProjectilesToAddNextFrame = new List<Projectile>();
        public static List<Projectile> friendlyProjectilesToAddNextFrame = new List<Projectile>();
        public static List<Particle> ParticlesToRemove = new List<Particle>();
        public static List<Particle> ParticlesToAdd = new List<Particle>();
        public static void RemoveEntities()
        {
            activeNPCs.RemoveAll(NPC => NPC.ShouldRemoveOnEdgeTouch && NPC.TouchingAnEdge());
            activeNPCs.RemoveAll(NPC => NPC.DeleteNextFrame);

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
            activeProjectiles.RemoveAll(projectile => projectile.ShouldRemoveOnEdgeTouch && projectile.TouchingAnEdge() && projectile.TimeOutsidePlayArea > projectile.MercyTimeBeforeRemoval);
            activeFriendlyProjectiles.RemoveAll(projectile => projectile.ShouldRemoveOnEdgeTouch && projectile.TouchingAnEdge() && projectile.TimeOutsidePlayArea > projectile.MercyTimeBeforeRemoval && projectile.AITimer > 5);
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

            foreach (Particle p in ParticlesToAdd)
            {
                activeParticles.Add(p);
            }

            enemyProjectilesToAddNextFrame.Clear();

            friendlyProjectilesToAddNextFrame.Clear();

            ParticlesToAdd.Clear();

            NPCsToAddNextFrame.Clear();
        }

        public static void RunAIs()
        {
            List<TelegraphLine> toRemove = new List<TelegraphLine>();

            AddEntitiesNextFrame();

            player.AI();
            player.UpdateHitbox();

            foreach (NPC npc in activeNPCs)
            {
                for (int i = 0; i < npc.Updates; i++)
                {
                    npc.AI();
                    npc.UpdateHitbox();
                    npc.CheckForHits();
                    npc.Update();
                }

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

            foreach (Particle p in ParticlesToRemove)
            {
                activeParticles.Remove(p);
            }
        }

        /// <summary>
        /// Clears all NPCs and projectiles except for the player.
        /// </summary>
        public static void Clear()
        {
            foreach (Projectile p in activeProjectiles)
            {
                p.Delete();
            }

            foreach (Projectile p in activeFriendlyProjectiles)
            {
                p.Delete();
            }

            foreach (NPC npc in activeNPCs)
            {
                npc.Delete();
            }
        }

        public static void SpawnBoss()
        {
            Boss toSpawn = new Boss();

            switch (GameState.Boss)
            {
                case GameState.Bosses.SecondBoss:
                    toSpawn = new SecondBoss(Utilities.CentreOfScreen(), new Vector2(0, 0));
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

            toSpawn.Spawn(toSpawn.Position, toSpawn.Velocity, 1, toSpawn.Texture.Name, toSpawn.Size, toSpawn.Health, 200, toSpawn.Colour, false, true);
        }

    }
}
