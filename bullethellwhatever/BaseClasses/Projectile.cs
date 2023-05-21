using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using bullethellwhatever.MainFiles;
using bullethellwhatever.DrawCode;
using System.Runtime.CompilerServices;
using System;
using bullethellwhatever.Projectiles.Base;


namespace bullethellwhatever.BaseClasses
{
    public class Projectile : Entity
    {
        public float Acceleration;
        public int TimeAlive;
        public Entity Owner;
        public bool RemoveOnHit;
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, string texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = Main.Assets[texture];
            Acceleration = acceleration;
            Colour = colour;
            DeleteNextFrame = false;
            Size = size;
            Owner = owner;
            IsHarmful = isHarmful;
            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            RemoveOnHit = removeOnHit;
            Hitbox = new((int)position.X - Texture.Width / 2, (int)position.Y - Texture.Height / 2, Texture.Width, Texture.Height);
            Hitbox.Width = Hitbox.Width * (int)size.X;
            Hitbox.Height = Hitbox.Height * (int)size.Y;
            Opacity = 1f;

            if (isHarmful)
                Main.enemyProjectilesToAddNextFrame.Add(this);
            else Main.friendlyProjectilesToAddNextFrame.Add(this);
        }

         //and drawing
        
        public override void AI()
        {
           
            TimeAlive++;
            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            Position = Position + Velocity;
        }

        public virtual void CheckForHits()
        {
            Hitbox = new((int)Position.X - Texture.Width / 2, (int)Position.Y - Texture.Height / 2, Texture.Width, Texture.Height);

            if (!IsHarmful) // If you want the player able to spawn NPCs, make a friendlyNPCs list and check through that if the projectile is harmful.
            {
                foreach (NPC npc in Main.activeNPCs)
                {
                    if (IsCollidingWithEntity(npc) && npc.IFrames == 0) //why am i checking this twice? remove
                    {
                        npc.IFrames = npc.MaxIFrames;

                        npc.Health = npc.Health - Damage;

                        if (RemoveOnHit)
                            DeleteNextFrame = true;
                    }
                }
            }

            if (IsHarmful)
            {
                if (IsCollidingWithEntity(Main.player) && Main.player.IFrames == 0)
                {
                    Main.player.IFrames = 20f;

                    Main.player.Health = Main.player.Health - Damage;

                    Drawing.ScreenShake(3, 10);

                    if (RemoveOnHit)
                        DeleteNextFrame = true;
                }
            }
        }
        public virtual bool IsCollidingWithEntity(Entity entity)
        {
            if (entity.Hitbox.Intersects(Hitbox))
                return true;
            else return false;
        }

        //public virtual void DealDamage()
        //{
        //    foreach (NPC npc in Main.activeNPCs)
        //    {
        //        if (npc.IsHarmful != IsHarmful)
        //        {
        //            if (IsCollidingWithEntity(this, npc) && npc.IFrames == 0) //why am i checking this twice? remove
        //            {
        //                    npc.IFrames = 5f;

        //                    npc.Health = npc.Health - Damage;

        //                    DeleteNextFrame = true;
        //            }
        //        }
        //    }
        //}

        public override void Draw(SpriteBatch s)
        {
            Drawing.BetterDraw(Main.player.Texture, Position, null, Colour * Opacity, Rotation, Size, SpriteEffects.None, 0f);
        }

    }
}
