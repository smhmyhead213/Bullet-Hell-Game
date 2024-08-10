using bullethellwhatever.BaseClasses;
using bullethellwhatever.Bosses.EyeBoss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class EyeBossAttack : BossAttack
    {
        public EyeBoss EyeOwner;
        public Pupil Pupil;
        public EyeBossAttack(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            EyeOwner = (EyeBoss)Owner;
            Pupil = EyeOwner.Pupil;
        }

        public override void HandleBounces()
        {
            if (Owner.TouchingLeft())
            {
                if (Owner.Velocity.X < 0)
                {
                    Owner.Velocity.X = Owner.Velocity.X * -1;
                }
            }

            if (Owner.TouchingRight())
            {
                if (Owner.Velocity.X > 0)
                {
                    Owner.Velocity.X = Owner.Velocity.X * -1;
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

        public override void ExtraAttackEnd()
        {
            if (Owner.AttackNumber == Owner.BossAttacks.Length - 1 && EyeOwner is not EyeBossPhaseTwoMinion)
            {
                Owner.RandomlyArrangeAttacks();
            }
        }
    }
}
