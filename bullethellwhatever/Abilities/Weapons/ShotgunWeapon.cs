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
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.DrawCode;

namespace bullethellwhatever.Abilities.Weapons
{
    public class ShotgunWeapon : Weapon
    {
        public bool DeployingReflector;
        public Vector2 ReflectorTarget;
        public ShotgunWeapon(Player player, string iconTexture) : base(player, iconTexture)
        {

        }
        public override void WeaponInitialise()
        {
            PrimaryFireCoolDownDuration = 60;
            SecondaryFireCoolDownDuration = 5;
            DeployingReflector = false;
            ReflectorTarget = Vector2.Zero;
        }

        public override void AI()
        {
            if (DeployingReflector)
            {
                ReflectorTarget = MousePositionWithCamera();
            }
        }

        public override void PrimaryFire()
        {
            float spreadAngle = PI / 3;
            int projectiles = 1; // keep this an odd number
            float angleBetween = spreadAngle / projectiles;
            float damage = 1.3f;
            float projectileSpeed = 5;

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
            if (!DeployingReflector)
            {
                DeployingReflector = true;
            }
            else
            {
                DeployingReflector = false;

                Projectile p = SpawnProjectile<Projectile>(ReflectorTarget, Vector2.Zero, 0f, 1, "box", Vector2.One, Owner, false, Color.LightGoldenrodYellow, false, false);

                p.Label = EntityLabels.SharpShotReflector;

                p.SetExtraDraw(new Action(() =>
                {
                    Utilities.drawTextInDrawMethod(p.ExtraData[0].ToString(), p.Position + new Vector2(0f, 20f), _spriteBatch, font, Color.White);
                }));
            }
        }

        public override void Draw(SpriteBatch s)
        {
            if (DeployingReflector)
            {
                Drawing.BetterDraw("TargetReticle", ReflectorTarget, null, Color.White, 0f, Vector2.One, SpriteEffects.None, 0f);
            }
        }
    }
}
