using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Abilities.Weapons
{
    public abstract class Weapon
    {
        public float Damage;
        public int PrimaryFireCoolDownDuration;
        public int SecondaryFireCoolDownDuration;
        public int PrimaryFireCoolDown;
        public int SecondaryFireCoolDown;
        public Player Owner;
        public int AITimer;
        public Weapon(Player player)
        {
            Owner = player;
        }
        public abstract void WeaponInitialise();
        public void Initialise()
        {
            PrimaryFireCoolDown = PrimaryFireCoolDownDuration;
            SecondaryFireCoolDown = SecondaryFireCoolDownDuration;
            AITimer = 0;
            // the above allocations can be changed in weaponinit
            WeaponInitialise();
        }
        public virtual bool CanSwitchWeapon()
        {
            return true;
        }

        public void BaseUpdate()
        {
            AITimer++;

            HandleCooldowns();

            if (IsLeftClickDown() && WasMouseDownLastFrame)
            {
                LeftClickHeldBehaviour();
            }
        }

        /// <summary>
        /// Handles the basic primary fire and secondary fire cooldowns. Override to disable these.
        /// </summary>
        public virtual void HandleCooldowns()
        {
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
        public virtual void LeftClickHeldBehaviour()
        {

        }

        public virtual bool CanPrimaryFire()
        {
            return PrimaryFireCoolDown == 0 && IsLeftClickDown();
        }
        public virtual bool CanSecondaryFire()
        {
            return SecondaryFireCoolDown == 0 && IsRightClickDown();
        }

        public virtual void Draw(SpriteBatch s)
        {

        }
        public abstract void PrimaryFire();
        public abstract void SecondaryFire();
        public abstract void AI();
    }
}
