using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public abstract class CrabArmBehaviour
    {
        public CrabBoss Owner;
        public int ArmIndex;
        public int AITimer;
        public bool Done;
        public CrabArmBehaviour(CrabBoss owner, int armIndex)
        {
            Owner = owner;
            ArmIndex = armIndex;
            Done = false;
        }

        public abstract void Execute();
        public CrabArm Arm()
        {
            return Owner.Arms[ArmIndex];
        }

        public void SignalDone()
        {
            Done = true;
        }
    }
}
