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
using System;
using bullethellwhatever.DrawCode.Particles;

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

            activeProjectiles.RemoveAll(projectile => projectile.DeleteNextFrame);
            activeProjectiles.RemoveAll(projectile => projectile.ShouldRemoveOnEdgeTouch && projectile.TouchingAnEdge() && projectile.TimeOutsidePlayArea > projectile.MercyTimeBeforeRemoval);
            activeFriendlyProjectiles.RemoveAll(projectile => projectile.ShouldRemoveOnEdgeTouch && projectile.TouchingAnEdge() && projectile.TimeOutsidePlayArea > projectile.MercyTimeBeforeRemoval && projectile.AITimer > 5);
            activeFriendlyProjectiles.RemoveAll(projectile => projectile.DeleteNextFrame);
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

            player.AI();
            player.UpdateHitbox();

            foreach (NPC npc in activeNPCs)
            {
                for (int i = 0; i < npc.Updates; i++)
                {
                    npc.PreUpdate();
                    npc.AI();
                    npc.UpdateHitbox();
                    npc.CheckForHits();
                    npc.PostUpdate();
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
                    projectile.PreUpdate();
                    projectile.AI();
                    projectile.UpdateAndCheckHits();
                    projectile.PostUpdate();
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
                    projectile.PreUpdate();
                    projectile.AI();
                    projectile.UpdateAndCheckHits();
                    projectile.PostUpdate();
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
                p.PreUpdate();
                p.AI();
                p.PostUpdate();
            }

            foreach (Particle p in ParticlesToRemove)
            {
                activeParticles.Remove(p);
            }

            AddEntitiesNextFrame();
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

        public static NPC ClosestNPC(Vector2 point, Predicate<NPC> predicate)
        {
            NPC closestNPC = new NPC(); //the target
            float minDistance = float.MaxValue;

            if (activeNPCs.Count > 0)
            {
                foreach (NPC npc in activeNPCs)
                {
                    if (predicate(npc))
                    {
                        float distance = (point - npc.Position).Length();
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestNPC = npc;
                        }
                    }
                }

                return closestNPC;
            }

            else return null;
        }
        public static Projectile ClosestProjectile(List<Projectile> projList, Vector2 point, Func<Projectile, bool> predicate)
        {
            Projectile closestProj = new Projectile(); //the target
            float minDistance = float.MaxValue;

            if (projList.Count > 0)
            {
                foreach (Projectile proj in projList)
                {
                    if (predicate(proj))
                    {
                        float distance = (point - proj.Position).Length();
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestProj = proj;
                        }
                    }
                }

                return closestProj;
            }

            else return null;
        }
        public static NPC ClosestTargetableNPC(Vector2 point)
        {
            return ClosestNPC(point, (NPC npc) => npc.TargetableByHoming && npc.Participating);
        }
    }
}
