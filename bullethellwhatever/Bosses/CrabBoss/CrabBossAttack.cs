using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBossAttack : BossAttack
    {
        public CrabBoss CrabOwner;
        public CrabBossAttack(int endTime) : base (endTime)
        {
            EndTime = endTime;
            
        }

        public override void InitialiseAttackValues()
        {
            CrabOwner = (CrabBoss)Owner;
        }
    }
}
