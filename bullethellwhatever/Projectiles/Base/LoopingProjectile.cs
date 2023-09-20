using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Projectiles.Base
{
    public class LoopingProjectile : Projectile
    {
        public Vector2 SpawnPosition;
        public Vector2 OriginalSize;

        public float LoopPeriod; //coefficient of x in sin(ax)
        public float LoopAmplitude;
        public float LoopOffset;
        public float RotationOfOrbit; //FROM VERTICAL DONT FORGET
        public LoopingProjectile(float loopPeriod, float loopAmplitude, float loopOffset, float rotationOfOrbit)
        {
            LoopPeriod = loopPeriod;
            LoopAmplitude = loopAmplitude;
            LoopOffset = loopOffset;
            RotationOfOrbit = -rotationOfOrbit; // this - makes it be clockwise from vertical
        }

        public override void Spawn(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            base.Spawn(position, velocity, damage, pierce, texture, acceleration, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);

            SpawnPosition = position;
            OriginalSize = size;
        }
        public override void AI()
        {
            float sineOscillation = MathF.Sin(LoopPeriod * AITimer / 10f - LoopOffset);
            Position = SpawnPosition + new Vector2(MathF.Sin(RotationOfOrbit) * LoopAmplitude * sineOscillation, MathF.Cos(RotationOfOrbit) * LoopAmplitude * sineOscillation);
            float minSize = 0.4f;
            Size = OriginalSize * (0.5f * MathF.Cos(LoopPeriod * AITimer / 10f - LoopOffset) + 0.5f + minSize); //this whole file can be rewritten due to the depth changes
        }
    }
}
