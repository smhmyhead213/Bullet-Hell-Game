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
                CurrentAttack.ExtraDraw(spriteBatch, AITimer);
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

        public virtual void DisplayBossHPBar()
        {
            HealthBar hpBar = new HealthBar("box", new Vector2(900f, 30f), this, new Vector2(GameWidth / 2, GameHeight / 20 * 19));
            hpBar.SetAssociatedEntity(this);
            hpBar.DisplayPercentage = true;
            hpBar.Display();
        }

        public override void Die()
        {
            base.Die();

            UI.CreateAfterBossMenu();
        }
    }
}
