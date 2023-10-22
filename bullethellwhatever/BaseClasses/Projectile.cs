using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using bullethellwhatever.MainFiles;
using bullethellwhatever.DrawCode;
using System.Runtime.CompilerServices;
using System;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.Player;

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
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            Depth = 0;

            DrawAfterimages = false;
            Position = position;
            Pierce = pierce;
            TimeOutsidePlayArea = 0;
            PierceRemaining = Pierce;
            Velocity = velocity;
            Damage = damage;
            Texture = Assets[texture];
            Acceleration = acceleration;
            Colour = colour;
            DeleteNextFrame = false;
            Size = size;
            Owner = owner;
            IsHarmful = isHarmful;
            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            RemoveOnHit = removeOnHit;
            Updates = 1;

            Opacity = 1f;

            Dying = false;

            DyingTimer = 0;

            //float sizeScalar = ScreenWidth * 1f / IdealScreenWidth * 1f; //adjust for screen size horizontally

            //Size = new Vector2(Size.X / IdealScreenWidth * ScreenWidth, Size.Y / IdealScreenHeight * ScreenHeight) * sizeScalar;

            PrepareProjectile();
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
        public virtual void PrepareProjectile()
        {
            Hitbox = new RotatedRectangle(Rotation, Texture.Width * GetSize().X, Texture.Height * GetSize().Y, Position, this);

            SetHitbox();

            if (IsHarmful)
                enemyProjectilesToAddNextFrame.Add(this);
            else friendlyProjectilesToAddNextFrame.Add(this);
        }
        public virtual void Update()
        {
            AITimer++;

            if (afterimagesPositions is not null)
            {
                Utilities.moveVectorArrayElementsUpAndAddToStart(ref afterimagesPositions, Position);
            }

            if (touchingAnEdge(this))
            {
                TimeOutsidePlayArea++;
            }

            else TimeOutsidePlayArea = 0;

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
                float fadeOutTime = 10f;

                Opacity = MathHelper.Lerp(1f, 0f, DyingTimer / fadeOutTime);

                DyingTimer++;

                if (DyingTimer == fadeOutTime)
                {
                    base.Die();
                }
            }
        }

        public override void AI()
        {
            Position = Position + Velocity;
        }

        public override void Die()
        {
            if (!(Owner is Player)) // avoid visual vomit by not doing this if player owned
            {
                Dying = true;
                StartingOpacity = Opacity;
            }

            else DeleteNextFrame = true;
        }
        public virtual void CheckForHits()
        {
            if (!IsHarmful) // If you want the player able to spawn NPCs, make a friendlyNPCs list and check through that if the projectile is harmful.
            {
                foreach (NPC npc in activeNPCs)
                {
                    if (IsCollidingWithEntity(npc) && npc.IFrames == 0 && !npc.IsInvincible && !Dying)
                    {
                        npc.IFrames = npc.MaxIFrames;

                        npc.Health = npc.Health - ((1 - npc.DamageReduction) * Damage);

                        HandlePierce(npc.PierceToTake);
                    }
                }
            }

            if (IsHarmful)
            {
                if (IsCollidingWithEntity(player) && player.IFrames == 0 && !Dying)
                {
                    player.IFrames = 20f;

                    player.Health = player.Health - Damage;

                    Drawing.ScreenShake(3, 10);

                    HandlePierce(1);
                }
            }
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

        public virtual bool IsCollidingWithEntity(Entity entity)
        {
            if (entity.Hitbox.Intersects(Hitbox))
                return true;
            else return false;
        }
    }
}
