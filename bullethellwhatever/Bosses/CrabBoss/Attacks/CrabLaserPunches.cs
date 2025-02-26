using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.DrawCode;
using Microsoft.VisualBasic.Logging;
using System.Security.Policy;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabLaserPunches : CrabBossAttack
    {

        public CrabLaserPunches(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            //return;

            int pullBackArmTime = 90;
            int punchSwingTime = 5;
            int resetTime = 30;

            int totalPunchTime = pullBackArmTime + punchSwingTime;

            float armLength = Arm(0).WristLength();
            int armZeroTimer = AITimer;
            int armOneTimer = AITimer - 30;
            float pullBackAngle = PI / 4;

            for (int i = 0; i < 2; i++)
            {
                // decide which timer to use
                int usedTimer = i == 0 ? armZeroTimer : armOneTimer;

                int expandedi = Utilities.ExpandedIndex(i);

                if (usedTimer < pullBackArmTime && usedTimer >= 0)
                {
                    // move arm in rough circle around its start

                    float holdOutDistEnd = armLength * 0.7f;
                    float interpolant = EasingFunctions.EaseOutExpo((float)usedTimer / pullBackArmTime);
                    float holdOutDistance = MathHelper.Lerp(armLength, holdOutDistEnd, interpolant);
                    Vector2 rootToEnd = holdOutDistance * Utilities.SafeNormalise(RestingPosition(i) - Arm(i).Position);
                    Vector2 targetPosition = Arm(i).Position + Utilities.RotateVectorClockwise(rootToEnd, PI / 2);

                    //Arm(0).TouchPoint(Vector2.Lerp(RestingPosition(0), targetPosition, interpolant), true);
                    RotateArm(i, -expandedi * pullBackAngle, usedTimer, pullBackArmTime, EasingFunctions.EaseOutExpo);
                }

                if (usedTimer > pullBackArmTime && usedTimer <= pullBackArmTime + punchSwingTime)
                {
                    int localTime = usedTimer - pullBackArmTime;
                    Vector2 punchTarget = Arm(i).Position + new Vector2(0f, Arm(i).WristLength()).Rotate(Owner.Rotation);
                    float interpolant = (float)localTime / punchSwingTime;

                    Arm(i).LerpToPoint(punchTarget, interpolant);
                }
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabFlail(CrabOwner, 0);
        }
    }
}
