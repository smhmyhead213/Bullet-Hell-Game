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
            float armLength = Arm(0).WristLength();

            if (AITimer < pullBackArmTime)
            {
                // move arm in rough circle around its start

                float holdOutDistEnd = armLength * 0.7f;
                float interpolant = EasingFunctions.EaseOutExpo((float)AITimer / pullBackArmTime);
                float holdOutDistance = MathHelper.Lerp(armLength, holdOutDistEnd, interpolant);
                Vector2 rootToEnd = holdOutDistance * Utilities.SafeNormalise(RestingPosition(0) - Arm(0).Position);
                Vector2 targetPosition = Arm(0).Position + Utilities.RotateVectorClockwise(rootToEnd, PI / 2);

                //Arm(0).TouchPoint(Vector2.Lerp(RestingPosition(0), targetPosition, interpolant), true);
                RotateArm(0, PI / 2, AITimer, pullBackArmTime, EasingFunctions.EaseOutExpo);
            }

            if (AITimer > pullBackArmTime && AITimer <= pullBackArmTime + punchSwingTime)
            {
                int localTime = AITimer - pullBackArmTime;
                Vector2 punchTarget = Arm(0).RestPositionEnd() + new Vector2(0f, -10f);
                float interpolant = (float)localTime / punchSwingTime;

                //Arm(0).TouchPoint(Vector2.Lerp(Arm(0).WristPosition(), punchTarget, interpolant), true);
                Arm(0).LerpToRestPosition(interpolant);
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabFlail(CrabOwner, 0);
        }

        public override void ExtraDraw(SpriteBatch s)
        {
            for (int i = 0; i < 2; i++)
            {
                Drawing.BetterDraw("box", RestingPosition(i), null, Color.Red, 0f, Vector2.One, SpriteEffects.None, 1f);
            }
        }
    }
}
