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

namespace bullethellwhatever.Abilities.Weapons
{
    public class SharpShot : Projectile
    {
        public List<Projectile> ReflectorsHit;
        public int Reflections;
        public bool Bounced;

        public override void Initialise()
        {
            ReflectorsHit = new List<Projectile>();
            Reflections = 0;
            Bounced = false;
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
                    if (!ReflectorsHit.Contains(reflector) && CollidedWith(reflector))
                    {
                        Bounced = true;

                        Damage *= 1.1f;
                        Size *= 1.1f;

                        ReflectorsHit.Add(reflector);

                        ref float reflections = ref reflector.ExtraData[0];

                        reflections += 1f;

                        if (reflections >= 2)
                        {
                            reflector.Die();
                        }

                        if (ReflectorsHit.Count < foundReflectors.Count)
                        {
                            Projectile closestReflector = EntityManager.ClosestProjectile(foundReflectors, Position, (Projectile p) => !ReflectorsHit.Contains(p));

                            Velocity = Velocity.Length() * 1.2f * Utilities.SafeNormalise(closestReflector.Position - Position);
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

                Vector2 toTarget = Utilities.SafeNormalise(target.Position - Position);

                Velocity = Utilities.ConserveLengthLerp(Velocity, toTarget, 0.02f);
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
