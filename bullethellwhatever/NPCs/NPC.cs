using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using bullethellwhatever.MainFiles;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.Base;
using System.Collections.Generic;
using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.DrawCode;
using System.Reflection.Metadata.Ecma335;
using bullethellwhatever.DrawCode.UI;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.Projectiles;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.Bosses;
using bullethellwhatever.BaseClasses.Entities;
using SharpDX.XAudio2;
using log4net;

namespace bullethellwhatever.NPCs
{
    public class NPC : Entity
    {
        public float IFrames;

        public int MaxIFrames;

        public bool IsInvincible;

        public bool TargetableByHoming;

        public bool BlockDeathrays;

        public int PierceToTake;

        public float Health;
        public float DamageReduction;

        public bool CanDie;

        public BossAttack CurrentAttack;
        public BossAttack PreviousAttack;

        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, string texture, Vector2 size, float MaxHealth, int pierceToTake, Color colour, bool shouldRemoveOnEdgeTouch, bool harmfulToPlayer, bool harmfulToEnemy)
        {
            Updates = 1; //default

            DamageReduction = 0;

            IsInvincible = false;

            TargetableByHoming = true;

            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = AssetRegistry.GetTexture2D(texture);
            Colour = colour;

            Scale = size;

            Health = MaxHealth;
            MaxHP = MaxHealth;
            PierceToTake = pierceToTake;

            ContactDamage = false;
            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            Opacity = 1f;
            InitialOpacity = Opacity;

            ExtraData = new float[4];

            HarmfulToPlayer = harmfulToPlayer;
            HarmfulToEnemy = harmfulToEnemy;

            PrepareNPC();
        }

        /// <summary>
        /// Returns a bool corresponding to whether or not the camera should try to zoom to accomodate for this NPC.
        /// </summary>
        /// <returns></returns>
        public virtual bool ConsideredForCameraZoom()
        {
            return true;
        }

        public override void AI()
        {
            if (ExtraAI is not null)
            {
                ExtraAI();
            }
        }

        public override void PostUpdate()
        {
            if (IFrames > 0)
            {
                IFrames--;
            }

            Position = Position + Velocity;

            UpdateHitbox();
            CheckForHits();

            AITimer++;

            base.PostUpdate();

            Rotation = Rotation + RotationalVelocity;

            Rotation = Utilities.BringAngleIntoRange(Rotation);
        }

        public float HPRatio()
        {
            return Health / MaxHP;
        }

        public virtual void CheckForHits() // all passive checks go here as well
        {
            if (Participating)
            {
                if (Health < 0 && CanDie)
                {
                    Die();

                    foreach (Projectile projectile in EntityManager.activeProjectiles)
                    {
                        if (projectile.Owner == this)
                        {
                            projectile.Die(); //when an npc dies, kill all of its projectiles
                        }
                    }
                }

                if (HarmfulToEnemy) // If you want the player able to spawn NPCs, make a friendlyNPCs list and check through that if the projectile is harmful.
                {
                    foreach (NPC npc in EntityManager.activeNPCs) // if not harmful (player allegiance), search for entities to attack
                    {
                        //Collision c = CollisionWithEntity(npc);

                        //if (c.Collided && npc.IFrames == 0)
                        //{
                            
                        //}
                    }
                }

                if (HarmfulToPlayer)
                {
                    if (IsCollidingWith(player))
                    {
                        DealDamage(player);
                    }
                }
            }
        }
        public virtual void Heal(float amount)
        {
            Health = Health + amount;

            if (Health > MaxHP)
            {
                Health = MaxHP;
            }
        }

        public virtual void TakeDamage(Projectile projectile)
        {
            TakeDamage(projectile.Damage);
        }

        public virtual void TakeDamage(NPC npc)
        {
            TakeDamage(npc.Damage); 
        }

        public virtual void TakeDamage(float damage)
        {
            if (IFrames == 0)
            {
                IFrames = MaxIFrames;

                DeductHealth(damage);
            }
        }
        public virtual void DeductHealth(float damage)
        {
            Health = Health - ((1 - DamageReduction) * damage);
        }
        public void SetDR(float dr)
        {
            DamageReduction = MathHelper.Clamp(dr, 0f, 1f);
        }
        public override void Delete()
        {
            DeleteNextFrame = true;
        }

        public virtual void DrawHPBar(SpriteBatch spriteBatch)
        {
            if (Participating)
            {
                //base.Draw(spriteBatch);

                //if (this is not Boss)
                //{
                //    UI.DrawHealthBar(spriteBatch, this, Position + new Vector2(0, 10f * DepthFactor()), 50f * DepthFactor(), 10f * DepthFactor());
                //}

                //else  // boss bar
            }
        }

        public virtual void SetBlockDeathrays(bool block)
        {
            BlockDeathrays = block;
        }

        public virtual bool IsHittable()
        {
            return !IsInvincible && Participating && IFrames == 0;
        }
        public virtual void CreateNPC(Vector2 position, Vector2 velocity, float damage, Texture2D texture, Vector2 size, float MaxHealth, int pierceToTake, Color colour, bool shouldRemoveOnEdgeTouch, bool isHarmful)
        {
            Updates = 1;

            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Colour = colour;

            TargetableByHoming = true;

            Scale = size;
            Health = MaxHealth;
            MaxHP = MaxHealth;
            PierceToTake = pierceToTake;

            ContactDamage = false;
            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            Opacity = 1f;


            HarmfulToPlayer = isHarmful;
        }
        public virtual void PrepareNPC()
        {
            PrepareNPCButDontAddToListYet();

            NPCsToAddNextFrame.Add(this);
        }

        public virtual void PrepareNPCButDontAddToListYet()
        {
            Participating = true;
            SetHitbox();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            DrawHitbox();
        }
    }
}
