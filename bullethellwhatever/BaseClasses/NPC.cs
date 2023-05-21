using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.Base;
using System.Collections.Generic;
using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.DrawCode;

namespace bullethellwhatever.BaseClasses
{
    public class NPC : Entity
    {

        public float IFrames;
        public int MaxIFrames;

        public bool ContactDamage;
        public float HPRatio => Health / MaxHP;
        public DialogueSystem dialogueSystem;
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, string texture, Vector2 size, float MaxHealth, Color colour, bool shouldRemoveOnEdgeTouch, bool isHarmful)
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = Main.Assets[texture];
            Colour = colour;
            Main.NPCsToAddNextFrame.Add(this);
            Size = size;
            Health = MaxHealth;
            MaxHP = MaxHealth;
            ContactDamage = false;
            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            Opacity = 1f;
            
            IsHarmful = isHarmful;
            SetHitbox(this);
        }

        public override void AI()
        {

        }

        public virtual void Update()
        {
            if (IFrames > 0)
            {
                IFrames--;
            }

            Position = Position + Velocity;

            AITimer++;
        }
        public virtual bool IsCollidingWithEntity(Entity entity)
        {
            return Hitbox.Intersects(entity.Hitbox);
        }

        public virtual void CheckForHits() // all passive checks go here as well
        {
            if (Health < 0 && IsDesperationOver)
            {
                DeleteNextFrame = true;

                foreach (Projectile projectile in Main.activeProjectiles)
                {
                    if (projectile.Owner == this)
                    {
                        projectile.DeleteNextFrame = true;
                    }
                }
            }

            SetHitbox(this);

            if (!IsHarmful) // If you want the player able to spawn NPCs, make a friendlyNPCs list and check through that if the projectile is harmful.
            {
                foreach (NPC npc in Main.activeNPCs)
                {
                    if (IsCollidingWithEntity(npc) && npc.IFrames == 0) 
                    {
                        npc.IFrames = 5f;

                        npc.Health = npc.Health - Damage;

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
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {               
            Drawing.BetterDraw(Texture, Position, null, Colour, Rotation, Size, SpriteEffects.None, 0f);
        }
    }
}
