using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using bullethellwhatever.Bosses;
using bullethellwhatever.NPCs;
using bullethellwhatever.DrawCode.UI;
using bullethellwhatever.DrawCode.UI.Buttons;
using bullethellwhatever.MainFiles;

namespace bullethellwhatever.BaseClasses
{
    public class Boss : NPC
    {
        public int AttackNumber; //position in pattern

        public BossAttack[] BossAttacks;

        public virtual void InitialiseBoss()
        {
            AttackNumber = 0;

            Health = MaxHP;

            ReplaceAttackPattern(BossAttacks);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (BossAttacks is not null && AttackNumber < BossAttacks.Length)
                BossAttacks[AttackNumber].ExtraDraw(spriteBatch);
        }

        public override void AI()
        {
            base.AI();

            ExecuteCurrentAttack();
        }
        public void RandomlyArrangeAttacks()
        {
            BossAttack[] newOrder = Utilities.RandomlyRearrangeArray(BossAttacks);

            // never do the same attack twice

            if (BossAttacks.Length > 1 && newOrder[0] == BossAttacks[BossAttacks.Length - 1]) // if the first new attack is the same as the last old one
            {
                BossAttack bucket = newOrder[1]; // swap the new first and new second attacks
                newOrder[1] = newOrder[0];
                newOrder[0] = bucket;
            }

            BossAttacks = newOrder;
        }
        public virtual void ExecuteCurrentAttack()
        {
            if (BossAttacks.Length > 0)
            {
                BossAttacks[AttackNumber].Execute(ref AITimer, ref AttackNumber);

                BossAttacks[AttackNumber].TryEndAttack(ref AITimer, ref AttackNumber);
            }
        }

        public void ReplaceAttackPattern(BossAttack[] attacks)
        {
            if (BossAttacks is not null)
            {
                BossAttacks = attacks;

                for (int i = 0; i < BossAttacks.Length; i++)
                {
                    BossAttacks[i].Owner = this;
                    BossAttacks[i].InitialiseAttackValues();
                }

                AITimer = 0;
                AttackNumber = 0;
                //if (attacks.Length == 1)
                //    AttackNumber = 0; // remember 0 is desp / death anim
                //else AttackNumber = 1;
            }
        }

        public override void Die()
        {
            base.Die();

            UI.CreateAfterBossMenu();
        }
    }
}
