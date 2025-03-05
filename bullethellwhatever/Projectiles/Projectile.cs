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
        public Projectile(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, Vector2 size, Entity owner, bool harmfulToPlayer, bool harmfulToEnemy, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            CreateProjectile(position, velocity, damage, pierce, texture, size, owner, harmfulToPlayer, harmfulToEnemy, colour, shouldRemoveOnEdgeTouch, removeOnHit);
        }

        public void CreateProjectile(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, Vector2 size, Entity owner, bool harmfulToPlayer, bool harmfulToEnemy, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
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

            Scale = size;
            Owner = owner;

            HarmfulToPlayer = harmfulToPlayer;
            HarmfulToEnemy = harmfulToEnemy;

            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            RemoveOnHit = removeOnHit;

            ContactDamage = harmfulToPlayer;

            if (Updates == 0) // if we havent already set updates
            {
                Updates = 1;
            }

            Opacity = 1f;
            InitialOpacity = Opacity;
            Dying = false;

            DyingTimer = 0;

            Participating = true;

            if (MercyTimeBeforeRemoval == 0)
            {
                MercyTimeBeforeRemoval = 180;
            }

            ExtraData = new float[4];

            Label = EntityLabels.None;

            // if it doesnt damage the player, mark it friendly. this can be manually changed.
            Friendly = !harmfulToPlayer;

            SetHitbox();
        }
        public void Prepare(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, Vector2 size, Entity owner, bool harmfulToPlayer, bool harmfulToEnemy, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            CreateProjectile(position, velocity, damage, pierce, texture, size, owner, harmfulToPlayer, harmfulToEnemy, colour, shouldRemoveOnEdgeTouch, removeOnHit);

            Initialise();

            EntityManager.projectilesToAdd.Add(this);
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

        /// <summary>
        /// Updates the position of the projectile based on its velocity. This can be subdivided so that multiple collision checks can be done in one frame to prevent skipping.
        /// </summary>
        /// <param name="progress"></param>
        public virtual void UpdatePosition()
        {
            Position = Position + Velocity;
        }

        public void UpdateAndCheckHits()
        {
            PerformAdjustments();
            UpdateHitbox();
            CheckForHits(); 
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
                if (HarmfulToEnemy)
                {
                    CheckForAndHitNPCs();
                }

                if (HarmfulToPlayer && Damage != 0f)
                {
                    if (IsCollidingWith(player) && player.IFrames == 0 && !Dying)
                    {
                        DealDamage(player);
                        OnHitEffect(player.Position); // projectile on hit effect happens at player's position not projectile's position
                    }
                }
            }
        }

        public virtual void CheckForAndHitNPCs()
        {
            foreach (NPC npc in EntityManager.activeNPCs)
            {
                if (IsCollidingWith(npc))
                {
                    DealDamage(npc);
                    OnHitEffect(npc.Position);
                }
            }
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

        public override void DealDamage(NPC npc)
        {
            if (Damage != 0)
            {
                npc.TakeDamage(Damage);
            }

            HandlePierce(npc.PierceToTake);
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
    }
}
