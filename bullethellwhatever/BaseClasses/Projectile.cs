using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using bullethellwhatever.MainFiles;
using bullethellwhatever.DrawCode;
using System.Runtime.CompilerServices;
using System;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.Player;
using bullethellwhatever.BaseClasses.Hitboxes;

namespace bullethellwhatever.BaseClasses
{
    public class Projectile : Entity
    {
        public float Acceleration;

        public Entity Owner;
        public bool RemoveOnHit;

        public int Pierce;
        public int TimeOutsidePlayArea;
        public int PierceRemaining;

        // for projectile fade out

        public bool Dying;
        public int DyingTimer;
        public float StartingOpacity;

        public Func<float, float>? DepthFunction;
        public Func<float, Vector2> VelocityFunction;

        public Action OnHit;
        public Action OnEdgeTouch;

        public int MercyTimeBeforeRemoval;

        public bool IsEffect;
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            Texture = Assets[texture];

            PrepareProjectile(position, velocity, damage, pierce, acceleration, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);
        }

        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, int pierce, Texture2D texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            Texture = texture;

            PrepareProjectile(position, velocity, damage, pierce, acceleration, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);
        }
        //and drawing
        public void HandleBounces()
        {
            if (touchingLeft(this))
            {
                if (Velocity.X < 0)
                    Velocity.X = Velocity.X * -1;
            }

            if (touchingRight(this))
            {
                if (Velocity.X > 0)
                    Velocity.X = Velocity.X * -1;
            }

            if (touchingTop(this))
            {
                if (Velocity.Y < 0)
                    Velocity.Y = Velocity.Y * -1f;

            }

            if (touchingBottom(this))
            {
                if (Velocity.Y > 0)
                    Velocity.Y = Velocity.Y * -1f;
            }
        }
        public virtual void PrepareProjectile(Vector2 position, Vector2 velocity, float damage, int pierce, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            Depth = 0;

            if (!DrawAfterimages) // dont set to false by default if already set to true
            {
                DrawAfterimages = false;
            }

            Position = position;
            Pierce = pierce;
            TimeOutsidePlayArea = 0;
            PierceRemaining = Pierce;
            Velocity = velocity;
            Damage = damage;
            Acceleration = acceleration;
            Colour = colour;
            DeleteNextFrame = false;
            Size = size;
            Owner = owner;
            IsHarmful = isHarmful;
            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            RemoveOnHit = removeOnHit;

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
                MercyTimeBeforeRemoval = 60;
            }

            SetHitbox();

            if (IsHarmful)
                enemyProjectilesToAddNextFrame.Add(this);
            else friendlyProjectilesToAddNextFrame.Add(this);
        }
        public override void Update()
        {
            AITimer++;

            if (touchingAnEdge(this))
            {
                TimeOutsidePlayArea++;
            }

            else TimeOutsidePlayArea = 0;

            if (TimeOutsidePlayArea == 1) // just hit edge
            {
                EdgeTouchEffect();
            }

            if (VelocityFunction is null)
            {
                if (Acceleration != 0)
                {
                    Velocity = Velocity * Acceleration;
                }
            }
            else
            {
                Velocity = VelocityFunction(AITimer);
            }

            if (DepthFunction is not null)
            {
                SetDepth(DepthFunction(AITimer));
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

            base.Update();
        }
        public override void AI()
        {
            if (ExtraAI is not null)
            {
                ExtraAI();
            }

            Position = Position + Velocity * ScaleFactor();
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
        
        public virtual void CheckForHits()
        {
            if (Participating && !Dying)
            {
                if (!IsHarmful) // If you want the player able to spawn NPCs, make a friendlyNPCs list and check through that if the projectile is harmful.
                {
                    CheckForAndHitNPCs();
                }

                if (IsHarmful)
                {
                    if (CollisionWithEntity(player).Collided && player.IFrames == 0 && !Dying)
                    {
                        DamagePlayer();
                    }
                }
            }
        }

        public virtual void CheckForAndHitNPCs()
        {
            foreach (NPC npc in activeNPCs)
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

        public virtual void OnHitEffect(Vector2 position)
        {
            if (OnHit is not null)
                OnHit();
        }

        public override void DamagePlayer()
        {
            player.IFrames = 20f;

            player.Health = player.Health - Damage;

            Drawing.ScreenShake(3, 10);

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
    }
}
