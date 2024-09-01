﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using bullethellwhatever.BaseClasses;
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

        public float DamageReduction;

        public bool CanDie;

        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, string texture, Vector2 size, float MaxHealth, int pierceToTake, Color colour, bool shouldRemoveOnEdgeTouch, bool isHarmful)
        {
            Updates = 1; //default

            Depth = 0;

            DamageReduction = 0;

            IsInvincible = false;

            DrawAfterimages = false;

            TargetableByHoming = true;

            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = AssetRegistry.GetTexture2D(texture);
            Colour = colour;

            Size = size;

            Health = MaxHealth;
            MaxHP = MaxHealth;
            PierceToTake = pierceToTake;

            DealDamage = false;
            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            Opacity = 1f;
            InitialOpacity = Opacity;

            ExtraData = new float[4];

            IsHarmful = isHarmful;

            PrepareNPC();
        }

        public override void AI()
        {
            if (ExtraAI is not null)
            {
                ExtraAI();
            }
        }

        public override void Update()
        {
            if (IFrames > 0)
            {
                IFrames--;
            }

            Position = Position + Velocity;

            AITimer++;

            base.Update();

            Rotation = Rotation + RotationalVelocity;
        }
        public virtual Collision CollisionWithEntity(Entity entity)
        {
            return Hitbox.Intersects(entity.Hitbox);
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

                if (!IsHarmful) // If you want the player able to spawn NPCs, make a friendlyNPCs list and check through that if the projectile is harmful.
                {
                    foreach (NPC npc in EntityManager.activeNPCs) // if not harmful (player allegiance), search for entities to attack
                    {
                        Collision c = CollisionWithEntity(npc);

                        if (c.Collided && npc.IFrames == 0)
                        {
                            DealDamageTo(npc, c);
                        }
                    }
                }

                if (IsHarmful)
                {
                    if (CollisionWithEntity(player).Collided && player.IFrames == 0)
                    {
                        DamagePlayer();
                    }
                }
            }
        }

        public void SetDR(float dr)
        {
            DamageReduction = MathHelper.Clamp(dr, 0f, 1f);
        }
        public override void Delete()
        {
            DeleteNextFrame = true;
        }
        public virtual void TakeDamage(Collision collision, Projectile projectile)
        {
            IFrames = MaxIFrames;

            Health = Health - ((1 - DamageReduction) * projectile.Damage);

            projectile.OnHitEffect(collision.CollisionPoint);

            projectile.HandlePierce(PierceToTake);
        }
        public virtual void DrawHPBar(SpriteBatch spriteBatch)
        {
            if (Participating)
            {
                //base.Draw(spriteBatch);

                if (this is not Boss)
                {
                    UI.DrawHealthBar(spriteBatch, this, Position + new Vector2(0, 10f * DepthFactor()), 50f * DepthFactor(), 10f * DepthFactor());
                }

                else UI.DrawHealthBar(spriteBatch, this, new Vector2(GameWidth / 2, GameHeight / 20 * 19), 900f, 30f); // boss bar
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

            Depth = 0;

            DrawAfterimages = false;

            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Colour = colour;

            TargetableByHoming = true;

            Size = size;
            Health = MaxHealth;
            MaxHP = MaxHealth;
            PierceToTake = pierceToTake;

            DealDamage = false;
            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            Opacity = 1f;


            IsHarmful = isHarmful;
        }
        public virtual void PrepareNPC()
        {
            PrepareNPCButDontAddToListYet();

            NPCsToAddNextFrame.Add(this);
        }

        public virtual void PrepareNPCButDontAddToListYet()
        {
            Hitbox = new RotatedRectangle(Rotation, Texture.Width * GetSize().X, Texture.Height * GetSize().Y, Position, this);
            Participating = true;
            SetHitbox();
        }
    }
}