using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.BaseClasses.Hitboxes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.Projectiles
{
    public static class CommonProjectileMethods
    {
        public static Projectile SpawnProjectile(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            Projectile proj = new Projectile();

            proj.Depth = 0;
            proj.Texture = Assets[texture];

            if (!proj.DrawAfterimages) // dont set to false by default if already set to true
            {
                proj.DrawAfterimages = false;
            }

            proj.Position = position;
            proj.Pierce = pierce;
            proj.TimeOutsidePlayArea = 0;
            proj.PierceRemaining = proj.Pierce;
            proj.Velocity = velocity;
            proj.Damage = damage;
            proj.Acceleration = acceleration;
            proj.Colour = colour;
            proj.DeleteNextFrame = false;
            proj.Size = size;
            proj.Owner = owner;
            proj.IsHarmful = isHarmful;
            proj.ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            proj.RemoveOnHit = removeOnHit;

            proj.DealDamage = isHarmful;

            if (proj.Updates == 0) // if we havent already set updates
            {
                proj.Updates = 1;
            }
            proj.Opacity = 1f;
            proj.InitialOpacity = proj.Opacity;
            proj.Dying = false;

            proj.DyingTimer = 0;

            proj.Hitbox = new RotatedRectangle(proj.Rotation, proj.Texture.Width * proj.GetSize().X, proj.Texture.Height * proj.GetSize().Y, proj.Position, this);

            proj.Participating = true;

            if (proj.MercyTimeBeforeRemoval == 0)
            {
                proj.MercyTimeBeforeRemoval = 60;
            }

            proj.SetHitbox();

            if (proj.IsHarmful)
                enemyProjectilesToAddNextFrame.Add(proj);
            else friendlyProjectilesToAddNextFrame.Add(proj);

            return proj;
        }

        public static Projectile SpawnProjectile(Vector2 position, Vector2 velocity, float damage, int pierce, Texture2D texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            Projectile proj = new Projectile();

            proj.Depth = 0;
            proj.Texture = texture;

            if (!proj.DrawAfterimages) // dont set to false by default if already set to true
            {
                proj.DrawAfterimages = false;
            }

            proj.Position = position;
            proj.Pierce = pierce;
            proj.TimeOutsidePlayArea = 0;
            proj.PierceRemaining = proj.Pierce;
            proj.Velocity = velocity;
            proj.Damage = damage;
            proj.Acceleration = acceleration;
            proj.Colour = colour;
            proj.DeleteNextFrame = false;
            proj.Size = size;
            proj.Owner = owner;
            proj.IsHarmful = isHarmful;
            proj.ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            proj.RemoveOnHit = removeOnHit;

            proj.DealDamage = isHarmful;

            if (proj.Updates == 0) // if we havent already set updates
            {
                proj.Updates = 1;
            }
            proj.Opacity = 1f;
            proj.InitialOpacity = proj.Opacity;
            proj.Dying = false;

            proj.DyingTimer = 0;

            proj.Hitbox = new RotatedRectangle(proj.Rotation, proj.Texture.Width * proj.GetSize().X, proj.Texture.Height * proj.GetSize().Y, proj.Position, this);

            proj.Participating = true;

            if (proj.MercyTimeBeforeRemoval == 0)
            {
                proj.MercyTimeBeforeRemoval = 60;
            }

            proj.SetHitbox();

            if (proj.IsHarmful)
                enemyProjectilesToAddNextFrame.Add(proj);
            else friendlyProjectilesToAddNextFrame.Add(proj);

            return proj;
        }
    }
}
