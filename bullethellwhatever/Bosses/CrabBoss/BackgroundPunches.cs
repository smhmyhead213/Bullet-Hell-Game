using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Bosses.CrabBoss.Projectiles;
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace bullethellwhatever.Bosses.CrabBoss
{
    public class BackgroundPunches : CrabBossAttack
    {
        public BackgroundPunches(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer % (1000);

            CrabOwner.SetBoosters(false);

            float sine = Sin(AITimer / 40f) + 1f;
            sine = sine / 2f;
            CrabOwner.SetDepth(sine);

            CrabOwner.SetArmDepth(0, sine);
            CrabOwner.SetArmDepth(1, sine);

        }
            

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
