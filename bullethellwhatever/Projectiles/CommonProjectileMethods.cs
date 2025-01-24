using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses.Entities;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.Projectiles
{
    public static class CommonProjectileMethods
    {
        public static Projectile SpawnProjectile(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            Projectile proj = new Projectile();

            proj.SpawnProjectile(position, velocity, damage, pierce, texture, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);

            return proj;
        }

        public static T SpawnProjectile<T>(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit) where T : Projectile, new()
        {
            T proj = new T();

            proj.SpawnProjectile(position, velocity, damage, pierce, texture, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);

            return proj;
        }

        public static void SpawnProjectile(this Projectile proj, Vector2 position, Vector2 velocity, float damage, int pierce, string texture, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            proj.Prepare(position, velocity, damage, pierce, texture, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);
        }

        public static void RadialProjectileBurst(Projectile projectile, int numberOfProjectiles, float angleOffset, float projectileSpeed, Action extraProjectileAI = null)
        {
            for (int i = 0; i < numberOfProjectiles; i++)
            {
                // change the texture used here once ManagedTexture is implemented
                Projectile p = SpawnProjectile<Projectile>(projectile.Position, projectileSpeed * Utilities.AngleToVector(Tau / numberOfProjectiles * i + angleOffset), projectile.Damage, projectile.PierceRemaining, "box", projectile.Size,
                    projectile.Owner, projectile.IsHarmful, projectile.Colour, projectile.ShouldRemoveOnEdgeTouch, projectile.RemoveOnHit);

                p.Texture = projectile.Texture;
            }
        }
        public static Deathray SpawnDeathray(Vector2 position, float initialRotation, float damage, int duration, string texture, float width,
            float length, float angularVelocity, bool isHarmful, Color colour, string? shader, Entity owner)
        {
            Deathray ray = new Deathray();

            ray.CreateDeathray(position, initialRotation, damage, duration, texture, width, length, angularVelocity, isHarmful, colour, shader, owner);

            ray.AddDeathrayToActiveProjectiles();

            return ray;
        }

        public static TelegraphLine SpawnTelegraphLine(float rotation, float rotationalVelocity, float width, float length, int duration, Vector2 origin, Color colour, string texture, Entity owner, bool stayWithOwner)
        {
            return new TelegraphLine(rotation, rotationalVelocity, width, length, duration, origin, colour, texture, owner, stayWithOwner);
        }

        // makes a projectile spawn with a velocityh that hits 0 at the end of its lifetime
        public static void FadeOutParticle(this Particle p, float rotation, float speed, int lifetime, Vector2 position, Color colour)
        {
            Vector2 velocity = speed * Utilities.RotateVectorClockwise(-Vector2.UnitY, rotation);
            p.Spawn("box", position, velocity, -velocity / 2f / lifetime, Vector2.One * 0.45f, rotation, colour, 1f, 20);
        }
        public static void ExponentialAccelerate(this Projectile projectile, float acceleration)
        {
            projectile.Velocity *= acceleration;
        }
    }
}
