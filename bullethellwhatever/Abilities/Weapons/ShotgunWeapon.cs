using FMOD;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.Projectiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses.Entities;
using bullethellwhatever.AssetManagement;

namespace bullethellwhatever.Abilities.Weapons
{
    public class ShotgunWeapon : Weapon
    {
        public ShotgunWeapon(Player player, string iconTexture) : base(player, iconTexture)
        {

        }
        public override void WeaponInitialise()
        {
            PrimaryFireCoolDownDuration = 60;
            SecondaryFireCoolDownDuration = 120;
        }

        public override void AI()
        {
            
        }

        public override void PrimaryFire()
        {
            float spreadAngle = PI / 3;
            int projectiles = 1; // keep this an odd number
            float angleBetween = spreadAngle / projectiles;
            float damage = 1.3f;
            float projectileSpeed = 30;

            Vector2 toMouse = MousePositionWithCamera() - Owner.Position;
            float startingAngle = Utilities.VectorToAngle(toMouse) - (projectiles / 2) * angleBetween;

            for (int i = 0; i < projectiles; i++)
            {
                float firingAngle = startingAngle + i * angleBetween;

                SharpShot p = SpawnProjectile<SharpShot>(Owner.Position, projectileSpeed * Utilities.AngleToVector(firingAngle), damage, 1, "box", Vector2.One, Owner, false, Color.Yellow, true, true);

                p.Label = EntityLabels.SharpShot;

                p.Rotation = firingAngle;
                p.Opacity = 1f;

                p.AddTrail(14);
            }
        }

        public override void SecondaryFire()
        {
            float projectileSpeed = 5f;
            float damage = 0.1f;

            Projectile p = SpawnProjectile<Projectile>(Owner.Position, projectileSpeed * Utilities.SafeNormalise(MousePositionWithCamera() - Owner.Position), damage, 1, "box", Vector2.One, Owner, false, Color.LightGoldenrodYellow, true, true);

            p.Label = EntityLabels.SharpShotReflector;

            p.SetExtraAI(new Action(() =>
            {
                p.Velocity *= 0.99f;

                if (p.Velocity.Length() < 0.1f)
                {
                    p.Velocity = Vector2.Zero;
                }

            }));
        }
    }
}
