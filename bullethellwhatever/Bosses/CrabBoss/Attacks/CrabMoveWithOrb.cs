using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabMoveWithOrb : CrabBossAttack
    {
        public CrabMoveWithOrb(CrabBoss owner) : base(owner)
        {

        }
        public override void Execute(int AITimer)
        {
            int openArmsTime = 30;
            float totalAngleToOpenArms = PI / 5;

            if (AITimer < openArmsTime)
            {
                for (int i = 0; i < 2; i++)
                {
                    int expandedi = Utilities.ExpandedIndex(i);

                    Leg(i).RotateLeg(-expandedi * totalAngleToOpenArms / openArmsTime);
                }
            }
        }
    }
}
