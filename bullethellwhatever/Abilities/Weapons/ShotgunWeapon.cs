using FMOD;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.Projectiles;
using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.Abilities.Weapons
{
    public class ShotgunWeapon : Weapon
    {
        public ShotgunWeapon(Player player) : base(player)
        {

        }
        public override void WeaponInitialise()
        {
            PrimaryFireCoolDownDuration = 60;
            SecondaryFireCoolDown = 120;
        }

        public override void AI()
        {
            
        }

        public override void PrimaryFire()
        {
            float spreadAngle = PI;
            int projectiles = 1; // keep this an odd number
            float angleBetween = spreadAngle / projectiles;
            float damage = 1.3f;
            float projectileSpeed = 30f;

            Vector2 toMouse = MousePositionWithCamera() - Owner.Position;
            float startingAngle = Utilities.VectorToAngle(toMouse) - (projectiles / 2) * angleBetween;

            for (int i = 0; i < projectiles; i++)
            {
                float firingAngle = startingAngle + i * angleBetween;

                Projectile p = SpawnProjectile(Owner.Position, projectileSpeed * Utilities.AngleToVector(firingAngle), damage, 1, "box", Vector2.One, Owner, false, Color.Yellow, true, true);

                p.Rotation = firingAngle;

                p.AddTrail(14);
            }
        }

        public override void SecondaryFire()
        {
            
        }
    }
}
