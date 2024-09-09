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
        public virtual void InitialiseBoss()
        {
            Health = MaxHP;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (CurrentAttack is not null)
                CurrentAttack.ExtraDraw(spriteBatch);
        }

        public override void AI()
        {
            ExecuteCurrentAttack();

            base.AI();
        }

        public virtual void ExecuteCurrentAttack()
        {
            CurrentAttack.Execute(AITimer);
        }

        public override void Die()
        {
            base.Die();

            UI.CreateAfterBossMenu();
        }
    }
}
