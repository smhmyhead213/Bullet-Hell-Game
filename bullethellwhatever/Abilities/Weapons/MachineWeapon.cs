using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using bullethellwhatever.MainFiles;
using bullethellwhatever.NPCs;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Abilities.Weapons
{
    public class MachineWeapon : Weapon
    {
        public MachineWeapon(Player player) : base(player)
        {
            
        }
        public override void WeaponInitialise()
        {
            PrimaryFireCoolDownDuration = 3;
        }
        public override bool CanSwitchWeapon()
        {
            return true;
        }

        public override void AI()
        {
            
        }

        public override void PrimaryFire()
        {
            Random rnd = new Random();

            Projectile playerProjectile = SpawnProjectile(Owner.Position, 20f * Utilities.RotateVectorClockwise(Utilities.Normalise(MousePositionWithCamera() - Owner.Position), Utilities.ToRadians(rnd.Next(-10, 10))),
                0.4f, 1, "MachineGunProjectile", Vector2.One, Owner, false, Color.LightBlue, true, true);

            playerProjectile.Rotation = Utilities.VectorToAngle(Utilities.RotateVectorClockwise(Utilities.Normalise(MousePositionWithCamera() - Owner.Position), Utilities.ToRadians(rnd.Next(-10, 10))));
        }
        public override void SecondaryFire()
        {

        }
    }
}
