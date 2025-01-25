using bullethellwhatever.AssetManagement;
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
        public Texture2D IconHUD;
        public int AITimer;
        public Weapon(Player player, string iconTexture)
        {
            Owner = player;
            IconHUD = AssetRegistry.GetTexture2D(iconTexture);
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

            if (LeftClickReleased())
            {
                LeftClickReleasedBehaviour();
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

            HandleShooting();
        }

        public virtual void HandleShooting()
        {
            HandlePrimary();
            HandleSecondary();
        }

        public virtual void HandlePrimary()
        {
            if (PrimaryFiring())
            {
                PrimaryFire();
                PrimaryFireCoolDown = PrimaryFireCoolDownDuration;
            }
        }

        public virtual void HandleSecondary()
        {
            if (SecondaryFiring())
            {
                ShootSecondary();
            }
        }

        public void ShootSecondary()
        {
            SecondaryFire();
            SecondaryFireCoolDown = SecondaryFireCoolDownDuration;
        }

        public virtual void LeftClickHeldBehaviour()
        {

        }

        public virtual void LeftClickReleasedBehaviour()
        {

        }

        public virtual bool PrimaryFireReady()
        {
            return PrimaryFireCoolDown == 0;
        }
        public virtual bool SecondaryFireReady()
        {
            return SecondaryFireCoolDown == 0;
        }
        public virtual bool PrimaryFiring()
        {
            return PrimaryFireReady() && IsLeftClickDown();
        }
        public virtual bool SecondaryFiring()
        {
            return SecondaryFireReady() && IsRightClickDown();
        }

        public virtual void Draw(SpriteBatch s)
        {

        }
        public abstract void PrimaryFire();
        public abstract void SecondaryFire();
        public abstract void AI();
    }
}
