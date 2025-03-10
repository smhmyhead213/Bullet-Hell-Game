using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabGrabPunish : CrabBossAttack
    {
        public CrabGrabPunish(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int moveToPositionTime = 20;
            int chargeUpTime = 60;

            if (AITimer < moveToPositionTime)
            {
                float progress = AITimer / (float)moveToPositionTime;
                float distanceFromBodyToHold = 40f;
            }
        }
    }
}
