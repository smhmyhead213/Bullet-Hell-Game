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
        public override void TryEndAttack(ref int AITimer, ref int AttackNumber) // literally the same except it resets arm rotation after each attack
        {
            if (Owner.AITimer == Owner.BossAttacks[Owner.AttackNumber].EndTime && AttackNumber != 0)
            {
                Owner.AITimer = 0; //to prevent jank with EndAttack taking a frame, allows attacks to start on 0, change back to -1 if cringe things happen

                Owner.Rotation = 0;
                if (Owner.AttackNumber != Owner.BossAttacks.Length - 1)
                {
                    Owner.AttackNumber++;
                    Owner.BossAttacks[Owner.AttackNumber].InitialiseAttackValues();
                    CrabOwner.ResetArmRotations();
                }
                else
                {
                    Owner.AttackNumber = 1;
                    Owner.BossAttacks[Owner.AttackNumber].InitialiseAttackValues();
                    CrabOwner.ResetArmRotations();
                }
            }

            if (Owner.Health <= 0 && !Owner.IsDesperationOver && !HasResetAITimerForDesperation)
            {
                HasResetAITimerForDesperation = true;
                Owner.AttackNumber = 0;
                Owner.AITimer = -1;
            }

            if (Owner.IsDesperationOver)
            {
                Owner.Die();
            }
        }
        public CrabLeg Leg(int index)
        {
            return CrabOwner.Legs[index];
        }
    }
}
