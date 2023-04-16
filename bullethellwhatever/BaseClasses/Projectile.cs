using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using bullethellwhatever.MainFiles;
using System.Runtime.CompilerServices;
using System;

namespace bullethellwhatever.BaseClasses
{
    public class Projectile : Entity
    {
        public float Acceleration;
        public float TimeAlive;
        public Entity Owner;
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, Texture2D texture, float acceleration, Vector2 size, Entity owner)
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Acceleration = acceleration;
            Main.enemyProjectilesToAddNextFrame.Add(this);
            DeleteNextFrame = false;
            Size = size;
            owner = Owner;
        }

         //and drawing
        
        public override bool ShouldRemoveOnEdgeTouch() => true;
        public override void AI()
        {
            TimeAlive++;
            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            Position = Position + Velocity;
        }

        public virtual bool IsCollidingWithEntity(Projectile projectile, Entity entity)
        {
            float totalwidth = Hitbox.Width + entity.Hitbox.Width;

            if (Math.Abs(Position.X - entity.Position.X) <= totalwidth && Math.Abs(Position.Y - entity.Position.Y) <= totalwidth)
                return true;

            return false;
        }

    }
}
