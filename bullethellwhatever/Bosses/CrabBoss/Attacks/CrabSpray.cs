﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabSpray : CrabBossAttack
    {
        public CrabSpray(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int chargeUpTime = 40;
            float holdOutAngle = PI / 6f;
            float armLength = Arm(0).WristLength();

            for (int i = 0; i < 2; i++)
            {
                int expandedi = Utilities.ExpandedIndex(i);

                if (AITimer < chargeUpTime)
                {                  
                    float interpolant = AITimer / (float)chargeUpTime;
                    float finalLength = armLength * 0.7f;
                    float usedLength = MathHelper.Lerp(armLength, finalLength, interpolant);
                    float usedAngle = MathHelper.Lerp(0f, holdOutAngle, interpolant);

                    Arm(i).TouchPoint(Arm(i).Position + new Vector2(0, usedLength).Rotate(usedAngle * -expandedi).Rotate(Owner.Rotation));
                }
            }
        }
    }
}
