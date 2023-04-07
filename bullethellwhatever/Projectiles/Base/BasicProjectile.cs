using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bullethellwhatever.Projectiles.Enemy;

namespace bullethellwhatever.Projectiles.Base
{
    public class BasicProjectile : HarmfulProjectile
    {
        public override bool ShouldRemoveOnEdgeTouch() => true;
        public override Color Colour() => Color.Red;

    }
}
