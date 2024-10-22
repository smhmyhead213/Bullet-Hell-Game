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

            ref float angularVelocity = ref Owner.ExtraData[1]; // index 0 is reserved 

            if (AITimer < spinUpTime)
            {
                angularVelocity += spinAngularAccel;

                for (int i = 0; i < 2; i++)
                {
                    int expandedi = Utilities.ExpandedIndex(i);
                    float angle = PI / 2;
                    RotateArm(i, -expandedi * angle, AITimer, spinUpTime, EasingFunctions.EaseInQuint);
                }
            }
            float maxChargeSpeed = 15f;

            Owner.Rotation += angularVelocity;

            if (AITimer > spinUpTime && AITimer <= spinUpTime + accelerateTime)
            {
                int localTime = AITimer - spinUpTime;
                float interpolant = localTime / (float)accelerateTime;
                
                Owner.Velocity = maxChargeSpeed * interpolant * Utilities.SafeNormalise(player.Position - Owner.Position);
            }

            // revisit movement formula and add projectile barfing
            if (AITimer >= spinUpTime + accelerateTime)
            {
                Owner.Velocity = maxChargeSpeed * Utilities.SafeNormalise(Vector2.Lerp(Owner.Velocity, player.Position - Owner.Position, 0.005f));
            }            
        }
    }
}
