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
            if (AITimer == 0)
            {
                Arm(0).RotateLeg(PI / 2);
                Arm(1).RotateLeg(-PI / 2);
            }


            Owner.Rotation += Tau / 10;
        }
    }
}
