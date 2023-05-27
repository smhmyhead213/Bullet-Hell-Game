using Microsoft.Xna.Framework;
using System;

using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.Projectiles.Base
{
    public class OscillatingSpeedProjectile : BasicProjectile
    {
        public float OscillationFrequency;
        public float ProjectileSpeed;

        public OscillatingSpeedProjectile(float oscillationFrequency, float projectileSpeed)
        {
            OscillationFrequency = oscillationFrequency;
            ProjectileSpeed = projectileSpeed;
        }

        public override void AI() //and drawing
        {
            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            float speed = ProjectileSpeed * (MathF.Sin(AITimer / OscillationFrequency) + 1.5f);

            Velocity = speed * Utilities.SafeNormalise(Velocity, Vector2.Zero);

            Position = Position + Velocity;
        }
    }
}
