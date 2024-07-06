using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.Projectiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.Projectiles
{
    public static class CommonProjectileMethods
    {
        public static Projectile SpawnProjectile(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            Projectile proj = new Projectile();

            proj.Texture = Assets[texture];

            proj.Prepare(position, velocity, damage, pierce, acceleration, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);

            return proj;
        }

        public static Projectile SpawnProjectile(Vector2 position, Vector2 velocity, float damage, int pierce, Texture2D texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            Projectile proj = new Projectile();

            proj.Texture = texture;

            proj.Prepare(position, velocity, damage, pierce, acceleration, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);

            return proj;
        }

        public static Deathray SpawnDeathray(Vector2 position, float initialRotation, float damage, int duration, string texture, float width,
            float length, float angularVelocity, float angularAcceleration, bool isHarmful, Color colour, string? shader, Entity owner)
        {
            Deathray ray = new Deathray();

            ray.CreateDeathray(position, initialRotation, damage, duration, texture, width, length, angularVelocity, angularAcceleration, isHarmful, colour, shader, owner);

            AddDeathrayToActiveProjectiles();
        }
    }
}
