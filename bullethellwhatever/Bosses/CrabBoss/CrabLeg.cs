﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabLeg
    {
        public Vector2 Position;
        public Vector2 Velocity;

        public CrabBoss Owner;

        public bool Dead;

        public int LegIndex;

        //public bool LockPosition;

        public CrabBossAppendage[] LegParts;
        public CrabBossAppendage UpperArm => LegParts[0];
        public CrabBossAppendage LowerArm => LegParts[1];
        public CrabBossAppendage UpperClaw => LegParts[2];
        public CrabBossAppendage LowerClaw => LegParts[3];

        public bool HorizontalFlip;

        public bool DeathAnimation;

        public CrabLeg(Vector2 position, CrabBoss owner, int legIndex)
        {
            LegParts = new CrabBossAppendage[4];
            Owner = owner;

            LegParts[0] = new CrabBossAppendage(Owner, this, AppendageType.UpperArm, legIndex);
            UpperArm.SetMaxHP(50f, true);
            LegParts[1] = new CrabBossAppendage(Owner, this, AppendageType.LowerArm, legIndex);
            LowerArm.SetMaxHP(35f, true);
            LegParts[2] = new CrabBossAppendage(Owner, this, AppendageType.UpperClaw, legIndex);
            UpperClaw.SetMaxHP(20f, true);
            LegParts[3] = new CrabBossAppendage(Owner, this, AppendageType.LowerClaw, legIndex);
            LowerClaw.SetMaxHP(20f, true);

            HorizontalFlip = false;

            LegIndex = legIndex;

            DeathAnimation = false;

            Position = position;
            UpperArm.Position = Position;
            LowerArm.Position = UpperArm.CalculateEnd();
            UpperClaw.Position = LowerArm.CalculateEnd();
            LowerClaw.Position = LowerArm.CalculateEnd();

            Dead = false;
        }

        public void MoveToPoint(Vector2 point, int movementTimer, int duration)
        {
            Vector2 vectorToPoint = point - Position;
            float distanceToTravel = vectorToPoint.Length();

            // top 5 integration moments
            Owner.Velocity = Utilities.SafeNormalise(vectorToPoint, Vector2.Zero) * (2f * PI * distanceToTravel / duration) * Sin(PI * movementTimer / duration);
        }

        public float Length()
        {
            return UpperArm.Texture.Height * UpperArm.GetSize().Y + LowerArm.Texture.Height * LowerArm.GetSize().Y + UpperClaw.Texture.Height * UpperClaw.GetSize().Y + LowerClaw.Texture.Height * LowerClaw.GetSize().Y;
        }
        public void ResetRotations()
        {
            UpperArm.RotationToAdd = 0f;
            LowerArm.RotationToAdd = 0f;
            UpperClaw.RotationToAdd = 0f;
            LowerClaw.RotationToAdd = 0f;
        }

        public void PointLegInDirection(float angle)
        {
            UpperArm.Rotate(-UpperArm.Rotation + angle + PI);
            LowerArm.Rotate(-LowerArm.Rotation + angle + PI);
            UpperClaw.Rotate(-UpperClaw.Rotation + angle + PI);
            LowerClaw.Rotate(-LowerClaw.Rotation + angle + PI);
        }

        public void ContactDamage(bool on)
        {
            UpperArm.DealDamage = on;
            LowerArm.DealDamage = on;
            UpperClaw.DealDamage = on;
            LowerClaw.DealDamage = on;
        }
        public void RotateLeg(float angle)
        {
            UpperArm.Rotate(angle);
            LowerArm.Rotate(angle);
            UpperClaw.Rotate(angle);
            LowerClaw.Rotate(angle);
        }

        public void SetAllSizes(Vector2 size)
        {
            UpperArm.Size = size;
            LowerArm.Size = size;
            UpperClaw.Size = size;
            LowerClaw.Size = size;
        }
        public void DrawHitboxes()
        {
            UpperArm.Hitbox.DrawHitbox();
            LowerArm.Hitbox.DrawHitbox();
            UpperClaw.Hitbox.DrawHitbox();
            LowerClaw.Hitbox.DrawHitbox();
        }

        public void SetAllRotations(float rotation)
        {
            foreach (CrabBossAppendage appendage in LegParts)
            {
                appendage.Rotation = rotation;
            }
        }
        public Vector2 PositionAtDistanceFromWrist(float distance)
        {
            return LowerArm.CalculateEnd() + (Utilities.AngleToVector(LowerArm.RotationFromV()) * distance);
        }
        public float CalculateMaxHP()
        {
            return UpperArm.MaxHP + LowerArm.MaxHP + UpperClaw.MaxHP + LowerClaw.MaxHP;
        }

        public float CalculateHP()
        {
            return UpperArm.Health + LowerArm.Health + UpperClaw.Health + LowerClaw.Health;
        }
        public void Update()
        {
            if (Owner.LockArmPositions)
            {
                int expandedi = LegIndex * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                //Position = Owner.Position + Utilities.RotateVectorClockwise(new Vector2(expandedi * Owner.Texture.Width / 1.4f * factorToMoveArms, Owner.Texture.Height / 2.54f * factorToMoveArms), Owner.Rotation);
                Position = Owner.CalculateArmPostions(expandedi);
            }
            
            Position = Position + Velocity;

            if (!DeathAnimation)
            {
                UpperArm.Position = Position;

                LowerArm.Position = UpperArm.CalculateEnd();

                LowerClaw.Position = LowerArm.CalculateEnd();

                if (UpperClaw.LegIndex == 0)
                {
                    UpperClaw.Position = LowerArm.CalculateEnd();
                    UpperClaw.Position = UpperClaw.Position + Utilities.RotateVectorClockwise(new Vector2(UpperClaw.Texture.Width * 1f * UpperClaw.GetSize().X, 0f), UpperClaw.Rotation);
                }
                else UpperClaw.Position = LowerArm.CalculateEnd();

                if (UpperArm.Health <= 0 && LowerArm.Health <= 0 && UpperClaw.Health <= 0 && LowerClaw.Health <= 0)
                {
                    Dead = true;
                }
            }
        }
    }
}
