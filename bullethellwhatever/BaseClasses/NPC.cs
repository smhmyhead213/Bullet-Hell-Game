using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Player;
using bullethellwhatever.UtilitySystems.Dialogue;

namespace bullethellwhatever.BaseClasses
{
    public class NPC : Entity
    {

        public float IFrames;
        public bool ContactDamage;
        public float HPRatio => Health / MaxHP;
        public DialogueSystem dialogueSystem;

        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, Texture2D texture, Vector2 size, float MaxHealth)
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;

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

        public bool isCollidingWithPlayerProjectile(FriendlyProjectile projectile)
        {
            float totalwidth = Hitbox.Width + projectile.Hitbox.Width;

            if (Math.Abs(Position.X - projectile.Position.X) <= totalwidth && Math.Abs(Position.Y - projectile.Position.Y) <= totalwidth)
                return true;

            return false;
        }

        public void CheckForAndTakeDamage()
        {
            foreach (FriendlyProjectile projectile in Main.activeFriendlyProjectiles)
            {
                if (isCollidingWithPlayerProjectile(projectile) && IFrames == 0)
                {
                    if (IFrames == 0)
                    {
                        IFrames = 5f;
                        if (projectile is PlayerSharpShooterProjectile) //give the sharpshooter projectile its damage multiplier against moveing targets
                        {
                            Health = Health - (projectile.Damage * (Velocity.Length() / 7f) + 0.3f);
                        }
                        else
                        {
                            Health = Health - projectile.Damage;
                        }
                        projectile.DeleteNextFrame = true;
                    }
                }
            }
        }
    }
}
