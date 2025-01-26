using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode.Particles
{
    public static class CommonParticles
    {
        public static Particle Spark(Vector2 position, float particleSpeed, int particleLifetime, Color colour)
        {
            float angle = Utilities.RandomAngle();
            Particle p = new Particle();
            p.Spawn("box", position, particleSpeed * Utilities.RandomFloat(0.5f, 1.5f) * Utilities.AngleToVector(angle), Vector2.Zero, new Vector2(0.5f, 2.5f), angle, colour, 1f, particleLifetime);

            p.SetExtraAI(new Action(() =>
            {
                float interpolant = p.AITimer / (float)particleLifetime;
                p.Velocity *= 0.95f;
                p.Opacity = MathHelper.Lerp(1f, 0f, interpolant);
                p.Shader.SetColour(p.Colour * p.Opacity);
            }));

            p.SetShader("EnergyParticleShader");

            return p;
        }
    }
}
