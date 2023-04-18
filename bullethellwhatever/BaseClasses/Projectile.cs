﻿using Microsoft.Xna.Framework;
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
        public bool IsHarmful;
        public Entity Owner;
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, Texture2D texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour)
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Acceleration = acceleration;
            Colour = colour;
            DeleteNextFrame = false;
            Size = size;
            Owner = owner;

            IsHarmful = isHarmful;

            if (isHarmful)
                Main.enemyProjectilesToAddNextFrame.Add(this);
            else Main.friendlyProjectilesToAddNextFrame.Add(this);
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

        //public override void DealDamage(Entity entity)
        //{

        //}

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Drawing.BetterDraw(Main.player.Texture, Position, null, Colour, Rotation, Size, SpriteEffects.None, 0f);
        }

    }
}