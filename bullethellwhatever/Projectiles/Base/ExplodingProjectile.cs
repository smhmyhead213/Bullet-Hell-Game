using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever
{
    public class ExplodingProjectile : Projectile
    {
        public float NumberOfProjectiles;
        public float ExplosionDelay;
        public bool ShouldSlowDown;
        public bool ShouldAccelerate;

        public ExplodingProjectile(float numberOfProjectiles, float explosionDelay, bool shouldSlowDown, bool shouldAccelerate)
        {
            NumberOfProjectiles = numberOfProjectiles;
            ExplosionDelay = explosionDelay;
            Size = new Vector2(2, 2);
            ShouldSlowDown = shouldSlowDown;
        }

        public override void AI()
        {
            TimeAlive++;

            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            if (ShouldSlowDown)
                Velocity = Velocity * 0.99f; //slow down to a stop

            if (TimeAlive == 120f)
            {
                for (int i = 0; i < NumberOfProjectiles; i++)
                {
                    BasicProjectile projectile = new BasicProjectile();

                    float accel = ShouldAccelerate ? 1.005f : 1f;

                    projectile.Spawn(Position, 3f * Utilities.RotateVectorClockwise(Vector2.UnitY, MathF.PI * 2 / NumberOfProjectiles * i), 1f, Texture, accel, Vector2.One);

                    DeleteNextFrame = true;
                }
            }

            Position = Position + Velocity;
        }

        public override Color Colour() => Color.Red;
    }
}
