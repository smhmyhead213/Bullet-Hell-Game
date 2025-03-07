using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabGrab : CrabBossAttack
    {
        public CrabGrab(CrabBoss owner) : base(owner)
        {

        }
        public override void Execute(int AITimer)
        {
            int expandedi = Utilities.ExpandedIndex(1);

            int pullBackArmTime = 45;
            int waitTime = 30;

            int swingTime = 30;
            float pullBackAngle = -expandedi * PI / 2f;
            float finalSwingAngle = -expandedi * -PI / 18f;
            
            float initialarmLength = Arm(0).WristLength(); // kinda hacky but this gives the original non enlarged length
            float armLength = Arm(1).WristLength();

            if (AITimer < pullBackArmTime)
            {
                // calculate the size the arm must be to reach the player

                float distToPlayer = Arm(1).Position.Distance(player.Position);
                float scaleFactor = distToPlayer / initialarmLength;
                float fractionOfLengthHoldNearBody = 0.3f;

                //Arm(1).SetScale(scaleFactor);

                float interpolant = EasingFunctions.EaseOutExpo(AITimer / (float)pullBackArmTime);

                Arm(1).LerpToPoint(Arm(1).Position + new Vector2(0f, armLength * fractionOfLengthHoldNearBody).Rotate(pullBackAngle), interpolant, true);

                Arm(1).LowerClaw.LerpRotation(0f, -expandedi * PI / 2f, interpolant);
                //Arm(1).UpperClaw.LerpRotation(0f, -expandedi * -PI / 2, interpolant);

            }

            if (AITimer >= pullBackArmTime + waitTime && AITimer < pullBackArmTime + swingTime + waitTime)
            {
                int localTime = AITimer - (pullBackArmTime + waitTime);
                float interpolant = EasingFunctions.Linear(localTime / (float)swingTime);
                float finalArmLength = armLength;
                float armLengthNow = MathHelper.Lerp(armLength, finalArmLength, interpolant);
                Vector2 finalPoint = Arm(1).Position + new Vector2(0f, armLengthNow).Rotate(finalSwingAngle);

                Arm(1).LerpToPoint(finalPoint, interpolant, false);
                Arm(1).LowerClaw.LerpToZero(interpolant);

                //Arm(1).LerpRotation(pullBackAngle, finalSwingAngle, interpolant);
            }
        }

        public override bool SelectionCondition()
        {
            return Owner.DistanceFromPlayer() > 1200f;
        }
        public override BossAttack PickNextAttack()
        {
            int nextAttack = Utilities.RandomInt(1, 3);
            if (nextAttack == 1 || nextAttack == 2)
                return new CrabPunchToNeutralTransition(CrabOwner);
            else
                return new CrabPunchToProjectileSpreadTransition(CrabOwner);
        }
    }
}
