using System;
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
            TimeAlive++;
            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            Position = Position + Velocity;

            if (TimeAlive > 10)
            {
                ShouldRemoveOnEdgeTouch = true;
            }
        }
    }
}
