using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using bullethellwhatever.Projectiles.Base;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.DrawCode;
using Microsoft.VisualBasic.Logging;
using System.Security.Policy;
using SharpDX.Direct2D1;

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

            int pullBackArmTime = 50;
            int punchSwingTime = 3;
            int resetTime = 30;
            int delayAfterPunchToCloseClaw = 20; // should be less than reset time
            int totalPunchTime = pullBackArmTime + punchSwingTime + resetTime;

            float armLength = Arm(0).WristLength();
            int armZeroTimer = AITimer;
            int armOneTimer = AITimer - (totalPunchTime / 2);
            float pullBackAngle = PI / 2.4f;

            for (int i = 0; i < 2; i++)
            {
                // decide which timer to use
                int usedTimer = (i == 0 ? armZeroTimer : armOneTimer) % totalPunchTime;

                int expandedi = Utilities.ExpandedIndex(i);

                if (usedTimer < pullBackArmTime && usedTimer >= 0)
                {
                    // move arm in rough circle around its start

                    float holdOutDistEnd = armLength * 0.5f;
                    float interpolant = EasingFunctions.EaseOutExpo((float)usedTimer / pullBackArmTime);
                    float holdOutDistance = MathHelper.Lerp(armLength, holdOutDistEnd, interpolant);
                    Vector2 rootToEnd = holdOutDistance * Utilities.SafeNormalise(RestingPosition(i) - Arm(i).Position);
                    float armRotation = MathHelper.Lerp(0f, pullBackAngle, interpolant);
                    Vector2 targetPosition = Arm(i).Position + rootToEnd.Rotate(-expandedi * armRotation);

                    Arm(i).TouchPoint(targetPosition, false);

                    // open up the claw to shoot laser at player
                    Arm(i).LowerClaw.LerpRotation(0f, -expandedi * PI / 2, interpolant);
                }

                if (usedTimer > pullBackArmTime && usedTimer <= pullBackArmTime + punchSwingTime)
                {
                    int localTime = usedTimer - pullBackArmTime;
                    Vector2 punchTarget = Arm(i).Position + new Vector2(0f, Arm(i).WristLength()).Rotate(Owner.Rotation);
                    float interpolant = (float)localTime / punchSwingTime;

                    Arm(i).LerpToPoint(punchTarget, interpolant, false);
                }

                if (usedTimer > pullBackArmTime + punchSwingTime && usedTimer <= pullBackArmTime + punchSwingTime + resetTime)
                {
                    int localTime = usedTimer - (pullBackArmTime + punchSwingTime);
                    int rayDuration = resetTime - delayAfterPunchToCloseClaw;

                    float interpolant = (float)localTime / resetTime;
                    Arm(i).LerpToRestPosition(interpolant, false);
                    
                    if (usedTimer == pullBackArmTime + punchSwingTime + 1)
                    {
                        Deathray d = SpawnDeathray(Arm(i).WristPosition(), Arm(i).UpperArm.RotationFromV(), 1f, rayDuration, "box", Arm(i).UpperArm.Width(), GameWidth, 0f, true, Color.Red, "DeathrayShader2", Owner);
                    }

                    if (usedTimer > pullBackArmTime + punchSwingTime + delayAfterPunchToCloseClaw)
                    {
                        int localerTime = localTime - delayAfterPunchToCloseClaw;
                        int clawCloseTime = 6; // close the claws faster than the arm resets and MAKE SURE THIS IS LESS THAN RESETTIME
                        float clawCloseInterpolant = MathHelper.Clamp((float)localerTime / clawCloseTime, 0f, 1f);

                        Arm(i).LowerClaw.LerpRotation(-expandedi * PI / 2, 0f, EasingFunctions.EaseOutExpo(clawCloseInterpolant));
                    }
                }
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabFlail(CrabOwner, 0);
        }
    }
}
