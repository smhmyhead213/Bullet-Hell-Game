using Microsoft.Xna.Framework;

namespace bullethellwhatever;

public class EntityManager
{
    public static void RemoveEntities()
    {
        Main.activeNPCs.RemoveAll(NPC => NPC.ShouldRemoveOnEdgeTouch() && Entity.touchingAnEdge(NPC,
            Main._graphics.PreferredBackBufferWidth, Main._graphics.PreferredBackBufferHeight));
        Main.activeNPCs.RemoveAll(NPC => NPC.DeleteNextFrame && NPC.hasDesperation() == false);
        Main.activeProjectiles.RemoveAll(projectile =>
            (projectile.ShouldRemoveOnEdgeTouch() && Entity.touchingAnEdge(projectile,
                Main._graphics.PreferredBackBufferWidth, Main._graphics.PreferredBackBufferHeight)) ||
            projectile.DeleteNextFrame);
        Main.activeFriendlyProjectiles.RemoveAll(projectile =>
            (projectile.ShouldRemoveOnEdgeTouch() && Entity.touchingAnEdge(projectile,
                Main._graphics.PreferredBackBufferWidth, Main._graphics.PreferredBackBufferHeight)) ||
            projectile.DeleteNextFrame);
    }

    public static void AddEntitiesNextFrame()
    {
        foreach (var npc in Main.NPCsToAddNextFrame)
            Main.activeNPCs.Add(npc);

        foreach (var projectile in Main.enemyProjectilesToAddNextFrame) Main.activeProjectiles.Add(projectile);

        foreach (var projectile in Main.friendlyProjectilesToAddNextFrame)
            Main.activeFriendlyProjectiles.Add(projectile);
    }

    public static void RunAIs()
    {
        foreach (var npc in Main.activeNPCs) npc.AI();

        foreach (var projectile in Main.activeProjectiles) projectile.AI();

        foreach (Projectile projectile in Main.activeFriendlyProjectiles) projectile.AI();


        Main.player.AI();
    }

    public static void SpawnBoss()
    {
        switch (GameState.Boss)
        {
            case GameState.Bosses.TestBoss:
                Main.activeNPCs.Add(new TestBoss(
                    new Vector2(Main._graphics.PreferredBackBufferWidth / 2,
                        Main._graphics.PreferredBackBufferHeight / 20), new Vector2(0, 0)));
                break;
        }

        Main.activeNPCs[0]
            .Spawn(
                new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 20),
                new Vector2(2f, 0f), 1, Main.playerTexture, new Vector2(5, 5), 200f);
    }
}