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

        public BossAttack PickAttack()
        {
            List<CrabBossAttack> attacks =
                [new CrabGrab(CrabOwner),
                    new CrabPunch(CrabOwner),
                    new CrabProjectilePunches(CrabOwner),
                    new NeutralToCrabFlailChargeTransition(CrabOwner),
                    new CrabBombThrow(CrabOwner),
                    new CrabSpray(CrabOwner)];

            List<float> probabilities =
                [0.99f, // big chance for grab atatck if player is close
                    0.2f,
                    0.15f,
                    0.22f,
                    0.23f,
                    0.15f];

            float totalProbability = probabilities.Sum();
            probabilities = probabilities.Select(p => p / totalProbability).ToList(); // normalise probabilities

            int rerolls = 0;
            int maxRerolls = 5;

            while (rerolls < maxRerolls)
            {
                float runningTotal = 0f;
                float rng = Utilities.RandomFloat(0f, 1f);

                for (int i = 0; i < probabilities.Count; i++)
                {
                    if (rng <= probabilities[i])
                    {
                        if (attacks[i].SelectionCondition())
                        {
                            return attacks[i];
                        }
                        else
                        {
                            rerolls++;
                            runningTotal += probabilities[i];
                        }
                    }
                }
            }

            return new CrabPunch(CrabOwner);
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
