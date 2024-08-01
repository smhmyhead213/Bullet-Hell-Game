using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles;

using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class DoNothing : CrabBossAttack
    {
        public DoNothing(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            Owner.Position.X += 3f;
            MainCamera.SetCameraPositon(Owner.Position);
            MainCamera.SetZoom(1f + AITimer / 400f);
        }
    }
}
