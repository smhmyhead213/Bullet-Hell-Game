using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.Bosses.CrabBoss.Attacks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabNeutralState : CrabBossAttack
    {
        public CrabNeutralState(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            End();
        }

        public BossAttack PickAttack(int repetitions = 0)
        {
            if (repetitions > 5)
            {
                return new CrabPunch(CrabOwner);
            }
            else
            {
                int rng = Utilities.RandomInt(1, 7);

                BossAttack chosen = rng switch
                {
                    1 => new CrabPunch(CrabOwner),
                    2 => new CrabBombThrow(CrabOwner),
                    3 => new NeutralToCrabFlailChargeTransition(CrabOwner),
                    4 => new CrabProjectilePunches(CrabOwner),
                    5 => new CrabGrab(CrabOwner),
                    > 5 => new CrabSpray(CrabOwner),
                    // idk how this would get reached but whatever
                    _ => new CrabPunch(CrabOwner),
                };

                if (chosen.SelectionCondition())
                {
                    return chosen;
                }
                else
                {
                    return PickAttack(repetitions + 1);
                }
            }
        }
        public override BossAttack PickNextAttack()
        {
            // all attack logic goes here

            // gap close

            if (Owner.Position.Distance(player.Position) > 1000)
            {
                return new CrabPunch(CrabOwner);
            }

            return PickAttack();
        }
    }
}
