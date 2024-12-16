using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabFlit : CrabBossAttack
    {
        public CrabFlit(CrabBoss owner) : base(owner)
        {

        }
        public override void Execute(int AITimer)
        {
            
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
