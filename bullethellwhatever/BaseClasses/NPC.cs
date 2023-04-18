using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.Base;

namespace bullethellwhatever.BaseClasses
{
    public class NPC : Entity
    {

        public float IFrames;
        public bool ContactDamage;
        public float HPRatio => Health / MaxHP;
        public DialogueSystem dialogueSystem;
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, Texture2D texture, Vector2 size, float MaxHealth, Color colour)
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Colour = colour;
            Main.NPCsToAddNextFrame.Add(this);
            Size = size;
            Hitbox = new((int)position.X - texture.Width / 2, (int)position.Y - texture.Height / 2, texture.Width, texture.Height);
            Hitbox.Width = Hitbox.Width * (int)size.X;
            Hitbox.Height = Hitbox.Height * (int)size.Y;
            Health = MaxHealth;
            MaxHP = MaxHealth;
            ContactDamage = false;
        }

        public override void AI()
        {
            CheckForAndTakeDamage();
        }

        public bool isCollidingWithPlayerProjectile(Projectile projectile)
        {
            float totalwidth = Hitbox.Width + projectile.Hitbox.Width;

            if (Math.Abs(Position.X - projectile.Position.X) <= totalwidth && Math.Abs(Position.Y - projectile.Position.Y) <= totalwidth)
                return true;

            return false;
        }

        public void CheckForAndTakeDamage()
        {
            foreach (Projectile projectile in Main.activeFriendlyProjectiles)
            {
                if (projectile.IsCollidingWithEntity(projectile, this) && IFrames == 0)
                {
                    if (IFrames == 0)
                    {
                        IFrames = 5f;
                        
                        Health = Health - projectile.Damage;

                        if (projectile is not Deathray)
                        {
                            projectile.DeleteNextFrame = true;
                        }
                    }
                }
            }
        }
    }
}
