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

            int slowDownTime = 3;
            int swingTime = 15;
            int bufferTimeAfterSwing = 12;
            int resetTime = 10;

            ref float initialSpeed = ref ExtraData[0];
            ref float initialRotation = ref ExtraData[1];
            ref float chosenArm = ref Owner.ExtraData[0];
            int chosenArmInt = (int)chosenArm;
            int expandedi = Utilities.ExpandedIndex(ChosenArmIndex());

            CrabOwner.FacePlayer();

            if (AITimer == 0)
            {
                initialSpeed = Owner.Velocity.Length();
            }

            if (AITimer < slowDownTime - 1) 
            {
                if (Owner.Velocity.Length() < 0.01f) // prevent NaN error
                {
                    Owner.Velocity = Vector2.Zero;
                }

                Owner.Velocity -= Utilities.SafeNormalise(Owner.Velocity) * initialSpeed / (float)slowDownTime;
            }

            if (AITimer < slowDownTime)
            {
                float interpolant = AITimer / (float)slowDownTime;
                ChosenArm().UpperArm.LerpTo(-expandedi * -PI / 2f, interpolant);
                ChosenArm().LowerArm.LerpTo(0, interpolant);
            }

            if (AITimer >= slowDownTime && AITimer < slowDownTime + swingTime)
            {                
                int localTime = AITimer - slowDownTime;
                float armLength = ChosenArm().Length();
                float interpolant = localTime / (float)swingTime;

                //float anglePreviousFrame = angleToSwingThrough * EasingFunctions.Linear((localTime - 1) / (float)swingTime);
                //float angleThisFrame = angleToSwingThrough * EasingFunctions.Linear(localTime / (float)swingTime);

                //int expandedi = -Utilities.ExpandedIndex(chosenArmInt);

                //CrabOwner.Arms[chosenArmInt].RotateLeg(expandedi * (angleThisFrame - anglePreviousFrame));

                ChosenArm().LerpRotation(-expandedi * -PI / 2, -expandedi * PI / 2, interpolant);

                int timeBetweenProjectiles = 2;

                if (localTime % timeBetweenProjectiles == 0)
                {
                    Vector2 spawnPosition = CrabOwner.Arms[chosenArmInt].LowerClaw.Position;
                    float projectileInitialSpeed = 3f;
                    
                    Projectile p = SpawnProjectile<Projectile>(spawnPosition, projectileInitialSpeed * Utilities.AngleToVector(CrabOwner.Arms[chosenArmInt].UpperArm.RotationFromV()), 1f, 1, "box", Vector2.One, Owner, true, false, Color.Red, true, false);
                    p.Rotation = Utilities.VectorToAngle(p.Velocity);

                    p.AddTrail(22);

                    p.SetExtraAI(new Action(() =>
                    {
                        p.ExponentialAccelerate(1.05f);
                        //if (p.AITimer < projectileSlowTime)
                        //{
                        //    p.Velocity *= 0.98f;
                        //}
                        //else if (p.AITimer == projectileSlowTime)
                        //{
                        //    p.Velocity += 5f * Utilities.SafeNormalise(p.Velocity);
                        //}
                        //else
                        //{
                        //    p.Velocity *= 1.05f;
                        //}
                    }));
                }
            }

            if (AITimer >= slowDownTime + swingTime + bufferTimeAfterSwing && AITimer < slowDownTime + swingTime + bufferTimeAfterSwing + resetTime)
            {
                int localTime = AITimer - (slowDownTime + swingTime + bufferTimeAfterSwing);
                float interpolant = localTime / (float)resetTime;

                ChosenArm().LerpToRestPosition(interpolant);
            }
            if (AITimer == slowDownTime + swingTime + bufferTimeAfterSwing + resetTime)
            {
                End();
            }

            HandleBounces();
        }
    }
}
