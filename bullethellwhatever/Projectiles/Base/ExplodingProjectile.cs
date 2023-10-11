using Microsoft.Xna.Framework;
using System;

using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.Projectiles.Base
{
    public class ExplodingProjectile : Projectile
    {
        public float NumberOfProjectiles;
        public int ExplosionDelay;
        public bool ShouldSlowDown;
        public bool ShouldAccelerate;
        public bool ShouldAccountForVelocityInOrientation;
        public float Offset;

        public ExplodingProjectile(float numberOfProjectiles, int explosionDelay, float offset, bool shouldSlowDown, bool shouldAccelerate, bool shouldAccountForVelocityInOrientation)
        {
            NumberOfProjectiles = numberOfProjectiles;
            ExplosionDelay = explosionDelay;
            Size = new Vector2(2, 2);
            ShouldSlowDown = shouldSlowDown;
            ShouldAccelerate = shouldAccelerate;
            ShouldAccountForVelocityInOrientation = shouldAccountForVelocityInOrientation;
            Offset = offset;
        }

        public override void AI()
        {

            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            if (ShouldSlowDown)
                Velocity = Velocity * 0.99f; //slow down to a stop

            if (AITimer == ExplosionDelay)
            {
                Die();
            }

            if (touchingAnEdge(this))
            {
                Die();
            }

            Position = Position + Velocity;
        }

        public override void Die()
        {
            Explode();
            DeleteNextFrame = true; // dont bother fading out    
        }
        public virtual void Explode()
        {
            for (int i = 0; i < NumberOfProjectiles; i++)
            {
                ExplodingProjectileFragment projectile = new ExplodingProjectileFragment();

                float accel = ShouldAccelerate ? 1.005f : 1f;

                //make it explode based on its velocity, see Supreme Calamitas gigablasts exploding based on orientation
                if (ShouldAccountForVelocityInOrientation)
                    projectile.Spawn(Position, 3f * Utilities.RotateVectorClockwise(Utilities.SafeNormalise(Velocity, Vector2.Zero), (MathF.PI * 2 / NumberOfProjectiles * i) + Offset),
                        1f, 1, "box", accel, Vector2.One, this, true, Color.Red, false, false);

                else projectile.Spawn(Position, 3f * Utilities.RotateVectorClockwise(Utilities.SafeNormalise(Vector2.UnitY, Vector2.Zero), (MathF.PI * 2 / NumberOfProjectiles * i) + Offset),
                        1f, 1, "box", accel, Vector2.One, this, true, Color.Red, false, false);
            }
        }
    }
}
