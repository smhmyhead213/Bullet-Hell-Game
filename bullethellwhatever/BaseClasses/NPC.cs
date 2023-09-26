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

        
        public bool TargetableByHoming;

        public int PierceToTake;        
        public float HPRatio => Health / MaxHP;
        public DialogueSystem dialogueSystem;
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, string texture, Vector2 size, float MaxHealth, int pierceToTake, Color colour, bool shouldRemoveOnEdgeTouch, bool isHarmful)
        {
            Updates = 1; //default

            Depth = 0;

            DrawAfterimages = false;

            TargetableByHoming = true;

            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = Assets[texture];
            Colour = colour;

            Size = size;
            Health = MaxHealth;
            MaxHP = MaxHealth;
            PierceToTake = pierceToTake;

            DealDamage = false;
            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            Opacity = 1f;

            dialogueSystem = new DialogueSystem(this);
            dialogueSystem.dialogueObject = new DialogueObject(position, string.Empty, this, 1, 1);

            IsHarmful = isHarmful;

            PrepareNPC();
        }
        //this one takes a texture directly
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, Texture2D texture, Vector2 size, float MaxHealth, int pierceToTake, Color colour, bool shouldRemoveOnEdgeTouch, bool isHarmful)
        {
            Updates = 1;

            Depth = 0;

            TargetableByHoming = true;

            DrawAfterimages = false;

            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Colour = colour;

            Size = size;
            Health = MaxHealth;
            MaxHP = MaxHealth;
            PierceToTake = pierceToTake;

            DealDamage = false;
            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            Opacity = 1f;

            Depth = 0;

            IsHarmful = isHarmful;

            PrepareNPC();
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

            if (afterimagesPositions is not null)
            {
                Utilities.moveVectorArrayElementsUpAndAddToStart(ref afterimagesPositions, Position);
            }

            if (Health <= 0)
            {
                TargetableByHoming = false;
            }
            else TargetableByHoming = true;

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
                Die();

                foreach (Projectile projectile in activeProjectiles)
                {
                    if (projectile.Owner == this)
                    {
                        projectile.Die(); //when an npc dies, kill all of its projectiles
                    }
                }
            }

            if (!IsHarmful) // If you want the player able to spawn NPCs, make a friendlyNPCs list and check through that if the projectile is harmful.
            {
                foreach (NPC npc in activeNPCs)
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
                if (IsCollidingWithEntity(player) && player.IFrames == 0)
                {
                    player.IFrames = 20f;

                    player.Health = player.Health - Damage;

                    Drawing.ScreenShake(3, 10);
                }
            }
        }

        public virtual void CreateNPC(Vector2 position, Vector2 velocity, float damage, Texture2D texture, Vector2 size, float MaxHealth, int pierceToTake, Color colour, bool shouldRemoveOnEdgeTouch, bool isHarmful)
        {
            Updates = 1;

            Depth = 0;

            DrawAfterimages = false;

            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Colour = colour;

            Size = size;
            Health = MaxHealth;
            MaxHP = MaxHealth;
            PierceToTake = pierceToTake;

            DealDamage = false;
            ShouldRemoveOnEdgeTouch = shouldRemoveOnEdgeTouch;
            Opacity = 1f;


            IsHarmful = isHarmful;
        }
        public virtual void PrepareNPC()
        {
            PrepareNPCButDontAddToListYet();

            NPCsToAddNextFrame.Add(this);

        }

        public virtual void PrepareNPCButDontAddToListYet()
        {
            Hitbox = new RotatedRectangle(Rotation, Texture.Width * GetSize().X, Texture.Height * GetSize().Y, Position, this);
            SetHitbox();
            dialogueSystem = new DialogueSystem(this);
            dialogueSystem.dialogueObject = new DialogueObject(Position, string.Empty, this, 1, 1);
        }
    }
}
