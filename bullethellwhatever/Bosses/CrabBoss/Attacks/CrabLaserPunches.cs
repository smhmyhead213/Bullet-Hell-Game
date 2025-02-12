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
            //foreach (CrabArm leg in CrabOwner.Arms)
            //{
            //    Vector2 targetPosition = leg.Position + 1f * (MousePositionWithCamera() - leg.Position); // change this to make bend testing easier
            //    leg.TouchPoint(targetPosition);
            //}

            int pullBackArmTime = 90;

            if (AITimer < pullBackArmTime)
            {
                // move arm in rough circle around its start

                float interpolant = EasingFunctions.EaseOutExpo((float)AITimer / pullBackArmTime);
                float armLength = Arm(0).Length();
                float holdOutDistance = MathHelper.Lerp(armLength, armLength / 2, interpolant);
                Vector2 rootToEnd = holdOutDistance * Utilities.SafeNormalise(RestingPosition(0) - Arm(0).Position);
                Vector2 targetPosition = Arm(0).Position + Utilities.RotateVectorClockwise(rootToEnd, PI / 3);

                Arm(0).TouchPoint(Vector2.Lerp(CrabOwner.ArmRestingEnds[0], targetPosition, interpolant), false);
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabFlail(CrabOwner, 0);
        }

        public override void ExtraDraw(SpriteBatch s)
        {
            //Drawing.BetterDraw("box", TargetPosition(), null, Color.Red, 0f, Vector2.One, SpriteEffects.None, 1f);
        }
    }
}
