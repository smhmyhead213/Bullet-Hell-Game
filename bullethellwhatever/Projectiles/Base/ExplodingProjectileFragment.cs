using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Projectiles.Base
{
    public class ExplodingProjectileFragment : BasicProjectile
    {
        public override void AI()
        {
            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            Position = Position + Velocity;

            if (AITimer > 10)
            {
                ShouldRemoveOnEdgeTouch = true;
            }
        }
    }
}
