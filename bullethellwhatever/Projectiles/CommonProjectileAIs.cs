using bullethellwhatever.MainFiles;
using bullethellwhatever.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Projectiles
{
    public static class CommonProjectileAIs
    {
        /// <summary>
        /// Ensure that the used extra data slot is unused and 0 to avoid issues.
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="target"></param>
        /// <param name="extraDataSlot"></param>
        public static void Homing(this Projectile projectile, NPC target, int extraDataSlot)
        {
            ref float homingTime = ref projectile.ExtraData[extraDataSlot];

            homingTime++;

            projectile.Velocity = 0.4f / projectile.Updates * Vector2.Normalize(target.Position - projectile.Position) * homingTime;
        }
    }
}
