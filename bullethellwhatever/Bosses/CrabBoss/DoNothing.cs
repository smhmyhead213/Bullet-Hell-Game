using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles;

using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class DoNothing : CrabBossAttack
    {
        public DoNothing(CrabBoss owner) : base(owner)
        {

        }
        public override void Execute(int AITimer)
        {
            Owner.Velocity = Vector2.Zero;

            int chungusTime = 30;
            int resetDuration = 180;

            if (AITimer <= chungusTime)
            {
                float interp = AITimer / (float)chungusTime;
                Arm(0).LerpToPoint(Arm(0).Position + new Vector2(0f, 10f), interp);
            }

            if (AITimer > chungusTime && AITimer <= resetDuration)
            {
                float interp = (AITimer - chungusTime) / (float)resetDuration;
                Arm(0).LerpToRestPosition(interp);
            }

            if (AITimer == resetDuration)
            {
                End();
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new DoNothing(CrabOwner);
        }
    }
}
