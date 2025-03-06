using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabLongCharge : CrabBossAttack
    {
        public CrabLongCharge(CrabBoss owner) : base(owner)
        {

        }
        public override void Execute(int AITimer)
        {
            int spinUpTime = 60;
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
