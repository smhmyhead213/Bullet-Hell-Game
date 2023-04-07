using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace bullethellwhatever
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
            TimeAlive++;

            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            float speed = ProjectileSpeed * (MathF.Sin(TimeAlive / OscillationFrequency) + 1.5f);

            Velocity = speed * Utilities.SafeNormalise(Velocity, Vector2.Zero);

            Position = Position + Velocity;
        }
    }
}
