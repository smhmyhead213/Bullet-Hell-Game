using bullethellwhatever.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBossUpperArm : CrabBossAppendage
    {
        public CrabBossUpperArm(Entity owner, CrabLeg leg, string texture, int legIndex) : base(owner, leg, texture, legIndex) 
        {
            MaxHP = 100f;
            Health = 100f;
        }

        public override void Update()
        {
            base.Update();
           
        }
        
    }
}
