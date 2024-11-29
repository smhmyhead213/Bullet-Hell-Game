using bullethellwhatever.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Abilities.Weapons
{
    public abstract class Weapon
    {
        public int AITimer;
        public float Damage;
        public int PrimaryFireCoolDownDuration;
        public int SecondaryFireCoolDownDuration;
        public int PrimaryFireCoolDown;
        public int SecondaryFireCoolDown;
        public Player Owner;

        public Weapon(Player player)
        {
            Owner = player;
        }
        public abstract void WeaponInitialise();
        public void Initialise()
        {
            AITimer = 0;

            PrimaryFireCoolDown = 0;
            SecondaryFireCoolDown = 0;

            // the above allocations can be changed in weaponinit
            WeaponInitialise();
        }
        public abstract bool CanSwitchWeapon();
        public void BaseUpdate()
        {
            AITimer++;

            if (PrimaryFireCoolDown > 0)
            {
                PrimaryFireCoolDown--;
            }

            if (SecondaryFireCoolDown > 0)
            {
                SecondaryFireCoolDown--;
            }

            if (CanPrimaryFire())
            {
                PrimaryFire();
                PrimaryFireCoolDown = PrimaryFireCoolDownDuration;
            }

            if (CanSecondaryFire())
            {
                SecondaryFire();
                SecondaryFireCoolDown = SecondaryFireCoolDownDuration;
            }
        }

        public virtual bool CanPrimaryFire()
        {
            return PrimaryFireCoolDown == 0 && IsLeftClickDown();
        }
        public virtual bool CanSecondaryFire()
        {
            return SecondaryFireCoolDown == 0 && IsRightClickDown();
        }

        public abstract void PrimaryFire();
        public abstract void SecondaryFire();
        public abstract void AI();
    }
}
