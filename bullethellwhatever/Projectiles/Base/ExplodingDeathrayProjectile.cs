using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using System;

namespace bullethellwhatever.Projectiles.Base
{
    public class ExplodingDeathrayProjectile : ExplodingProjectile
    {
        public ExplodingDeathrayProjectile(float numberOfRays, int explosionDelay, float offset, bool shouldSlowDown, bool shouldAccelerate, bool shouldAccountForVelocityInOrientation) : base(numberOfRays, explosionDelay, offset, shouldSlowDown, shouldAccelerate, shouldAccountForVelocityInOrientation)
        {
            NumberOfProjectiles = numberOfRays;
            ExplosionDelay = explosionDelay;
            Size = new Vector2(2, 2);
            ShouldSlowDown = shouldSlowDown;
            ShouldAccelerate = shouldAccelerate;
            ShouldAccountForVelocityInOrientation = shouldAccountForVelocityInOrientation;
            Offset = offset;
        }

        public override void AI()
        {
            TimeAlive++;
            if (TimeAlive == 1)
            {
                for (int i = 0; i < NumberOfProjectiles; i++)
                {
                    TelegraphLine teleLine = new TelegraphLine((MathF.PI * 2 / NumberOfProjectiles * i) + Offset, 0, 0, 10, 2000, ExplosionDelay, Position, Color.White, Texture, this);
                }
            }

            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            if (ShouldSlowDown)
                Velocity = Velocity * 0.98f; //slow down to a stop

            if (TimeAlive == ExplosionDelay)
            {
                Explode();
            }

            if (touchingAnEdge(this))
            {
                Explode();
            }

            Position = Position + (Velocity * (ExplosionDelay - TimeAlive) / ExplosionDelay);
        }

        public override void Explode()
        {
            for (int i = 0; i < NumberOfProjectiles; i++)
            {
                Deathray ray = new Deathray();

                float accel = ShouldAccelerate ? 1.005f : 1f;

                //make it explode based on its velocity, see Supreme Calamitas gigablasts exploding based on orientation

                ray.SpawnDeathray(Position, i * MathHelper.TwoPi / NumberOfProjectiles, 1f, 10, Texture, 10, 2000, 0, 0, true, Color.Red, MainFiles.Main.deathrayShader2, Owner);

                DeleteNextFrame = true;
            }
        }
    }
}
