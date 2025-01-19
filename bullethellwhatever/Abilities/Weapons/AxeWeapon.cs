using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses.Entities;

namespace bullethellwhatever.Abilities.Weapons
{
    public class AxeWeapon : Weapon
    {
        public AxeWeapon(Player player, string iconTexture) : base(player, iconTexture)
        {
            
        }
        public float WeaponRotation;
        public float SwingAngle => PI / 3;
        public float SwingPeriod => 120;
        public List<Entity> HitEnemies; // hold enemies hit to prevent multihits
        public override void WeaponInitialise()
        {
            
        }
        public override bool CanSwitchWeapon()
        {
            return true;
        }
        public override void AI()
        {
            float angleToMouse = Utilities.VectorToAngle(MousePositionWithCamera() - Owner.Position);
            WeaponRotation = angleToMouse + SwingAngle * Sin(AITimer / PI / SwingPeriod);
        }

        public bool Swinging()
        {
            return IsLeftClickDown();
        }

        public override void PrimaryFire()
        {

        }

        public override void SecondaryFire()
        {

        }

        public override void Draw(SpriteBatch s)
        {
            
        }
    }
}
