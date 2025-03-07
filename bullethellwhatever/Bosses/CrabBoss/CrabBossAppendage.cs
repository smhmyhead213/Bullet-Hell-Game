using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.MainFiles;
using bullethellwhatever.NPCs;
using bullethellwhatever.DrawCode;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses.Entities;
using Microsoft.Xna.Framework.Media;
using bullethellwhatever.BaseClasses.Hitboxes;

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

        public CrabArm Leg;

        public Entity Owner;

        public Entity BehindThis;

        public int ArmIndex;

        public float RotationToAdd;

        public CrabBossAppendage(Entity owner, CrabArm leg, AppendageType appendageType, int legIndex, float scale = 1f)
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
            Scale = Vector2.One * scale;
            HarmfulToPlayer = true;
            Damage = 1f;

            Updates = 1;

            PierceToTake = 20;

            PrepareNPC();
            ArmIndex = legIndex;
            //Rotation = Rotation + PI / 2;

            Velocity = Vector2.Zero;
            RotationalVelocity = 0;

            Colour = Color.White;
        }

        public override void PostUpdate()
        {
            //Rotation = Rotation + PI / 60f;
            //if (this is CrabBossUpperArm)

            base.PostUpdate();

            //Rotation = CalculateFinalRotation();

            Rotation = Rotation + RotationalVelocity; // this might be already done in postupdate, consider removing

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

            RotationToAdd = Utilities.ClosestToZero(RotationToAdd);
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
            return Utilities.ClosestToZero(Owner.Rotation + PI + RotationToAdd);
            //float output = PI + Rotation;
            
            //while (output > Tau)
            //{
            //    output = output - Tau;
            //}

            //return output;
        }
        public virtual Vector2 CalculateEnd()
        {
            return Position + new Vector2(-Sin(CalculateFinalRotation()), Cos(CalculateFinalRotation())) * Texture.Height * GetSize().Y;
        }

        public void Rotate(float angle)
        {
            RotationToAdd = RotationToAdd + angle;
        }

        public void LerpRotation(float startAngle, float endAngle, float interpolant)
        {
            RotationToAdd = MathHelper.Lerp(startAngle, endAngle, interpolant);
        }

        public void LerpTo(float endAngle, float interpolant)
        {
            RotationToAdd = MathHelper.Lerp(RotationToAdd, endAngle, interpolant);
        }
        public void LerpToZero(float interpolant)
        {
            LerpRotation(RotationToAdd, 0f, interpolant);
        }

        /// <summary>
        /// Makes this appendage point in the same direction as the previous offset by an angle. The upper arm will point in the same direction as the boss.
        /// </summary>
        public void PointFromForward(float angle)
        {
            if (Type == AppendageType.UpperArm)
            {
                PointInDirection(BehindThis.Rotation + PI + angle); // the entity behind this is the crab body
            }
            else
            {
                PointInDirection(((CrabBossAppendage)BehindThis).RotationFromV() + angle);
            }         
        }

        public float CalculateFinalRotation()
        {
            float output = Owner.Rotation + RotationToAdd;

            return output;

            if (Type == AppendageType.UpperClaw || Type == AppendageType.LowerClaw)
            {

            }
        }

        public void PointInDirection(float angle)
        {
            Rotate(-Rotation + angle + PI);
        }

        public override bool ConsideredForCameraZoom()
        {
            return false;
        }

        public float Length()
        {
            return Texture.Height * GetSize().Y;
        }

        public override void DeductHealth(float damage)
        {
            Leg.Owner.TakeDamage(damage);
        }

        public List<Circle> LimbHitbox()
        {
            if (Type == AppendageType.UpperArm || Type == AppendageType.LowerArm)
            {
                Vector2 centre = Position + new Vector2(0f, Height() / 2f).Rotate(Rotation);
                return Utilities.FillRectWithCircles(centre, (int)Width(), (int)Height(), Rotation);
            }
            else if (Type == AppendageType.UpperClaw)
            {
                int expandedi = -Utilities.ExpandedIndex(ArmIndex);

                Vector2 upperPartCentre = Position + new Vector2(expandedi * Width() / 2f, Height() / 2f).Rotate(Rotation);
                return Utilities.FillRectWithCircles(upperPartCentre, (int)Width(), (int)Height(), Rotation);
            }
            else // lower claw
            {
                Vector2 upperPartCentre = Position + new Vector2(0f, Height() / 2f).Rotate(Rotation);
                return Utilities.FillRectWithCircles(upperPartCentre, (int)Width(), (int)(Height()), Rotation);
            }
        }
        public override void UpdateHitbox()
        {
            // do nothing here: logic is handled in LimbHitbox and added to the crabboss manually
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 originOffset = new Vector2(Texture.Width / 2, 0f);
            SpriteEffects spriteEffect = SpriteEffects.None;

            if (Type == AppendageType.UpperClaw)
            {
                originOffset = new Vector2(Texture.Width, 0);
            }

            if (ArmIndex == 0)
            {
                if (Type == AppendageType.UpperClaw)
                {
                    originOffset = new Vector2(0, 0);
                }

                spriteEffect = SpriteEffects.FlipHorizontally;
            }

            Drawing.BetterDraw(Texture, Position, null, Colour, Rotation, GetSize(), spriteEffect, 1f, originOffset);
            //Drawing.DrawBox(CalculateEnd(), Color.MediumPurple, 0.5f);
            //Drawing.DrawBox(Position, Color.Blue, 0.5f);
            //DrawHitbox();
        }
        public override void DrawHPBar(SpriteBatch spriteBatch)
        {
            // do nothing.
        }
    }
}
