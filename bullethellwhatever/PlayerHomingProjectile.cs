using Microsoft.Xna.Framework;

namespace bullethellwhatever;

public class PlayerHomingProjectile : FriendlyProjectile
{
    public float HomingFactor; //how strong the homing is

    public override void AI()
    {
        var closestNPC = new NPC(); //the target
        var minDistance = float.MaxValue;
        TimeAlive++;

        if (TimeAlive > 30f)
        {
            foreach (var npc in Main.activeNPCs)
            {
                var distance = Utilities.DistanceBetweenEntities(this, npc);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNPC = npc;
                }
            }

            //Vector2 vectorToTarget = closestNPC.Position - Main.player.Position; //get a vector to the target
            Velocity = Vector2.Normalize(closestNPC.Position - Position) * (TimeAlive - 30f);
        }

        Position = Position + Velocity;
    }

    public override Color Colour()
    {
        return Color.LimeGreen;
    }
}