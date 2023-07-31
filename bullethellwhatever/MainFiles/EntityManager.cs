using Microsoft.Xna.Framework;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Player;
using bullethellwhatever.Bosses;
using bullethellwhatever.Bosses.TestBoss;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.TelegraphLines;
using System.Collections.Generic;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Bosses.CrabBoss;

namespace bullethellwhatever.MainFiles
{
    public class EntityManager
    {
        public static List<TelegraphLine> telegraphsToRemove = new List<TelegraphLine>(); // could maybe remove this? cant remember why its even here
        public static void RemoveEntities()
        {
            activeNPCs.RemoveAll(NPC => NPC.ShouldRemoveOnEdgeTouch && Entity.touchingAnEdge(NPC));
            activeNPCs.RemoveAll(NPC => NPC.DeleteNextFrame && NPC.IsDesperationOver == true);
            activeProjectiles.RemoveAll(projectile => projectile.DeleteNextFrame);
            activeProjectiles.RemoveAll(projectile => projectile.ShouldRemoveOnEdgeTouch && Entity.touchingAnEdge(projectile) && projectile.TimeOutsidePlayArea > 60);
            activeFriendlyProjectiles.RemoveAll(projectile => projectile.ShouldRemoveOnEdgeTouch && Entity.touchingAnEdge(projectile) && projectile.AITimer > 5 || projectile.DeleteNextFrame);

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
            foreach (NPC npc in activeNPCs)
            {
                npc.AI();
                npc.UpdateHitbox();
                npc.CheckForHits();
                npc.Update();

                if (npc.dialogueSystem.dialogueObject is not null)
                    npc.dialogueSystem.dialogueObject.DoDialogue();

                foreach (TelegraphLine telegraphLine in npc.activeTelegraphs)
                {
                    telegraphLine.AI();
                    if (telegraphLine.DeleteNextFrame)
                    {
                        telegraphsToRemove.Add(telegraphLine);
                    }
                }
            }

            foreach (Projectile projectile in activeProjectiles)
            {
                projectile.AI();
                projectile.UpdateHitbox();
                projectile.CheckForHits();
                projectile.Update();

                foreach (TelegraphLine telegraphLine in projectile.activeTelegraphs)
                {
                    telegraphLine.AI();
                }
            }

            foreach (Projectile projectile in activeFriendlyProjectiles)
            {
                projectile.AI();
                projectile.UpdateHitbox();
                projectile.CheckForHits();
                projectile.Update();

                foreach (TelegraphLine telegraphLine in projectile.activeTelegraphs)
                {
                    telegraphLine.AI();
                }
            }

            for (int i = 0; i < telegraphsToRemove.Count; i++)
            {
                telegraphsToRemove[i].Owner.activeTelegraphs.Remove(telegraphsToRemove[i]);
            }

            player.UpdateHitbox();
            player.AI();
        }

        public static void SpawnBoss()
        {
            switch (GameState.Boss)
            {
                case GameState.Bosses.TestBoss:
                    activeNPCs.Add(new TestBoss(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 20), new Vector2(2, 0)));
                    break;
                case GameState.Bosses.SecondBoss:
                    activeNPCs.Add(new SecondBoss(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2), new Vector2(0, 0)));
                    break;
                case GameState.Bosses.CrabBoss:
                    activeNPCs.Add(new CrabBoss());
                    break;
            }

            Drawing.ScreenShakeTimer = 0;

            Drawing.screenShakeObject = new UtilitySystems.ScreenShakeObject(0, 0);

            Main.activeNPCs[0].Spawn(Main.activeNPCs[0].Position, Main.activeNPCs[0].Velocity, 1, Main.activeNPCs[0].Texture, Main.activeNPCs[0].Size, Main.activeNPCs[0].Health, 200, activeNPCs[0].Colour, false, true);
        }

    }
}
