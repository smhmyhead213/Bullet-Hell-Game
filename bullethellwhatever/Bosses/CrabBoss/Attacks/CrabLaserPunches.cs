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

            int pullBackArmTime = 20;

            if (AITimer < pullBackArmTime)
            {
                // move arm in rough circle around its start
                Vector2 rootToEnd = Arm(0).WristPosition() - Arm(0).Position;
                Vector2 targetPos = Arm(0).Position + Utilities.RotateVectorClockwise(rootToEnd, PI / 3);

                Arm(0).TouchPoint(Vector2.Lerp(CrabOwner.ArmRestingEnds[0], targetPos, pullBackArmTime / (float)AITimer));
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabFlail(CrabOwner, 0);
        }

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
