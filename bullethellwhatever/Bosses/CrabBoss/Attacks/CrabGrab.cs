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
            int pullBackArmTime = 60;
            int swingTime = 30;
            float pullBackAngle = PI / 2f;
            float swingThroughAngle = 2 * PI / 3f;
            
            float armLength = Arm(0).WristLength();
            int expandedi = Utilities.ExpandedIndex(1);

            if (AITimer < pullBackArmTime)
            {
                float interpolant = AITimer / (float)pullBackArmTime;
                Arm(1).LerpToPoint(Arm(1).Position + new Vector2(0f, armLength).Rotate(-expandedi * pullBackAngle), interpolant);
            }
            if (AITimer >= pullBackArmTime && AITimer < pullBackArmTime + swingTime)
            {
                int localTime = AITimer - pullBackArmTime;
            }
        }

        public override void ExtraDraw(SpriteBatch s, int AITimer)
        {
            
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
