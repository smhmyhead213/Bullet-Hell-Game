using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.DrawCode;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.NPCs;
using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses.Entities;

namespace bullethellwhatever.Abilities.Weapons
{
    public abstract class Weapon
    {
        public float Damage;
        public int PrimaryFireCoolDownDuration;
        public int SecondaryFireCoolDownDuration;
        public int PrimaryFireCoolDown;
        public int SecondaryFireCoolDown;

        public bool PrimaryFireHoldable;
        public bool SecondaryFireHoldable;

        public Player Owner;
        public Texture2D IconHUD;
        public int AITimer;

        public List<Circle> Hitbox;

        public List<Entity> HitEnemies; // hold enemies hit to prevent multihits, not used by all weapons, managed in weapons update manually

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
            PrimaryFireHoldable = true;
            SecondaryFireHoldable = true;
            Hitbox = new List<Circle>();
            HitEnemies = new List<Entity>();

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

            if (RightClickReleased())
            {
                RightClickReleasedBehaviour();
            }

            UpdateHitbox();
            CheckAndHitEnemies();
        }

        public void CheckAndHitEnemies()
        {
            foreach (NPC npc in EntityManager.activeNPCs)
            {
                if (!npc.IsInvincible && npc.Participating && !HitEnemies.Contains(npc))
                {
                    foreach (Circle circle in Hitbox)
                    {
                        bool hitFound = false;

                        foreach (Circle other in npc.Hitbox)
                        {                          
                            if (circle.Intersects(other))
                            {
                                OnHit(npc);

                                hitFound = true;
                                break;
                            }
                        }

                        if (hitFound)
                        {
                            break;
                        }
                    }
                }
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

        public virtual void RightClickReleasedBehaviour()
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

        public virtual void UpdateHitbox()
        {

        }

        public void DrawHitbox()
        {
            Drawing.DrawCircles(Hitbox, Color.Red, 0.5f);
        }

        public void DealDamage(NPC npc, float damage)
        {
            npc.TakeDamage(damage);
        }

        /// <summary>
        /// Code to execute when the weapon itself hits an enemy. For projectiles fired, put the logic in the projectile's on hit instead.
        /// </summary>
        /// <param name="npc"></param>
        public virtual void OnHit(NPC npc)
        {

        }

        public abstract void PrimaryFire();

        public abstract void SecondaryFire();
        public abstract void AI();
    }
}
