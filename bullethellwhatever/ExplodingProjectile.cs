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

        public ExplodingProjectile(float numberOfProjectiles, float explosionDelay)
        {
            NumberOfProjectiles = numberOfProjectiles;
            ExplosionDelay = explosionDelay;
            Size = new Vector2(2, 2);
        }

        public override void AI()
        {
            TimeAlive++;

            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            Velocity = Velocity * 0.99f; //slow down to a stop

            if (TimeAlive == 120f)
            {
                for (int i = 0; i < NumberOfProjectiles; i++)
                {
                    BasicProjectile projectile = new BasicProjectile();

                    projectile.Spawn(Position, 3f * Utilities.RotateVectorClockwise(Vector2.UnitY, (MathF.PI * 2 / NumberOfProjectiles) * i), 1f, Texture, 1.005f, Vector2.One);

                    DeleteNextFrame = true;
                }
            }

            Position = Position + Velocity;
        }

        public override Color Colour() => Color.Red;
    }
}
