using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles;

using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabProjectileSpread : CrabBossAttack
    {
        public CrabProjectileSpread(CrabBoss owner) : base(owner)
        {

        }
        public override void Execute(int AITimer)
        {
            float angleToSwingThrough = PI;
            int slowDownTime = 30;
            int swingTime = 30;
            int bufferTimeAfterSwing = 45;

            ref float initialSpeed = ref ExtraData[0];
            ref float initialRotation = ref ExtraData[1];

            CrabOwner.FacePlayer();

            if (AITimer == 0)
            {
                initialSpeed = Owner.Velocity.Length();
            }

            if (AITimer < slowDownTime)
            {
                Owner.Velocity -= Utilities.SafeNormalise(Owner.Velocity) * initialSpeed / (float)slowDownTime;
            }

            int timeBeforeStoppingToSwing = 10;

            if (AITimer > slowDownTime - timeBeforeStoppingToSwing && AITimer < slowDownTime + swingTime - timeBeforeStoppingToSwing)
            {                
                int localTime = AITimer - slowDownTime - timeBeforeStoppingToSwing;

                float anglePreviousFrame = angleToSwingThrough * EasingFunctions.EaseOutExpo((localTime - 1) / (float)swingTime);
                float angleThisFrame = angleToSwingThrough * EasingFunctions.EaseOutExpo(localTime / (float)swingTime);

                CrabOwner.Legs[0].RotateLeg(angleThisFrame - anglePreviousFrame);

                int timeBetweenProjectiles = 2;

                if (localTime % timeBetweenProjectiles == 0)
                {
                    Vector2 spawnPosition = CrabOwner.Legs[0].LowerClaw.Position;
                    float projectileInitialSpeed = 0.1f;

                    Projectile p = SpawnProjectile(spawnPosition, projectileInitialSpeed * Utilities.AngleToVector(CrabOwner.Legs[0].UpperArm.RotationFromV()), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                    p.Rotation = Utilities.VectorToAngle(p.Velocity);

                    p.AddTrail(22);

                    p.SetExtraAI(new Action(() =>
                    {
                        p.Velocity += 0.5f * Utilities.SafeNormalise(p.Velocity);
                    }));
                }
            }

            if (AITimer == slowDownTime + swingTime + bufferTimeAfterSwing)
            {
                End();
                CrabOwner.ResetArmRotations();
            }

            HandleBounces();
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabPunch(CrabOwner);
        }
    }
}
