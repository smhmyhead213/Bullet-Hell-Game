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
                        // ensure that the projectile stays at a reflector instead of detecting a collision after passing it
                        Position = reflector.Position;

                        Bounced = true;

                        int maxBuffs = 5;

                        if (Bounces <= maxBuffs)
                        {
                            Size *= 1.1f;
                            Damage *= 1.1f;
                        }

                        ReflectorsHit.Add(reflector);

                        ref float reflections = ref reflector.ExtraData[0];

                        reflections += 1f;

                        if (reflections >= 2)
                        {
                            reflector.Die();
                        }

                        Projectile closestReflector = EntityManager.ClosestProjectile(foundReflectors, Position, (Projectile p) => !ReflectorsHit.Contains(p));

                        if (closestReflector != null)
                        {
                            Bounces++;
                            float speedMultiplier = 1.2f;
                            Velocity = Velocity.Length() * speedMultiplier * Utilities.SafeNormalise(closestReflector.Position - Position);
                        }
                        else
                        {
                            FlyAtTarget();
                        }

                        Rotation = Utilities.VectorToAngle(Velocity);
                    }
                }
            }
            else if (Bounced)
            {
                // if there are no reflectors left and we've bounced at least once, home

                NPC target = EntityManager.ClosestTargetableNPC(Position);

                if (target != null)
                {
                    Vector2 toTarget = Utilities.SafeNormalise(target.Position - Position);

                    Velocity = Utilities.ConserveLengthLerp(Velocity, toTarget, 0.02f);
                }
            }
        }

        public void FlyAtTarget()
        {
            // homes to target in AI
            Vector2 direction = Utilities.AngleToVector(Utilities.RandomAngle());
            Velocity = direction * Velocity.Length() * 1.3f;
        }
    }
}
