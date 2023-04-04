using Microsoft.Xna.Framework;

namespace bullethellwhatever;

public class PlayerProjectile : FriendlyProjectile
{
    public override void AI()
    {
        Position = Position + Velocity;
    }

    public override bool ShouldRemoveOnEdgeTouch()
    {
        return true;
    }

    public override Color Colour()
    {
        return Color.LightBlue;
    }
}