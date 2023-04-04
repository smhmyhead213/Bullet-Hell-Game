using Microsoft.Xna.Framework;

namespace bullethellwhatever;

public class BasicProjectile : HarmfulProjectile
{
    public override bool ShouldRemoveOnEdgeTouch()
    {
        return true;
    }

    public override Color Colour()
    {
        return Color.Red;
    }
}