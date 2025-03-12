using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.Bosses;
using bullethellwhatever.DrawCode;
using Microsoft.Xna.Framework;
using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabGrabMiss : CrabBossAttack
    {
        public CrabGrabMiss(CrabBoss owner) : base (owner)
        {

        }

        public override void Execute(int AITimer)
        {
            if (AITimer == 20)
            {
                ShockwaveRing shockwave = new ShockwaveRing(200f, 0, 20000, 30);
                shockwave.ScrollSpeed = 0.04f;
                shockwave.Spawn(Owner.Position, Owner, Color.White);
            }
        }
    }
}
