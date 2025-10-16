using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using System.Diagnostics;
using bullethellwhatever.UtilitySystems;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabArm
    {
        public Vector2 Position;
        public Vector2 Velocity;

        public CrabBoss Owner;

        public int LegIndex;

        //public bool LockPosition;

        public CrabBossAppendage[] ArmParts;

        public int UpperArmIndex = 0;
        public int LowerArmIndex = 1;
        public int UpperClawIndex = 2;
        public int LowerClawIndex = 3;
        public CrabBossAppendage UpperArm => ArmParts[UpperArmIndex];
        public CrabBossAppendage LowerArm => ArmParts[LowerArmIndex];
        public CrabBossAppendage UpperClaw => ArmParts[UpperClawIndex];
        public CrabBossAppendage LowerClaw => ArmParts[LowerClawIndex];

        public bool HorizontalFlip;

        public bool DeathAnimation;

        public CrabArm(Vector2 position, CrabBoss owner, int legIndex, float scale = 1)
        {
            ArmParts = new CrabBossAppendage[4];
            Owner = owner;

            ArmParts[0] = new CrabBossAppendage(Owner, this, AppendageType.UpperArm, legIndex, scale);
            UpperArm.SetMaxHP(50f, true);
            UpperArm.BehindThis = Owner;

            ArmParts[1] = new CrabBossAppendage(Owner, this, AppendageType.LowerArm, legIndex, scale);
            LowerArm.SetMaxHP(35f, true);
            LowerArm.BehindThis = UpperArm;

            ArmParts[2] = new CrabBossAppendage(Owner, this, AppendageType.UpperClaw, legIndex, scale);
            UpperClaw.SetMaxHP(20f, true);
            UpperClaw.BehindThis = LowerArm;

            ArmParts[3] = new CrabBossAppendage(Owner, this, AppendageType.LowerClaw, legIndex, scale);
            LowerClaw.SetMaxHP(20f, true);
            LowerClaw.BehindThis = LowerArm;

            HorizontalFlip = false;

            LegIndex = legIndex;

            DeathAnimation = false;

            Position = position;

            UpdateAppendages();
        }

        /// <summary>
        /// The IK solver does not deal with the claws. The wrist length should be used in situations where the IK solver is.
        /// </summary>
        /// <returns></returns>
        public float WristLength()
        {
            return UpperArm.Length() + LowerArm.Length();
        }
        public float Length()
        {
             return WristLength() + UpperClaw.Length();
        }

        public float OriginalWristLength()
        {
            return UpperArm.OriginalLength() + LowerArm.OriginalLength();
        }

        public float OriginalLength()
        {
            return OriginalWristLength() + UpperClaw.OriginalLength();
        }

        public void ResetRotations()
        {
            UpperArm.RotationToAdd = 0f;
            LowerArm.RotationToAdd = 0f;
            UpperClaw.RotationToAdd = 0f;
            LowerClaw.RotationToAdd = 0f;
        }

        public Vector2[] GetPartScales()
        {
            return new Vector2[] { UpperArm.Scale, LowerArm.Scale, UpperClaw.Scale, LowerClaw.Scale };
        }

        public float[] GetPartRotations()
        {
            return new float[] { UpperArm.RotationToAdd, LowerArm.RotationToAdd, UpperClaw.RotationToAdd, LowerClaw.RotationToAdd };
        }

        public void ContactDamage(bool on)
        {
            UpperArm.ContactDamage = on;
            LowerArm.ContactDamage = on;
            UpperClaw.ContactDamage = on;
            LowerClaw.ContactDamage = on;
        }
        public void RotateLeg(float angle)
        {
            UpperArm.Rotate(angle);
            //LowerArm.Rotate(angle);
            //UpperClaw.Rotate(angle);
            //LowerClaw.Rotate(angle);
        }

        public void SetAllSizes(Vector2 size)
        {
            UpperArm.Scale = size;
            LowerArm.Scale = size;
            UpperClaw.Scale = size;
            LowerClaw.Scale = size;
        }
        public void DrawHitboxes()
        {
            foreach (CrabBossAppendage appendage in ArmParts)
            {
                appendage.DrawHitbox();
            }
        }

        public void SetAllRotations(float rotation)
        {
            foreach (CrabBossAppendage appendage in ArmParts)
            {
                appendage.Rotation = rotation;
            }
        }

        public Vector2 WristPosition()
        {
            return LowerArm.CalculateEnd();
        }
        public Vector2 PositionAtDistanceFromWrist(float distance)
        {
            return LowerArm.CalculateEnd() + (Utilities.AngleToVector(LowerArm.RotationFromV()) * distance);
        }

        public Vector2 WristOffsetBy(Vector2 offset)
        {
            return WristPosition() + offset.Rotate(LowerArm.RotationFromV());
        }
        public float CalculateMaxHP()
        {
            return UpperArm.MaxHP + LowerArm.MaxHP + UpperClaw.MaxHP + LowerClaw.MaxHP;
        }

        public float CalculateHP()
        {
            return UpperArm.Health + LowerArm.Health + UpperClaw.Health + LowerClaw.Health;
        }

        /// <summary>
        /// Solves IK to make the arm attempt to touch a certain point. If the point is longer than the arm's length, attempts to point the arm fully stretched in the direction of the target.
        /// </summary>
        /// <param name="targetPosition"></param>
        public void TouchPoint(Vector2 targetPosition, bool pointClaw = true)
        {
            float[] rotations = MathsUtils.SolveTwoPartIK(Position, targetPosition, UpperArm.Length(), LowerArm.Length(), -Utilities.ExpandedIndex(LegIndex));

            UpperArm.PointInDirection(rotations[0]);
            LowerArm.PointInDirection(rotations[1]);

            float lowerArmRotation = rotations[1];

            if (pointClaw)
            {
                LowerClaw.PointInDirection(lowerArmRotation);
                UpperClaw.PointInDirection(lowerArmRotation);
            }
        }

        public float Scale()
        {
            return UpperArm.Scale.X; // to do: make scale a float
        }
        public void SetScale(float scale)
        {
            foreach (CrabBossAppendage append in ArmParts)
            {
                append.Scale = new Vector2(scale);
            }
        }
        public void LerpRotation(float startAngle, float endAngle, float interpolant)
        {
            UpperArm.LerpRotation(startAngle, endAngle, interpolant);
        }

        public void LerpToRestPosition(float interpolant, bool pointClaws = true)
        {
            TouchPoint(Vector2.LerpPrecise(WristPosition(), RestPositionEnd(), interpolant), pointClaws);
        }
        public void LerpToPoint(Vector2 point, float interpolant, bool pointClaws = true)
        {
            TouchPoint(Vector2.LerpPrecise(WristPosition(), point, interpolant), pointClaws);
        }
        public Vector2 RestPositionEnd()
        {
            return Position + Scale() * Owner.ArmRestingEnds[LegIndex].Rotate(Owner.Rotation);
        }

        public void LerpArmToRest(float progress)
        {
            LerpToRestPosition(progress, true);
        }
        public void UpdateAppendages()
        {
            UpperArm.Position = Position;
            UpperArm.Rotation = UpperArm.CalculateFinalRotation();

            LowerArm.Position = UpperArm.CalculateEnd();
            LowerArm.Rotation = LowerArm.CalculateFinalRotation();

            LowerClaw.Position = LowerArm.CalculateEnd();
            LowerClaw.Rotation = LowerClaw.CalculateFinalRotation();

            UpperClaw.Position = LowerArm.CalculateEnd();
            UpperClaw.Rotation = UpperClaw.CalculateFinalRotation();
        }

        public void UpdatePositions()
        {
            int expandedi = LegIndex * 2 - 1;
            Position = Owner.CalculateArmPostions(expandedi);
        }
    }
}
