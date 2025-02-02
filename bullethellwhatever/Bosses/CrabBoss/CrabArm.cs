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

        public CrabBossAppendage[] LegParts;
        public CrabBossAppendage UpperArm => LegParts[0];
        public CrabBossAppendage LowerArm => LegParts[1];
        public CrabBossAppendage UpperClaw => LegParts[2];
        public CrabBossAppendage LowerClaw => LegParts[3];

        public bool HorizontalFlip;

        public bool DeathAnimation;

        public CrabArm(Vector2 position, CrabBoss owner, int legIndex, float scale = 1)
        {
            LegParts = new CrabBossAppendage[4];
            Owner = owner;

            LegParts[0] = new CrabBossAppendage(Owner, this, AppendageType.UpperArm, legIndex, scale);
            UpperArm.SetMaxHP(50f, true);
            UpperArm.BehindThis = Owner;

            LegParts[1] = new CrabBossAppendage(Owner, this, AppendageType.LowerArm, legIndex, scale);
            LowerArm.SetMaxHP(35f, true);
            LowerArm.BehindThis = UpperArm;

            LegParts[2] = new CrabBossAppendage(Owner, this, AppendageType.UpperClaw, legIndex, scale);
            UpperClaw.SetMaxHP(20f, true);
            UpperClaw.BehindThis = LowerArm;

            LegParts[3] = new CrabBossAppendage(Owner, this, AppendageType.LowerClaw, legIndex, scale);
            LowerClaw.SetMaxHP(20f, true);
            LowerClaw.BehindThis = LowerArm;

            HorizontalFlip = false;

            LegIndex = legIndex;

            DeathAnimation = false;

            Position = position;
            UpperArm.Position = Position;
            LowerArm.Position = UpperArm.CalculateEnd();
            UpperClaw.Position = LowerArm.CalculateEnd();
            LowerClaw.Position = LowerArm.CalculateEnd();
        }
        public float Length()
        {
            return UpperArm.Length() + LowerArm.Length() + UpperClaw.Length();
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
            UpperArm.ContactDamage = on;
            LowerArm.ContactDamage = on;
            UpperClaw.ContactDamage = on;
            LowerClaw.ContactDamage = on;
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
            float width = 5f; // awful code
            UpperArm.Hitbox.Draw(width);
            LowerArm.Hitbox.Draw(width);
            UpperClaw.Hitbox.Draw(width);
            LowerClaw.Hitbox.Draw(width);
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

        /// <summary>
        /// Solves IK to make the arm attempt to touch a certain point. If the point is longer than the arm's length, attempts to point the arm fully stretched in the direction of the target.
        /// </summary>
        /// <param name="targetPosition"></param>
        public void TouchPoint(Vector2 targetPosition)
        {
            // decide how far to stretch arms. the length includes the claw but the following claculations ignore it, this will be the source of any issues where the claws lie past the target position.
            float upperArmLength = UpperArm.Length();
            float lowerArmLength = LowerArm.Length();

            float lengthOfLeg = upperArmLength + lowerArmLength;

            // decide on a target. if the target it out of reach, choose a new target in the same direction that's reachable
            
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

            float upperArmAngle = Acos((upperArmSquared + distanceSquared - lowerArmSquared) / (2 * upperArmLength * distance));

            Debug.Assert(!float.IsNaN(upperArmAngle));

            int expandedi = -Utilities.ExpandedIndex(LegIndex);

            // direction of rotation should be opposite for each arm
            Vector2 elbowPos = Position + Utilities.RotateVectorClockwise(direction * upperArmLength, expandedi * upperArmAngle);

            UpperArm.PointInDirection(Utilities.VectorToAngle(elbowPos - Position));

            float lowerArmRotation = Utilities.VectorToAngle(targetPosition - elbowPos);

            LowerArm.PointInDirection(lowerArmRotation);
            LowerClaw.PointInDirection(lowerArmRotation);
            UpperClaw.PointInDirection(lowerArmRotation);
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
            }
        }
    }
}
