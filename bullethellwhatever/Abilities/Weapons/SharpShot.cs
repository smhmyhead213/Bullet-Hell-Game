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

        public override void Initialise()
        {
            ReflectorsHit = new List<Projectile>();
            Reflections = 0;
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

            foreach (Projectile reflector in foundReflectors)
            {
                if (CollidedWith(reflector))
                {
                    Damage *= 1.1f;
                    ReflectorsHit.Add(reflector);

                    if (ReflectorsHit.Count < foundReflectors.Count)
                    {
                        Projectile closestReflector = EntityManager.ClosestProjectile(foundReflectors, Position, (Projectile p) => !ReflectorsHit.Contains(p));

                        Velocity = Velocity.Length() * 1.2f * Utilities.SafeNormalise(closestReflector.Position - Position);
                    }
                    else
                    {
                        FlyAtTarget();
                    }
                }
            }
        }

        public void FlyAtTarget()
        {
            NPC target = EntityManager.ClosestTargetableNPC(Position);

            Vector2 direction = Vector2.Zero;

            if (target != null)
            {
                direction = Utilities.SafeNormalise(target.Position - Position);
            }
            else
            {
                direction = Utilities.AngleToVector(Utilities.RandomAngle());
            }

            Velocity = direction * Velocity.Length() * 1.3f;
        }
    }
}
