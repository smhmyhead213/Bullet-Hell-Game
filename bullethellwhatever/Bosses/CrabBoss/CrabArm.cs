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

        public void PointLegInDirection(float angle)
        {
            UpperArm.Rotate(-UpperArm.Rotation + angle + PI);
            //LowerArm.Rotate(-LowerArm.Rotation + angle + PI);
            //UpperClaw.Rotate(-UpperClaw.Rotation + angle + PI);
            //LowerClaw.Rotate(-LowerClaw.Rotation + angle + PI);
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
            // decide how far to stretch arms. the length includes the claw but the following claculations ignore it, this will be the source of any issues where the claws lie past the target position.
            float upperArmLength = UpperArm.Length();
            float lowerArmLength = LowerArm.Length();

            float lengthOfLeg = upperArmLength + lowerArmLength;

            // decide on a target. if the target it out of reach, choose a new target in the same direction that's reachable
            
            // an inaccuracy arises from the subtraction taking place here
            Vector2 direction = Utilities.SafeNormalise(targetPosition - Position);

            if (Utilities.DistanceBetweenVectors(Position, targetPosition) > lengthOfLeg)
            {
                // get the direction vector               
                targetPosition = Position + lengthOfLeg * direction;
            }

            // the distance between the start of the arm and the target
            float distance = Utilities.DistanceBetweenVectors(Position, targetPosition);

            if (distance > lengthOfLeg)
            {
                distance = lengthOfLeg;
            }

            // check that triangle inequality is not violated

            if (upperArmLength - lowerArmLength > distance)
            {
                distance = upperArmLength - lowerArmLength;
            }

            // cosine rule
            float upperArmSquared = Pow(upperArmLength, 2);
            float lowerArmSquared = Pow(lowerArmLength, 2);
            float distanceSquared = Pow(distance, 2);

            float withinAcos = (upperArmSquared + distanceSquared - lowerArmSquared) / (2 * upperArmLength * distance);
            float upperArmAngle = withinAcos < 1.0001f && withinAcos >= 1f ? 0 : Acos(withinAcos); // prevent NaN for exactly 1 and imprecisions

            Assert(!float.IsNaN(upperArmAngle));

            int expandedi = -Utilities.ExpandedIndex(LegIndex);

            // direction of rotation should be opposite for each arm
            Vector2 elbowPos = Position + Utilities.RotateVectorClockwise(direction * upperArmLength, expandedi * upperArmAngle);

            UpperArm.PointInDirection(Utilities.VectorToAngle(elbowPos - Position));

            float lowerArmRotation = Utilities.VectorToAngle(targetPosition - elbowPos);

            // the arm seems to be touching the correct point before executing this line, but afterwards seems to be slightly off
            LowerArm.PointInDirection(lowerArmRotation);

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

        public void LerpToRestPosition(float interpolant, bool pointClaws = true, bool breakpoint = false)
        {
            //if (breakpoint)
            //{
            //    Assert(false);
            //}

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
