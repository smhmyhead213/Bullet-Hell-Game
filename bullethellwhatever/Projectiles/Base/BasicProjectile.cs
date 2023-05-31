using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.Projectiles.Base
{
    public class BasicProjectile : Projectile
    {
        public override void Spawn(Vector2 position, Vector2 velocity, float damage, string texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            base.Spawn(position, velocity, damage, texture, acceleration, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);
        }
        public override void AI()
        {
            base.AI();
        }
    }
}
