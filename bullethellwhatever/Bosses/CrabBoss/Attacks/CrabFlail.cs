using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabFlail : CrabBossAttack
    { 
        public CrabFlail(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int spinUpTime = 60;
            float spinAngularAccel = PI / 540;
            int accelerateTime = 10;
            int slowDownTime = 30;
            int chargeTime = 20;

            ref float angularVelocity = ref Owner.ExtraData[1]; // index 0 is reserved 

            MainCamera.Position = Owner.Position;

            if (AITimer < spinUpTime)
            {
                angularVelocity += spinAngularAccel;

                for (int i = 0; i < 2; i++)
                {
                    int expandedi = Utilities.ExpandedIndex(i);
                    float holdOutArmsAngle = PI / 2;
                    RotateArm(i, -expandedi * holdOutArmsAngle, AITimer, spinUpTime, EasingFunctions.EaseInQuint);
                }

                // move away from player
                float interpolant = 1f - EasingFunctions.EaseOutExpo(AITimer / (float)spinUpTime);
                float moveBackSpeed = 30f;
                Owner.Velocity = interpolant * moveBackSpeed * Utilities.SafeNormalise(Owner.Position - player.Position);
            }

            float maxChargeSpeed = 30f;

            Owner.Rotation += angularVelocity;

            if (AITimer == spinUpTime)
            {
                Owner.Velocity = Utilities.SafeNormalise(player.Position - Owner.Position) * 0.01f; // set direction of movement but dont start moving
            }

            if (AITimer > spinUpTime && AITimer <= spinUpTime + accelerateTime)
            {
                int localTime = AITimer - spinUpTime;
                float interpolant = EasingFunctions.EaseOutExpo(localTime / (float)accelerateTime);
                
                Owner.Velocity = maxChargeSpeed * interpolant * Utilities.SafeNormalise(Owner.Velocity);
            }

            // revisit movement formula and add projectile barfing
            if (AITimer > spinUpTime + accelerateTime + chargeTime && AITimer <= spinUpTime + accelerateTime + chargeTime + slowDownTime)
            {
                //Owner.Velocity = maxChargeSpeed * Utilities.SafeNormalise(player.Position - Owner.Position);
                int localTime = AITimer - (spinUpTime + accelerateTime + chargeTime);
                float interpolant = 1 - EasingFunctions.EaseOutExpo(localTime / (float)slowDownTime);

                Owner.Velocity = maxChargeSpeed * interpolant * Utilities.SafeNormalise(Owner.Velocity);
            }            
        }
    }
}
