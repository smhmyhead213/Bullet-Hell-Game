using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;


namespace bullethellwhatever.BaseClasses
{
    public enum HitboxTypes
    {
        StaticRectangle,
        RotatableRectangle,
    }
    public class Hitbox
    { 
        public HitboxTypes HitboxType;
        public RotatedRectangle RotatableHitbox;
        public StaticRectangle StaticHitbox;
        public Entity Owner;

        public Hitbox(Entity owner)
        {
            Owner = owner;
            if (Owner.GetHitboxType() == HitboxTypes.RotatableRectangle)
                RotatableHitbox = new RotatedRectangle(0, 0, 0, Vector2.Zero, Vector2.Zero);
            else StaticHitbox = new StaticRectangle();
        }

        public bool CollidingWith(Hitbox hitbox)
        {
            if (Owner.GetHitboxType() == HitboxTypes.StaticRectangle && hitbox.Owner.GetHitboxType() == HitboxTypes.StaticRectangle)
            {
                return StaticHitbox.Rectangle.Intersects(hitbox.StaticHitbox.Rectangle);
            }

            else if (Owner.GetHitboxType() == HitboxTypes.StaticRectangle && hitbox.Owner.GetHitboxType() == HitboxTypes.RotatableRectangle)
            {
                return IsStaticRectangleCollidingWithRotatableRectangle(hitbox.RotatableHitbox, StaticHitbox);
            }

            else if (Owner.GetHitboxType() == HitboxTypes.RotatableRectangle && hitbox.Owner.GetHitboxType() == HitboxTypes.StaticRectangle)
            {
                return IsStaticRectangleCollidingWithRotatableRectangle(RotatableHitbox, hitbox.StaticHitbox);
            }

            else return RotatableHitbox.Intersects(hitbox.RotatableHitbox);
        }
        public bool IsStaticRectangleCollidingWithRotatableRectangle(RotatedRectangle rotatableRectangle, StaticRectangle staticRectangle)
        {
            Vector2 playerVector = new Vector2(staticRectangle.Position.X, staticRectangle.Position.Y);

            if (Utilities.DistanceBetweenVectors(playerVector, rotatableRectangle.Centre) < rotatableRectangle.Length)
            {            
                // Find the angle between the player and the vertical using the dot/scalar product.

                float dotProduct = Vector2.Dot(Vector2.UnitY, playerVector - rotatableRectangle.AxisOfRotation);

                // Find the angle that the deathray and entity must share approximtely for a collision.

                float angle = PI - Acos(dotProduct / (playerVector - rotatableRectangle.AxisOfRotation).Length());

                // Find the diagonal length of the entity to collide with.

                float diagonalOfTarget = Sqrt(Pow(staticRectangle.Rectangle.Width, 2) + Pow(staticRectangle.Rectangle.Height, 2));

                // Find the appropriate angle tolerance needed to perfectly encapsulate the entity.

                float angleTolerance = Atan(diagonalOfTarget / 2 / Utilities.DistanceBetweenVectors(rotatableRectangle.Centre, staticRectangle.Position));

                // For the collision checks, small degrees of error are used to account for Angle and Rotation never being exactly equal due to floating point jank.

                float staticRsAngleFromVertical = Utilities.VectorToAngle(playerVector - rotatableRectangle.AxisOfRotation);

                return Utilities.IsQuantityWithinARangeOfAValue(staticRsAngleFromVertical, rotatableRectangle.Rotation, angleTolerance) || Utilities.IsQuantityWithinARangeOfAValue(staticRsAngleFromVertical - 2f * PI, rotatableRectangle.Rotation, angleTolerance);
            }

            else return false;
        }
    }
}
