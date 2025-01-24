using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using bullethellwhatever.MainFiles;
using bullethellwhatever.DrawCode;
using System.Runtime.CompilerServices;
using System;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.Projectiles.TelegraphLines;
using SharpDX.MediaFoundation;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.NPCs;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses.Entities;

namespace bullethellwhatever.Projectiles
{
    public class Projectile : Entity
    {
        public Entity Owner;
        public bool RemoveOnHit;

        public int Pierce;
        public int TimeOutsidePlayArea;
        public int PierceRemaining;

        // for projectile fade out

        public bool Dying;
        public int DyingTimer;
        public float StartingOpacity;

        public Action OnHit;
        public Action OnEdgeTouch;

        public int MercyTimeBeforeRemoval;

        public bool IsEffect;

        public Projectile()
        {

        }
        public Projectile(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            CreateProjectile(position, velocity, damage, pierce, texture, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);
        }

        public void CreateProjectile(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            Texture = AssetRegistry.GetTexture2D(texture);

            Position = position;
            Pierce = pierce;
            TimeOutsidePlayArea = 0;
            PierceRemaining = Pierce;
            Velocity = velocity;
            Damage = damage;
            Colour = colour;
            DeleteNextFrame = false;
            Size = size;
            Owner = owner;
            IsHarmful = isHarmful;
            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            RemoveOnHit = removeOnHit;

            DealDamage = isHarmful;

            if (Updates == 0) // if we havent already set updates
            {
                Updates = 1;
            }

            Opacity = 1f;
            InitialOpacity = Opacity;
            Dying = false;

            DyingTimer = 0;

            Hitbox = new RotatedRectangle(Rotation, Texture.Width * GetSize().X, Texture.Height * GetSize().Y, Position, this);

            Participating = true;

            if (MercyTimeBeforeRemoval == 0)
            {
                MercyTimeBeforeRemoval = 180;
            }

            ExtraData = new float[4];

            Label = EntityLabels.None;

            SetHitbox();
        }
        public void Prepare(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            CreateProjectile(position, velocity, damage, pierce, texture, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);

            Initialise();

            if (IsHarmful)
                EntityManager.enemyProjectilesToAddNextFrame.Add(this);
            else EntityManager.friendlyProjectilesToAddNextFrame.Add(this);
        }

        public virtual void Initialise()
        {

        }

        public override void AI()
        {

        }

        public void HandleBounces()
        {
            if (TouchingLeft())
            {
                if (Velocity.X < 0)
                    Velocity.X = Velocity.X * -1;
            }

            if (TouchingRight())
            {
                if (Velocity.X > 0)
                    Velocity.X = Velocity.X * -1;
            }

            if (TouchingTop())
            {
                if (Velocity.Y < 0)
                    Velocity.Y = Velocity.Y * -1f;

            }

            if (TouchingBottom())
            {
                if (Velocity.Y > 0)
                    Velocity.Y = Velocity.Y * -1f;
            }
        }

        public override void PostUpdate()
        {
            // run any extra ais the projectile may have before continuing on

            if (ExtraAI is not null)
            {
                ExtraAI();
            }

            // move to update to make overriding ai easier?
            Position = Position + Velocity;

            AITimer++;

            base.PostUpdate();

            if (TouchingAnEdge())
            {
                TimeOutsidePlayArea++;
            }

            else TimeOutsidePlayArea = 0;

            if (TimeOutsidePlayArea == 1) // just hit edge
            {
                EdgeTouchEffect();
            }

            if (Dying)
            {
                float fadeOutTime = 10f * Updates;

                Opacity = MathHelper.Lerp(1f, 0f, DyingTimer / fadeOutTime);

                DyingTimer++;

                if (DyingTimer == fadeOutTime)
                {
                    base.Die();
                }
            }
        }

        public override void Die()
        {
            if (Owner is not Player) // avoid visual vomit by not doing this if player owned
            {
                Dying = true;
                StartingOpacity = Opacity;
            }

            else
            {
                InstantlyDie();
            }
        }

        public virtual void InstantlyDie()
        {
            DeleteNextFrame = true;

            if (OnDeath is not null)
            {
                OnDeath();
            }
        }

        public override void Delete()
        {
            DeleteNextFrame = true;
        }

        public virtual void CheckForHits()
        {
            if (Participating && !Dying)
            {
                if (!IsHarmful) // If you want the player able to spawn NPCs, make a friendlyNPCs list and check through that if the projectile is harmful.
                {
                    CheckForAndHitNPCs();
                }

                if (IsHarmful && DealDamage)
                {
                    if (CollisionWithEntity(player).Collided && player.IFrames == 0 && !Dying)
                    {
                        DamagePlayer();
                        OnHitEffect(player.Position); // projectile on hit effect happens at player's position not projectile's position
                    }
                }
            }
        }

        public virtual void CheckForAndHitNPCs()
        {
            foreach (NPC npc in EntityManager.activeNPCs)
            {
                Collision collision = CollisionWithEntity(npc);

                if (collision.Collided && npc.IFrames == 0 && !npc.IsInvincible && !Dying)
                {
                    DealDamageTo(npc, collision);
                    OnHitToNPC(npc);
                }
            }
        }
        public virtual void OnHitToNPC(NPC hitNPC)
        {

        }

        public override void DealDamageTo(NPC npc, Collision collision)
        {
            npc.TakeDamage(collision, this);
            HandlePierce(npc.PierceToTake);
        }

        public void SetOnHit(Action action)
        {
            OnHit = action;
        }

        public virtual void OnHitEffect(Vector2 position)
        {
            if (OnHit is not null)
                OnHit();
        }

        public override void DamagePlayer()
        {
            if (Damage != 0)
            {
                player.IFrames = 20f;

                player.Health = player.Health - Damage;

                Drawing.ScreenShake(3, 10);
            }

            HandlePierce(1);
        }

        public void SetEdgeTouchEffect(Action action)
        {
            OnEdgeTouch = action;
        }
        public virtual void EdgeTouchEffect()
        {
            if (OnEdgeTouch is not null)
            {
                OnEdgeTouch();
            }
        }
        public virtual void HomeAtTarget(Vector2 targetPos, float homingStrength)
        {
            float currentVelocityMagnitude = Velocity.Length();
            Vector2 toTarget = Utilities.SafeNormalise(targetPos - Position);

            Velocity = currentVelocityMagnitude * Utilities.SafeNormalise(Vector2.Lerp(Utilities.SafeNormalise(Velocity), toTarget, homingStrength));
        }
        public void HandlePierce(int pierceToTake)
        {
            if (PierceRemaining > 0)
            {
                PierceRemaining = PierceRemaining - pierceToTake;
            }

            if (RemoveOnHit && PierceRemaining <= 0 && !Dying) // dont reset death fade out if already dying
                Die();
        }

        public virtual Collision CollisionWithEntity(Entity entity)
        {
            Collision collision = entity.Hitbox.Intersects(Hitbox);

            if (collision.Collided)
                return collision;
            else return new Collision(Vector2.Zero, false);
        }

        public virtual bool CollidedWith(Entity entity)
        {
            return CollisionWithEntity(entity).Collided;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
