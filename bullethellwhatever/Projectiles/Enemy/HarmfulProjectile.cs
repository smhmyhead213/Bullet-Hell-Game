using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.Projectiles.Enemy
{
    public class HarmfulProjectile : Projectile
    {
        public override bool IsHarmful() => true;

    }
}
