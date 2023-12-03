﻿using bullethellwhatever.Projectiles.Base;
using SharpDX.MediaFoundation;
using System;
using bullethellwhatever.BaseClasses;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using bullethellwhatever.Bosses.EyeBoss;
using bullethellwhatever.DrawCode;

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
    }
}
