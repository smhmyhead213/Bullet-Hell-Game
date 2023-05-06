using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Projectiles.Base
{
    public class SizeChangingProjectile : BasicProjectile
    {
        Vector2 GrowthRate;
        public SizeChangingProjectile(float horizontalGrowthRate, float verticalGrowthRate)
        {
            GrowthRate = new Vector2(horizontalGrowthRate, verticalGrowthRate);
        }

        public override void AI()
        {
            TimeAlive++;
            Size = Size + GrowthRate;

            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            Position = Position + Velocity;
        }
    }
}
