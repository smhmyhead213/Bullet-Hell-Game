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
            int moveInDuration = 60;
            int numberOfSpins = 10;
            int waitTimeAfterStopping = 60;

            if (AITimer == 0)
            {
                MainCamera.LockCamera(true);
                //player.LockMovement();
                Owner.Position = Utilities.CentreWithCamera() - new Vector2(distanceToLeftOfPlayer, distanceAbovePlayer);
                Owner.Velocity = Vector2.UnitX * distanceToLeftOfPlayer / (float)moveInDuration;
                Owner.RotationalVelocity = numberOfSpins * Tau / moveInDuration;
            }

            if (AITimer == moveInDuration)
            {
                Owner.Velocity = Vector2.Zero;
                Owner.RotationalVelocity = 0;
                //player.UnlockMovement();
                MainCamera.LockCamera(false);
            }

            if (AITimer == moveInDuration + waitTimeAfterStopping)
            {
                End();
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabPunch(CrabOwner);
        }
    }
}
