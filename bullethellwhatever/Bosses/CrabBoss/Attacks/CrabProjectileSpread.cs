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
            int slowDownTime = 20;
            int swingTime = 15;
            int bufferTimeAfterSwing = 12;

            ref float initialSpeed = ref ExtraData[0];
            ref float initialRotation = ref ExtraData[1];

            CrabOwner.FacePlayer();

            if (AITimer == 0)
            {
                initialSpeed = Owner.Velocity.Length();
            }

            if (AITimer < slowDownTime - 1) 
            {
                Owner.Velocity -= Utilities.SafeNormalise(Owner.Velocity) * initialSpeed / (float)slowDownTime;
            }

            if (AITimer > slowDownTime && AITimer < slowDownTime + swingTime)
            {                
                int localTime = AITimer - slowDownTime;

                float anglePreviousFrame = angleToSwingThrough * EasingFunctions.Linear((localTime - 1) / (float)swingTime);
                float angleThisFrame = angleToSwingThrough * EasingFunctions.Linear(localTime / (float)swingTime);

                CrabOwner.Legs[0].RotateLeg(angleThisFrame - anglePreviousFrame);

                int timeBetweenProjectiles = 2;

                if (localTime % timeBetweenProjectiles == 0)
                {
                    Vector2 spawnPosition = CrabOwner.Legs[0].LowerClaw.Position;
                    float projectileInitialSpeed = 5f;
                    
                    Projectile p = SpawnProjectile(spawnPosition, projectileInitialSpeed * Utilities.AngleToVector(CrabOwner.Legs[0].UpperArm.RotationFromV()), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                    p.Rotation = Utilities.VectorToAngle(p.Velocity);

                    p.AddTrail(22);

                    int projectileSlowTime = 20;

                    p.SetExtraAI(new Action(() =>
                    {
                        if (p.AITimer <= projectileSlowTime)
                        {
                            p.Velocity *= 0.99f;
                        }
                        else
                        {
                            p.Velocity += 1f * Utilities.SafeNormalise(p.Velocity);
                        }
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
