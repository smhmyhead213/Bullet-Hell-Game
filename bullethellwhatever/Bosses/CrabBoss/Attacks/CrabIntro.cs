using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using System.Windows.Forms.Design;
using bullethellwhatever.DrawCode;
using System.Runtime.InteropServices;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabIntro : CrabBossAttack
    {
        public CrabIntro(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            float distanceAbovePlayer = GameHeight / 4f;
            float distanceToLeftOfPlayer = GameWidth * 2f;

            if (AITimer == 0)
            {
                MainCamera.LockCamera(true);
                Owner.Position = Utilities.CentreWithCamera() - new Vector2(distanceToLeftOfPlayer, distanceAbovePlayer);
            }

            if (Owner.Position.X < player.Position.X)
            {
                Owner.Velocity = 35f * Vector2.UnitX;
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabPunch(CrabOwner);
        }
    }
}
