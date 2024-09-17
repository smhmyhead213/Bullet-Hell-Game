using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles;

using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabPunch : CrabBossAttack
    {
        public CrabPunch(CrabBoss owner) : base(owner)
        {

        }
        public override void Execute(int AITimer)
        {
            int pullBackArmTime = 20;
            int swingDuration = 10;
            int attackDuration = 75;
            ref float swingTime = ref ExtraData[1]; // the farthest point in the attack the swing can happen. will be set to a smaller number if the boss is close to the player.
            int chargeTrackingTime = 5;
            int accelerateTime = 25;

            float angleToPullBackArm = PI / 3f;
            float additionalAngleToSwingThrough = PI / 2f;
            float angleToSwingThrough = angleToPullBackArm + additionalAngleToSwingThrough; // chosen so that the arm is parallel to body in good position for a follow up projectile spread
            float topChargeSpeed = 30f;

            Vector2 toPlayer = player.Position - Owner.Position;
            float angleToPlayer = Utilities.VectorToAngle(toPlayer);

            CrabOwner.Rotation = Utilities.VectorToAngle(-toPlayer);

            ref float initialSpeed = ref ExtraData[2];

            if (AITimer == 0)
            {
                initialSpeed = Owner.Velocity.Length();
                swingTime = 60;
            }

            if (AITimer < accelerateTime)
            {
                float trackingCompletionRatio = AITimer / (float)accelerateTime;
                float differenceBetweenTopAndInitialChargeSpeed = topChargeSpeed - initialSpeed;

                float chargeSpeed = MathHelper.Lerp(initialSpeed, topChargeSpeed, EasingFunctions.EaseInOutQuart(trackingCompletionRatio));

                if (AITimer < chargeTrackingTime)
                {
                    Owner.Velocity = chargeSpeed * Utilities.SafeNormalise(toPlayer);
                }
                else
                {
                    Owner.Velocity = chargeSpeed * Utilities.SafeNormalise(Owner.Velocity);
                }
            }

            if (AITimer < pullBackArmTime)
            {
                // figure out what angle to swing through this frame.
                float anglePreviousFrame = angleToPullBackArm * EasingFunctions.EaseOutQuad((AITimer - 1) / (float)pullBackArmTime);
                float angleThisFrame = angleToPullBackArm * EasingFunctions.EaseOutQuad(AITimer / (float)pullBackArmTime);
                CrabOwner.Legs[0].RotateLeg(angleThisFrame - anglePreviousFrame);
            }

            ref float HasSetSwingTime = ref ExtraData[0]; // if this is 0, swing has not been set. if 1, swing has been set
            float swingProximity = 300f;

            if (AITimer > pullBackArmTime && HasSetSwingTime == 0) // if the arm is fully pulled back and a swing time hasnt been chosen, the boss can choose when to punch
            {
                if (Utilities.DistanceBetweenEntities(player, Owner) < swingProximity)
                {
                    HasSetSwingTime = 1f;
                    swingTime = AITimer + 1; // swing on the next frame
                }
            }

            if (AITimer > swingTime && AITimer < swingTime + swingDuration)
            {
                int localTime = AITimer - (int)swingTime;
                float anglePreviousFrame = angleToSwingThrough * EasingFunctions.EaseOutExpo((localTime - 1) / (float)swingDuration);
                float angleThisFrame = angleToSwingThrough * EasingFunctions.EaseOutExpo(localTime / (float)swingDuration);

                CrabOwner.Legs[0].RotateLeg(-(angleThisFrame - anglePreviousFrame));
            }

            if (AITimer == attackDuration)
            {
                NextAttack = Utilities.RandomInt(1, 2);
                // reuse the extra data index that was used for the speed at the start of the attack

                initialSpeed = Owner.Velocity.Length();

                // if the next attack is chosen to be projectile spread = 1

                if (NextAttack == 1)
                {
                    End();
                }
                // otherwise continue the attack and move arm back to neutral position
            }

            int armRotateBackToNeutralTime = 10;

            if (AITimer > attackDuration && AITimer <= attackDuration + armRotateBackToNeutralTime)
            {
                Leg(0).RotateLeg(additionalAngleToSwingThrough / (float)armRotateBackToNeutralTime);

                float localTime = AITimer - attackDuration;

                float speedInterpolant = EasingFunctions.EaseInQuart(localTime / armRotateBackToNeutralTime);

                Owner.Velocity = Utilities.SafeNormalise(Owner.Velocity) * MathHelper.Lerp(initialSpeed, 0, speedInterpolant);
            }

            if (AITimer == attackDuration + armRotateBackToNeutralTime)
            {
                End();
            }

            HandleBounces();
        }

        public override BossAttack PickNextAttack()
        {
            if (NextAttack == 1)
            {
                return new CrabProjectileSpread(CrabOwner);
            }
            else
            {
                return new CrabPunch(CrabOwner);
            }
        }
    }
}
