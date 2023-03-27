using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;

namespace bullethellwhatever
{
    public class EntityManager
    {
        public static void RemoveEntites()
        {
            Main.activeNPCs.RemoveAll(NPC => NPC.ShouldRemoveOnEdgeTouch() && Entity.touchingAnEdge(NPC, Main._graphics.PreferredBackBufferWidth, Main._graphics.PreferredBackBufferHeight));
            Main.activeNPCs.RemoveAll(NPC => NPC.DeleteNextFrame && NPC.hasDesperation() == false);
            Main.activeProjectiles.RemoveAll(projectile => projectile.ShouldRemoveOnEdgeTouch() && Entity.touchingAnEdge(projectile, Main._graphics.PreferredBackBufferWidth, Main._graphics.PreferredBackBufferHeight));
            Main.activeFriendlyProjectiles.RemoveAll(projectile => (projectile.ShouldRemoveOnEdgeTouch() && Entity.touchingAnEdge(projectile, Main._graphics.PreferredBackBufferWidth, Main._graphics.PreferredBackBufferHeight) || projectile.DeleteNextFrame));
        }

        public static void RunAIs()
        {
            foreach (NPC npc in Main.activeNPCs)
            {
                npc.AI();
            }

            foreach (Projectile projectile in Main.activeProjectiles)
            {
                projectile.AI();
            }

            foreach (Projectile projectile in Main.activeFriendlyProjectiles)
            {
                projectile.AI();
            }


            Main.player.AI();
        }

        public static void SpawnBoss()
        {
            Main.activeNPCs.Add(new Boss(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 20), new Vector2(0, 0)));
            Main.activeNPCs[0].Spawn(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 20), new Vector2(2f, 0f), 10, Main.playerTexture, 5f, 200f);
        }

    }
}
