using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bullethellwhatever
{
    public class NPC : Entity
    {
        
        public float IFrames;
        public bool ContactDamage;
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, Texture2D texture, float size, float MaxHealth)
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Main.activeNPCs.Add(this);
            Size = size;
            Hitbox = new((int)position.X - (texture.Width / 2), (int)position.Y - (texture.Height / 2), texture.Width, texture.Height);
            Hitbox.Width = Hitbox.Width * (int)size;
            Hitbox.Height = Hitbox.Height * (int)size;
            Health = MaxHealth;
            MaxHP = MaxHealth;
            ContactDamage = false;
        }

        public override void AI()
        {
            
        }

        public override void HandleMovement()
        {
            
        }

        public bool isCollidingWithPlayerProjectile(FriendlyProjectile projectile)
        {
            float totalwidth = Hitbox.Width + projectile.Hitbox.Width;

            if (Math.Abs(Position.X - projectile.Position.X) <= totalwidth && Math.Abs(Position.Y - projectile.Position.Y) <= totalwidth)
                return true;

            return false;
        }


    }
}
