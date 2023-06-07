using bullethellwhatever.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Projectiles.Base
{
    public class ExponentialAcceleratingProjectile : Projectile
    {
        public int AccelTime;
        public float Exponent;
        public ExponentialAcceleratingProjectile(int accelTime, float exponent)
        {
            AccelTime = accelTime;
            Exponent = exponent;
        }

        public override void AI()
        {
            if (Velocity.Length() != 1 && Velocity.Length() != 0)
            {
                Velocity = Utilities.SafeNormalise(Velocity) * (MathF.Pow(AITimer * 2f / AccelTime, Exponent) + 0.1f);
            }

            else Velocity = Velocity * (MathF.Pow(AITimer * 2f / AccelTime, Exponent) + 0.1f); //Small factor is added to ensure that the velocity isnt multiplied by 0.

            base.AI();
        }
    }
}
