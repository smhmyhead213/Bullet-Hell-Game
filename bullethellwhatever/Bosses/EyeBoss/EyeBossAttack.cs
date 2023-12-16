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
            if (Entity.touchingLeft(Owner))
            {
                if (Owner.Velocity.X < 0)
                {
                    Owner.Velocity.X = Owner.Velocity.X * -1;
                }
            }

            if (Entity.touchingRight(Owner))
            {
                if (Owner.Velocity.X > 0)
                {
                    Owner.Velocity.X = Owner.Velocity.X * -1;
                }

            }

            if (Entity.touchingTop(Owner))
            {
                if (Owner.Velocity.Y < 0)
                    Owner.Velocity.Y = Owner.Velocity.Y * -1f;

            }

            if (Entity.touchingBottom(Owner))
            {
                if (Owner.Velocity.Y > 0)
                    Owner.Velocity.Y = Owner.Velocity.Y * -1f;
            }
        }

        public override void ExtraAttackEnd()
        {
            if (Owner.AttackNumber == Owner.BossAttacks.Length - 1 && EyeOwner.Phase != 2)
            {
                Owner.RandomlyArrangeAttacks();
            }
        }
    }
}
