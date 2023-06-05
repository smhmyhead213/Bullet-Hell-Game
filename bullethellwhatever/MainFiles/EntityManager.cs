using Microsoft.Xna.Framework;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Player;
using bullethellwhatever.Bosses;
using bullethellwhatever.Bosses.TestBoss;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.TelegraphLines;
using System.Collections.Generic;
using bullethellwhatever.DrawCode;

namespace bullethellwhatever.MainFiles
{
    public class EntityManager
    {
        public static List<TelegraphLine> telegraphsToRemove = new List<TelegraphLine>();
        public static void RemoveEntities()
        {
            Main.activeNPCs.RemoveAll(NPC => NPC.ShouldRemoveOnEdgeTouch && Entity.touchingAnEdge(NPC));
            Main.activeNPCs.RemoveAll(NPC => NPC.DeleteNextFrame && NPC.IsDesperationOver == true);
            Main.activeProjectiles.RemoveAll(projectile => projectile.DeleteNextFrame);
            Main.activeProjectiles.RemoveAll(projectile => (projectile.ShouldRemoveOnEdgeTouch && Entity.touchingAnEdge(projectile)) && projectile.AITimer > 60);
            Main.activeFriendlyProjectiles.RemoveAll(projectile => projectile.ShouldRemoveOnEdgeTouch && Entity.touchingAnEdge(projectile) && projectile.AITimer > 5 || projectile.DeleteNextFrame);

            //Main.activeDialogues.RemoveAll(DialogueObject => DialogueObject.DeleteNextFrame);
        }

        public static void AddEntitiesNextFrame()
        {
            foreach (NPC npc in Main.NPCsToAddNextFrame)
                Main.activeNPCs.Add(npc);

            foreach (Projectile projectile in Main.enemyProjectilesToAddNextFrame)
            {
                Main.activeProjectiles.Add(projectile);
            }

            foreach (Projectile projectile in Main.friendlyProjectilesToAddNextFrame)
            {
                Main.activeFriendlyProjectiles.Add(projectile);
            }
        }

        public static void RunAIs()
        {
            foreach (NPC npc in Main.activeNPCs)
            {
                npc.AI();
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

            foreach (Projectile projectile in Main.activeProjectiles)
            {
                projectile.AI();
                projectile.CheckForHits();
                projectile.Update();

                foreach (TelegraphLine telegraphLine in projectile.activeTelegraphs)
                {
                    telegraphLine.AI();
                }
            }

            foreach (Projectile projectile in Main.activeFriendlyProjectiles)
            {
                projectile.AI();
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
            
            Main.player.AI();
        }

        public static void SpawnBoss()
        {
            switch (GameState.Boss)
            {
                case GameState.Bosses.TestBoss:
                    Main.activeNPCs.Add(new TestBoss(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 20), new Vector2(2, 0)));
                    break;
                case GameState.Bosses.SecondBoss:
                    Main.activeNPCs.Add(new SecondBoss(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2), new Vector2(0, 0)));
                    break;
            }

            Drawing.ScreenShakeTimer = 0;

            Drawing.screenShakeObject = new UtilitySystems.ScreenShakeObject(0, 0);

            Main.activeNPCs[0].Spawn(Main.activeNPCs[0].Position, Main.activeNPCs[0].Velocity, 1, "box", new Vector2(5, 5), Main.activeNPCs[0].Health, 200, Color.White, false, true);
        }

    }
}
