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
            int pullBackArmTime = 40;
            int swingTime = 10;
            int attackDuration = 60;

            int chargeTrackingTime = 25;

            float angleToPullBackArm = PI / 3f;
            float angleToSwingThrough = angleToPullBackArm + PI / 2f; // chosen so that the arm is parallel to body in good position for a follow up projectile spread
            float topChargeSpeed = 20f;

            Vector2 toPlayer = player.Position - Owner.Position;

            CrabOwner.Rotation = Utilities.VectorToAngle(-toPlayer);

            if (AITimer < chargeTrackingTime)
            {
                Owner.Velocity = topChargeSpeed * EasingFunctions.EaseInOutQuart(AITimer / (float)chargeTrackingTime) * Utilities.SafeNormalise(toPlayer);
            }

            if (AITimer < pullBackArmTime)
            {
                // figure out what angle to swing through this frame.
                float anglePreviousFrame = angleToPullBackArm * EasingFunctions.EaseOutQuad((AITimer - 1) / (float)pullBackArmTime);
                float angleThisFrame = angleToPullBackArm * EasingFunctions.EaseOutQuad(AITimer / (float)pullBackArmTime);
                CrabOwner.Legs[0].RotateLeg(angleThisFrame - anglePreviousFrame);
            }

            if (AITimer > pullBackArmTime && AITimer < pullBackArmTime + swingTime)
            {
                int localTime = AITimer - pullBackArmTime;
                float anglePreviousFrame = angleToSwingThrough * EasingFunctions.EaseOutExpo((localTime - 1) / (float)swingTime);
                float angleThisFrame = angleToSwingThrough * EasingFunctions.EaseOutExpo(localTime / (float)swingTime);

                CrabOwner.Legs[0].RotateLeg(-(angleThisFrame - anglePreviousFrame));
            }

            if (AITimer == attackDuration)
            {
                End();         
            }

            HandleBounces();
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabProjectileSpread(CrabOwner);
        }
    }
}
