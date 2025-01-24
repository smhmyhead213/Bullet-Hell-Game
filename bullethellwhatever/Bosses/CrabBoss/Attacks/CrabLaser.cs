using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.DrawCode;
using Microsoft.VisualBasic.Logging;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabLaser : CrabBossAttack
    {

        public CrabLaser(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            foreach (CrabArm leg in CrabOwner.Arms)
            {
                Vector2 targetPosition = leg.Position + 1f * (MousePositionWithCamera() - leg.Position); // change this to make bend testing easier
                leg.TouchPoint(targetPosition);
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabFlail(CrabOwner, 0);
        }

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
