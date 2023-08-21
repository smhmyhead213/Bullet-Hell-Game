using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class TestAttack : CrabBossAttack
    {
        public TestAttack(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            CrabOwner.Rotation = CrabOwner.Rotation + PI / 60f;
            CrabOwner.Legs[0].UpperArm.Rotate(PI / 60);
        }
    }
}
