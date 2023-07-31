using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBossLowerArm : CrabBossAppendage
    {
        public CrabBossLowerArm(Entity owner, CrabLeg leg, string texture) : base(owner, leg, texture)
        {
            MaxHP = 75f;
            Health = 75f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
                      
        }
    }
}
