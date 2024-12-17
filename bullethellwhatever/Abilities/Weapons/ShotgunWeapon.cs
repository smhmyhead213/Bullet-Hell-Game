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
            float spreadAngle = PI / 3;
            int projectiles = 5; // keep this an odd number
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
                p.Opacity = 0f;

                p.SetExtraAI(new Action(() =>
                {
                    if (p.Opacity < 1f)
                    {
                        p.Opacity += 0.1f;
                    }

                    foreach (Projectile proj in EntityManager.activeFriendlyProjectiles)
                    {
                        if (proj.Label == EntityLabels.ShotgunAttractor)
                        {
                            if (p.IsCollidingWith(proj))
                            {
                                p.InstantlyDie();
                                proj.ExtraData[0] += 1f; // count how many pellets hit the attractor
                            }

                            p.Velocity = projectileSpeed * Utilities.SafeNormalise(proj.Position - p.Position);

                            break;
                        }                       
                    }
                }));

                p.AddTrail(14);
            }
        }

        public override void SecondaryFire()
        {
            // remove existing attractors

            foreach (Projectile proj in EntityManager.activeFriendlyProjectiles)
            {
                if (proj.Label == EntityLabels.ShotgunAttractor)
                {
                    proj.Die();
                }
            }

            float projectileSpeed = 5f;
            float damage = 0.1f;

            Projectile p = SpawnProjectile(Owner.Position, projectileSpeed * Utilities.SafeNormalise(MousePositionWithCamera() - Owner.Position), damage, 1, "box", Vector2.One, Owner, false, Color.LightGoldenrodYellow, true, true);

            p.Label = EntityLabels.ShotgunAttractor;

            p.SetExtraAI(new Action(() =>
            {
                if (p.ExtraData[0] == 5f)
                {
                    p.Die();
                }
            }));
        }
    }
}
