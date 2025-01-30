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
        }

        public override void AI()
        {
            // use a forwards raycast to check if we will be colliding with a reflector next frame.

            bool foundReflector = false;

            List<Projectile> foundAvailableReflectors = new List<Projectile>();

            foreach (Projectile reflect in EntityManager.activeFriendlyProjectiles)
            {
                if (reflect.Label == EntityLabels.SharpShotReflector)
                {
                    foundAvailableReflectors.Add(reflect);
                }
            }

            if (foundAvailableReflectors.Count > 0)
            {
                foreach (Projectile reflector in foundAvailableReflectors)
                {
                    if (!ReflectorsHit.Contains(reflector) && IsCollidingWith(reflector, false))
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
                        Size *= sizeMultiplier;

                        ReflectorsHit.Add(reflector);

                        Position = reflector.Position;

                        ref float reflections = ref reflector.ExtraData[0];
                        int maxReflections = 20;

                        reflections += 1f;

                        if (reflections >= maxReflections)
                        {
                            reflector.Die();
                        }

                        Projectile closestReflector = EntityManager.ClosestProjectile(foundAvailableReflectors, Position, (Projectile p) => !ReflectorsHit.Contains(p));

                        if (closestReflector != null)
                        {
                            OnHitEffect(closestReflector.Position);
                            Bounces++;
                            
                            Velocity = speedMultiplier * Velocity.Length() * Utilities.SafeNormalise(closestReflector.Position - Position);
                        }
                        else
                        {
                            // fly off randomly
                            FlyRandomly();
                        }

                        Rotation = Utilities.VectorToAngle(Velocity);

                        //break;
                    }
                }
            }
            else if (Bounced) // move this to be if no valid reflectors are found, not just if there are none
            {
                // if there are no reflectors left and we've bounced at least once, home

                NPC target = EntityManager.ClosestTargetableNPC(Position);

                if (target != null)
                {
                    Vector2 toTarget = Utilities.SafeNormalise(target.Position - Position);

                    // home harder over time
                    Velocity = Utilities.ConserveLengthLerp(Velocity, toTarget, 0.2f + AITimer * 0.03f);
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
    }
}
