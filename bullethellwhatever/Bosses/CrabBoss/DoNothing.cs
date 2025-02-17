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
            foreach (CrabArm arm in CrabOwner.Arms)
            {
                arm.LowerArm.Rotate(PI / 180f);
            }
        }
    }
}
