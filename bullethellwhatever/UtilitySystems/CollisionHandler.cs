using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles;
using bullethellwhatever.Projectiles.Base;

namespace bullethellwhatever.UtilitySystems
{
    public static class CollisionHandler
    {
        public static bool IsProjectileCollidingWithEntity(Projectile projectile, Entity entity)
        {
            if (projectile.Hitbox.Intersects(entity.Hitbox))
            {
                return true;
            }
            return false;
        }

        //public static bool IsDeathrayCollidingWithEntity(BaseDeathray deathray, Entity entity)
        //{

        //}
    }
}
