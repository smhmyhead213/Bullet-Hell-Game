using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Projectiles.Base
{
    public class PlayerHealingProjectile : Projectile
    {
        public float HealAmount;
        public PlayerHealingProjectile(float healAmount) : base()
        { 
            HealAmount = healAmount;
        }

        public override void DamagePlayer()
        {
            player.Health = player.Health + HealAmount;

            Die();
        }
    }
}
