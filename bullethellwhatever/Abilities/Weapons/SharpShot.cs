using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles;
using bullethellwhatever.NPCs;
using bullethellwhatever.BaseClasses.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SharpDX.MediaFoundation;
using System.Diagnostics;
using bullethellwhatever.DrawCode.Particles;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.Abilities.Weapons
{
    public class SharpShot : Projectile
    {
        public List<Projectile> ReflectorsHit;
        public int Reflections;
        public bool Bounced;
        public int Bounces;
        public override void Initialise()
        {
            ReflectorsHit = new List<Projectile>();
            Reflections = 0;
            Bounced = false;
            Bounces = 0;
            ref float timeSpentHoming = ref ExtraData[0];
            timeSpentHoming = 0;
        }

        public override void AI()
        {
            // use a forwards raycast to check if we will be colliding with a reflector next frame.

            bool foundReflector = false;

            List<Projectile> availableReflectors = new List<Projectile>();

            foreach (Projectile reflect in EntityManager.activeFriendlyProjectiles)
            {
                if (reflect.Label == EntityLabels.SharpShotReflector && !ReflectorsHit.Contains(reflect))
                {
                    availableReflectors.Add(reflect);
                }
            }

            if (availableReflectors.Count > 0)
            {
                foreach (Projectile reflector in availableReflectors)
                {
                    if (!ReflectorsHit.Contains(reflector) && IsCollidingWith(reflector, 1))
                    {
                        foundReflector = true;

                        Bounced = true;

                        int maxBuffs = 5;
                        float speedMultiplier = 1.2f;
                        float damageMultiplier = 1.1f;
                        float sizeMultiplier = 1.1f;

                        if (Bounces == 0)
                        {
                            speedMultiplier = 1.8f;
                            damageMultiplier = 1.5f;
                            sizeMultiplier = 1.4f;
                        }
                        else if (Bounces > maxBuffs) // cap the number of buffs that can be gained from bouncing
                        {
                            speedMultiplier = 1f;
                            damageMultiplier = 1f;
                            sizeMultiplier = 1f;
                        }

                        Damage *= damageMultiplier;
                        Scale *= sizeMultiplier;

                        ReflectorsHit.Add(reflector);

                        Position = reflector.Position;

                        ref float reflections = ref reflector.ExtraData[0];
                        int maxReflections = 3;

                        reflections += 1f;

                        if (reflections >= maxReflections)
                        {
                            reflector.Die();
                        }

                        Projectile closestReflector = EntityManager.ClosestProjectile(availableReflectors, Position, (Projectile p) => !ReflectorsHit.Contains(p));

                        if (closestReflector != null)
                        {
                            OnHitEffect(closestReflector.Position);
                            Bounces++;
                            
                            Velocity = speedMultiplier * Velocity.Length() * Utilities.SafeNormalise(closestReflector.Position - Position);
                        }
                        else
                        {
                            FlyRandomly();
                        }

                        Rotation = Utilities.VectorToAngle(Velocity);

                        //break;
                    }
                }
            }

            else
            {
                if (Bounced)
                {
                    Raycast.Direction = -1; // do not use a raycast now

                    NPC target = EntityManager.ClosestTargetableNPC(Position);

                    if (target != null)
                    {
                        //CommonProjectileAIs.Homing(this, target, 0, Velocity.Length(), 1f);
                        Velocity = Velocity.Length() * Utilities.SafeNormalise(target.Position - Position);
                    }
                }                
            }

            if (!foundReflector)
            {
                base.UpdatePosition();
            }
        }

        public override void OnHitEffect(Vector2 position)
        {
            int particles = (Bounces + 1) * 4;
            float particleSpeed = 10f;
            int particleLifetime = 30;
            int lifetimeSpread = 15;

            for (int i = 0; i < particles; i++)
            {
                CommonParticles.Spark(Position, particleSpeed, particleLifetime + Utilities.RandomInt(-lifetimeSpread, lifetimeSpread), Colour);
            }
        }
        public override void UpdatePosition()
        {
            // do nothing in here. the position is manually overriden when need be and allowed to update normally in AI().
        }

        public override void PreUpdate()
        {
            base.PreUpdate();
        }
        public void FlyRandomly()
        {
            // homes to target in AI
            Vector2 direction = Utilities.AngleToVector(Utilities.RandomAngle());
            Velocity = direction * Velocity.Length() * 1.3f;
        }

        public override void DealDamage(NPC npc)
        {
            base.DealDamage(npc);

            Position = npc.Position;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            DrawHitbox();
        }
    }
}
