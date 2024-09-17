using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.NPCs;
using bullethellwhatever.DrawCode;
using bullethellwhatever.AssetManagement;
using SharpDX.XAudio2;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public enum AppendageType
    {
        UpperArm,
        LowerArm,
        UpperClaw,
        LowerClaw,
    }
    public class CrabBossAppendage : NPC
    {
        public AppendageType Type;
        
        public Vector2 End; //where other limbs attach on

        public CrabLeg Leg;

        public Entity Owner;

        public int LegIndex;

        public float RotationConstant;

        public float RotationToAdd;

        public float Gravity;
        public CrabBossAppendage(Entity owner, CrabLeg leg, AppendageType appendageType, int legIndex)
        {
            Owner = owner;
            Leg = leg;
            Type = appendageType;

            string texture = "";

            switch (Type)
            {
                case AppendageType.UpperArm:
                    {
                        texture = "CrabUpperArm";
                        break;
                    }
                case AppendageType.LowerArm:
                    {
                        texture = "CrabLowerArm";
                        break;
                    }
                case AppendageType.UpperClaw:
                    {
                        texture = "CrabUpperClaw";
                        break;
                    }
                case AppendageType.LowerClaw:
                    {
                        texture = "CrabLowerClaw";
                        break;
                    }
            }

            Texture = AssetRegistry.GetTexture2D(texture);
            Size = Vector2.One;
            IsHarmful = true;
            Damage = 1f;

            Updates = 1;

            PierceToTake = 20;

            PrepareNPC();
            LegIndex = legIndex;
            //Rotation = Rotation + PI / 2;

            Velocity = Vector2.Zero;
            RotationalVelocity = 0;

            Gravity = 0.7f; // for death anim

            Colour = Color.White;
        }

        public override void Update()
        {
            //Rotation = Rotation + PI / 60f;
            //if (this is CrabBossUpperArm)

            Position = Position + Velocity;

            if (!((CrabBoss)Owner).StartedDeathAnim)
            {
                Rotation = CalculateFinalRotation();
            }

            Rotation = Rotation + RotationalVelocity;

            //End = CalculateEnd();

            if (Health <= 0)
            {
                TargetableByHoming = false;
                IsInvincible = true;
                Colour = Color.Gray;
            }
            else
            {
                TargetableByHoming = true;
                IsInvincible = false;
            }
        }
        
        public virtual void SetMaxHP(float maxHP, bool setHP)
        {
            MaxHP = maxHP;

            if (setHP)
            {
                Health = MaxHP;
            }
        }
        public virtual float RotationFromV() // rotation from vertical
        {
            float output = RotationConstant + PI + Rotation;
            
            while (output > Tau)
            {
                output = output - Tau;
            }

            return output;
        }
        public virtual Vector2 CalculateEnd()
        {
            return Position + new Vector2(-Sin(Rotation), Cos(Rotation)) * Texture.Height * GetSize().Y;
        }

        public void Rotate(float angle)
        {
            RotationToAdd = RotationToAdd + angle;
        }

        public override void UpdateHitbox()
        {
            Vector2 centre = Vector2.Lerp(Position, CalculateEnd(), 0.5f); // centre is halfway along arm

            if (Type == AppendageType.UpperClaw)
            {
                centre = centre - Utilities.RotateVectorClockwise(new Vector2(Texture.Width / 2f * GetSize().X, 0f), Rotation); // yeah totally sure yeah i was there yeah thats crazy man so true for real?
            }

            Hitbox.UpdateRectangle(Rotation, Texture.Width * GetSize().X, Texture.Height * GetSize().Y, centre);

            Hitbox.UpdateVertices();
        }
        public float CalculateFinalRotation()
        {
            if (!((CrabBoss)Owner).StartedPhaseTwoTransition)
                return Owner.Rotation + RotationConstant + RotationToAdd;

            else return RotationConstant + RotationToAdd; // if arms are detached, dont make arms rotate with body
        }

        public void PointInDirection(float angle)
        {
            Rotate(-Rotation + angle + PI);
        }

        public void HandleBounces()
        {
            for (int i = 0; i < Hitbox.Vertices.Length; i++)
            {
                float vertexX = Hitbox.Vertices[i].X;
                float vertexY = Hitbox.Vertices[i].Y;

                if (vertexX < 0 && Velocity.X < 0)
                {
                    Velocity.X = Velocity.X * -1f;
                    RotationalVelocity = -Sign(Velocity.X) * PI / 12;
                }

                if (vertexX > GameWidth && Velocity.X > 0)
                {
                    Velocity.X = Velocity.X * -1f;
                }

                if (vertexY < 0 && Velocity.Y < 0)
                {
                    Velocity.Y = Velocity.Y * -1f;
                }

                if (vertexY > GameHeight && Velocity.Y > 0)
                {
                    Velocity.Y = Velocity.Y * -1f;

                    Velocity.Y = Velocity.Y / 10f;
                    Gravity = Gravity / 10f;
                }
            }

            RotationalVelocity = RotationalVelocity * 0.95f;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 originOffset = new Vector2(Texture.Width / 2, 0f);
            SpriteEffects spriteEffect = SpriteEffects.None;

            if (Type == AppendageType.UpperClaw)
            {
                originOffset = new Vector2(Texture.Width, 0);
            }

            if (LegIndex == 0)
            {
                if (Type == AppendageType.UpperClaw)
                {
                    originOffset = new Vector2(Texture.Width, 0);
                }
                else
                {
                    originOffset = new Vector2(Texture.Width / 2, 0f);
                }

                spriteEffect = SpriteEffects.FlipHorizontally;
            }

            Drawing.BetterDraw(Texture, Position, null, Colour, Rotation, GetSize(), spriteEffect, 1f, originOffset);

            Texture2D texture = AssetRegistry.GetTexture2D("box");

            //spriteBatch.Draw(texture, Position, null, Color.Red, 0, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 1);
            //Hitbox.DrawVertices(spriteBatch, Color.Red);
        }
        public override void DrawHPBar(SpriteBatch spriteBatch)
        {
            // do nothing.
        }
    }
}
