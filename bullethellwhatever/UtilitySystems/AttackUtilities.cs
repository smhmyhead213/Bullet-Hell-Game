using bullethellwhatever.Projectiles.Base;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using bullethellwhatever.Bosses.EyeBoss;
using bullethellwhatever.Projectiles;
using bullethellwhatever.MainFiles;
using bullethellwhatever.DrawCode.Particles;

namespace bullethellwhatever.UtilitySystems
{
    public static class AttackUtilities
    {
        public static void SuckInParticles(Vector2 position, Color colour, float particleSpeed, float spawnDistance, int particlesPerFrame)
        {
            int particeDuration = (int)spawnDistance / (int)particleSpeed;

            for (int i = 0; i< particlesPerFrame; i++)
            {
                Particle p = new Particle();
                float rotationAroundPupil = Utilities.RandomFloat(0, Tau);

                Vector2 spawnPos = position + spawnDistance * Utilities.AngleToVector(rotationAroundPupil);

                p.Spawn("box", spawnPos, particleSpeed * Utilities.SafeNormalise(position - spawnPos), new Vector2(0f, 0f), Vector2.One * 0.5f, rotationAroundPupil, colour, 1f, particeDuration);

                p.SetExtraAI(new Action(() =>
                {
                    float velocityLength = p.Velocity.Length();

                    p.Velocity = Utilities.SafeNormalise(position - p.Position) * velocityLength; // seek towards pupil

                }));
            }
        }
        public static void ParticleExplosion(int numberOfParticles, int particleLifeTime, float particleVelocity, Vector2 particleSize, Vector2 position, Color colour)
        {
            for (int i = 0; i < numberOfParticles; i++)
            {
                float rotation = Utilities.RandomFloat(0, Tau);

                Particle p = new Particle();

                Vector2 velocity = particleVelocity * Utilities.RotateVectorClockwise(-Vector2.UnitY, rotation);
                int lifetime = particleLifeTime;

                p.Spawn("box", position, velocity, -velocity / 2f / lifetime, particleSize, rotation, colour, 1f, 20);
            }
        }

        public static void ClearProjectiles()
        {
            foreach (Projectile p in EntityManager.activeProjectiles)
            {
                if (!p.IsEffect)
                    p.Die();
            }
        }
    }
}
