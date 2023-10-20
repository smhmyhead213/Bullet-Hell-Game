using System;
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

        public CrabBossUpperArm UpperArm;
        public CrabBossLowerArm LowerArm;
        public CrabBossUpperClaw UpperClaw;
        public CrabBossLowerClaw LowerClaw;

        public bool HorizontalFlip;
        public CrabLeg(Vector2 position, CrabBoss owner, int legIndex)
        {
            Owner = owner;

            UpperArm = new CrabBossUpperArm(Owner, this, "CrabUpperArm", legIndex);
            LowerArm = new CrabBossLowerArm(Owner, this, "CrabLowerArm", legIndex);
            UpperClaw = new CrabBossUpperClaw(Owner, this, "CrabUpperClaw", legIndex);
            LowerClaw = new CrabBossLowerClaw(Owner, this, "CrabLowerClaw", legIndex);

            HorizontalFlip = false;

            LegIndex = legIndex;

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

        public void SetAllDepths(float depth)
        {
            UpperArm.SetDepth(depth);
            LowerArm.SetDepth(depth);
            UpperClaw.SetDepth(depth);
            LowerClaw.SetDepth(depth);
        }
        public void DrawHitboxes()
        {
            UpperArm.Hitbox.DrawHitbox();
            LowerArm.Hitbox.DrawHitbox();
            UpperClaw.Hitbox.DrawHitbox();
            LowerClaw.Hitbox.DrawHitbox();
        }
        public void Update()
        {
            if (Owner.LockArmPositions)
            {
                int expandedi = LegIndex * 2 - 1; // i = 0, this = -1, i = 1, this = 1
                float factorToMoveArms = MathHelper.Lerp(1f, 0.1f, Owner.Depth);

                Position = Owner.Position + Utilities.RotateVectorClockwise(new Vector2(expandedi * Owner.Texture.Width / 1.4f * factorToMoveArms, Owner.Texture.Height / 2.54f * factorToMoveArms), Owner.Rotation);
            }
            
            Position = Position + Velocity;

            UpperArm.Position = Position;

            LowerArm.Position = UpperArm.CalculateEnd();

            LowerClaw.Position = LowerArm.CalculateEnd();

            if (UpperClaw.LegIndex == 0)
            {
                UpperClaw.Position = LowerArm.CalculateEnd();
                UpperClaw.Position = UpperClaw.Position + Utilities.RotateVectorClockwise(new Vector2(UpperClaw.Texture.Width * 1f * UpperClaw.DepthFactor() * UpperClaw.GetSize().X, 0f), UpperClaw.Rotation);
            }
            else UpperClaw.Position = LowerArm.CalculateEnd();

            if (UpperArm.Health <= 0 && LowerArm.Health <= 0 && UpperClaw.Health <= 0 && LowerClaw.Health <= 0)
            {
                Dead = true;
            }
        }
    }
}
