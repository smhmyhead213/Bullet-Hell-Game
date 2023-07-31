using bullethellwhatever.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBossLowerClaw : CrabBossAppendage
    {
        public CrabBossLowerClaw(Entity owner, CrabLeg leg, string texture) : base(owner, leg, texture)
        {
            MaxHP = 50f;
            Health = 50f;
        }
    }
}