
using bullethellwhatever.BaseClasses;
using bullethellwhatever.NPCs;
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

        public CrabBossAttack(CrabBoss owner) : base(owner) 
        {
            CrabOwner = owner;
        }
        public override void InitialiseAttackValues()
        {
            CrabOwner = (CrabBoss)Owner;
        }

        public override void ExtraAttackEnd()
        {
            CrabOwner.ResetArmRotations();
        }

        public override void HandleBounces()
        {
            if (Owner.TouchingLeft())
            {
                if (Owner.Velocity.X < 0)
                {
                    Owner.Velocity.X = Owner.Velocity.X * -1;
                    if (CrabOwner.StartedDeathAnim)
                        Owner.RotationalVelocity = CrabOwner.SpinVelOnDeath;
                }
            }

            if (Owner.TouchingRight())
            {
                if (Owner.Velocity.X > 0)
                {
                    Owner.Velocity.X = Owner.Velocity.X * -1;
                    if (CrabOwner.StartedDeathAnim)
                        Owner.RotationalVelocity = CrabOwner.SpinVelOnDeath;
                }

            }

            if (Owner.TouchingTop())
            {
                if (Owner.Velocity.Y < 0)
                    Owner.Velocity.Y = Owner.Velocity.Y * -1f;

            }

            if (Owner.TouchingBottom())
            {
                if (Owner.Velocity.Y > 0)
                    Owner.Velocity.Y = Owner.Velocity.Y * -1f;
            }
        }
        public CrabLeg Leg(int index)
        {
            return CrabOwner.Legs[index];
        }
    }
}
