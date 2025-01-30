﻿using bullethellwhatever.MainFiles;
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
        public Vector2 ReflectorJustHitPos; // prevent clipping through the reflector that was just hit
        public Vector2 BounceTarget; // after readjusting position to reflector, the velocity should be recalculated to ensure the next reflector is hit
        public override void Initialise()
        {
            ReflectorsHit = new List<Projectile>();
            Reflections = 0;
            Bounced = false;
            Bounces = 0;
            ReflectorJustHitPos = Vector2.Zero;
            BounceTarget = Vector2.Zero;
        }

        public override void AI()
        {
            List<Projectile> foundReflectors = new List<Projectile>();

            foreach (Projectile reflect in EntityManager.activeFriendlyProjectiles)
            {
                if (reflect.Label == EntityLabels.SharpShotReflector)
                {
                    foundReflectors.Add(reflect);
                }
            }

            if (foundReflectors.Count > 0)
            {
                foreach (Projectile reflector in foundReflectors)
                {
                    if (!ReflectorsHit.Contains(reflector) && IsCollidingWith(reflector))
                    {
                        Bounced = true;

                        int maxBuffs = 5;

                        if (Bounces <= maxBuffs)
                        {
                            Size *= 1.1f;
                            Damage *= 1.1f;
                        }

                        ReflectorsHit.Add(reflector);

                        Position = reflector.Position;

                        ref float reflections = ref reflector.ExtraData[0];
                        int maxReflections = 20;

                        reflections += 1f;

                        if (reflections >= maxReflections)
                        {
                            reflector.Die();
                        }   
                        
                        Projectile closestReflector = EntityManager.ClosestProjectile(foundReflectors, Position, (Projectile p) => !ReflectorsHit.Contains(p));

                        if (closestReflector != null)
                        {
                            OnHitEffect(closestReflector.Position);
                            Bounces++;
                            float speedMultiplier = 1.2f;

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
                    Velocity = Utilities.ConserveLengthLerp(Velocity, toTarget, 0.2f + AITimer * 0.01f);
                }
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
        public override void UpdatePosition(float progress)
        {
            // do the velocity update here. the trail updates using the previous position of the projectile, so we move the projectile to the reflector so that
            // on the next frame, the trail takes a position from the reflector, ensuring that the projectile trail goes through the reflector.
            // we do this after the velocity updates to effectively override it if need be
            // if a reflector was hit

            // the problem is that on the PREVIOUS frame the sharpshot goes past the reflector
            // since the raycast is backwards the check is only done AFTER the projectile has already moved past

            //if (ReflectorJustHitPos != Vector2.Zero)
            //{
            //    Position = ReflectorJustHitPos;

            //    // if there is a target reflector to go to
            //    if (BounceTarget != Vector2.Zero)
            //    {
            //        Vector2 direction = Utilities.SafeNormalise(BounceTarget - Position);
            //        Velocity = direction * Velocity.Length();

            //        BounceTarget = Vector2.Zero;
            //    }

            //}

            //ReflectorJustHitPos = Vector2.Zero;

            base.UpdatePosition(progress);
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
