using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles;

using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabIdleState : CrabBossAttack
    {
        public CrabIdleState(CrabBoss owner) : base(owner)
        {

        }
        public override void Execute(int AITimer)
        {
            Owner.Velocity = 5f * Owner.Position.DirectionToPlayer();
            CrabOwner.FacePlayer();
        }
    }
}
