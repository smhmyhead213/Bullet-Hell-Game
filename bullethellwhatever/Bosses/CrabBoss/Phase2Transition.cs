using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class PhaseTwoTransition : CrabBossAttack
    {
        public int SlowDownTime;
        public int MoveToCentreEndTime;
        public float RotationToUndo;
        public float[] ArmRotationsToUndo;
        public bool Targeting;
        public float TargetRotation;
        public Vector2 TargetPosition;
        public Vector2 InitialSize;
        public float InitialDepth;
        public float LeftLegDepth;
        public float RightLegDepth;
        public Vector2[] LeftLegSizes;
        public Vector2[] RightLegSizes;

        public PhaseTwoTransition(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            SlowDownTime = 45;
            MoveToCentreEndTime = 90;
            Targeting = false;
            TargetRotation = 0;
            InitialSize = Vector2.Zero;
            InitialDepth = 0;

            LeftLegDepth = 0;
            RightLegDepth = 0;

            LeftLegSizes = new Vector2[4];
            RightLegSizes = new Vector2[4];
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer % 1000;

            if (time == 1) // i dont know why it has to be 1 not 0 it really annoys me but its late at night and i dont want to see what the problem is
            {
                CrabOwner.LockArmPositions = true;

                foreach (Projectile p in activeProjectiles)
                {
                    p.Die(); //clear all projectiles
                }

                InitialSize = Owner.Size;

                InitialDepth = Owner.Depth;

                Leg(0).ContactDamage(false);
                Leg(1).ContactDamage(false);

                Owner.DealDamage = false;

                LeftLegSizes[0] = Leg(0).UpperArm.Size; // help me
                LeftLegSizes[1] = Leg(0).LowerArm.Size;
                LeftLegSizes[2] = Leg(0).UpperClaw.Size;
                LeftLegSizes[3] = Leg(0).LowerClaw.Size;

                RightLegSizes[0] = Leg(1).UpperArm.Size;
                RightLegSizes[1] = Leg(1).LowerArm.Size;
                RightLegSizes[2] = Leg(1).UpperClaw.Size;
                RightLegSizes[3] = Leg(1).LowerClaw.Size;

                CrabOwner.SetBoosters(false);

                CrabOwner.ResetArmRotations();

                LeftLegDepth = Leg(0).UpperArm.Depth; // change this if for whatever reason you decide different appendages have different depths
                RightLegDepth = Leg(1).UpperArm.Depth;
            }

            if (time < SlowDownTime + 1)
            {
                float interpolant = time / (float)SlowDownTime;

                Owner.Velocity = Owner.Velocity * 0.98f; 
                Owner.Depth = MathHelper.Lerp(InitialDepth, 0, interpolant);
                Owner.Size = Vector2.Lerp(InitialSize, Vector2.One * 1.5f, interpolant);

                for (int i = 0; i < 2; i++)
                {
                    Leg(i).SetAllSizes(Vector2.Lerp(InitialSize, Vector2.One, interpolant));
                    Leg(i).SetAllDepths(MathHelper.Lerp(InitialDepth, 0, interpolant));
                }

                Leg(0).UpperArm.Size = Vector2.Lerp(LeftLegSizes[0], Vector2.One, interpolant);
                Leg(0).LowerArm.Size = Vector2.Lerp(LeftLegSizes[1], Vector2.One, interpolant);
                Leg(0).UpperClaw.Size = Vector2.Lerp(LeftLegSizes[2], Vector2.One, interpolant);
                Leg(0).LowerClaw.Size = Vector2.Lerp(LeftLegSizes[3], Vector2.One, interpolant);

                Leg(1).UpperArm.Size = Vector2.Lerp(RightLegSizes[0], Vector2.One, interpolant);
                Leg(1).LowerArm.Size = Vector2.Lerp(RightLegSizes[1], Vector2.One, interpolant);
                Leg(1).UpperClaw.Size = Vector2.Lerp(RightLegSizes[2], Vector2.One, interpolant);
                Leg(1).LowerClaw.Size = Vector2.Lerp(RightLegSizes[3], Vector2.One, interpolant);
            }

            int timeToWaitBeforeArmDetach = 90;

            if (time == SlowDownTime)
            {
                Owner.Velocity = Vector2.Zero;
                Owner.Depth = 0;

                RotationToUndo = Owner.Rotation;

                Drawing.ScreenShake(1, MoveToCentreEndTime + timeToWaitBeforeArmDetach - SlowDownTime);
            }

            float angleToRotate = PI / 4;

            if (time > SlowDownTime && time < MoveToCentreEndTime)
            {
                MoveToPoint(Utilities.CentreOfScreen(), MoveToCentreEndTime - time, MoveToCentreEndTime - SlowDownTime);
                Owner.Rotation = Owner.Rotation - (RotationToUndo / (MoveToCentreEndTime - SlowDownTime));

                Leg(0).UpperArm.Rotate(angleToRotate / (MoveToCentreEndTime - SlowDownTime));
                Leg(1).UpperArm.Rotate(-angleToRotate / (MoveToCentreEndTime - SlowDownTime));
            }

            if (time == MoveToCentreEndTime + timeToWaitBeforeArmDetach)
            {
                Drawing.ScreenShake(9, 20);

                Owner.Velocity = -Vector2.UnitY * 3f; // recoil

                CrabOwner.LockArmPositions = false; // detach

                Leg(0).ContactDamage(true);
                Leg(1).ContactDamage(true);

                Leg(0).Velocity = Utilities.RotateVectorClockwise(Vector2.UnitY * 15f, angleToRotate);
                Leg(1).Velocity = Utilities.RotateVectorCounterClockwise(Vector2.UnitY * 15f, angleToRotate);
            }

            int decelerateTimeBoss = 60;
            int decelerateTimeArms = 90;

            if (time >= MoveToCentreEndTime + decelerateTimeArms)
            {
                Leg(0).Velocity = Leg(0).Velocity * 0.96f;
                Leg(1).Velocity = Leg(1).Velocity * 0.96f;

                if (time >= MoveToCentreEndTime + decelerateTimeBoss)
                {
                    Owner.Velocity = Owner.Velocity * 0.98f;
                }
            }

            int waitAfterArmsStop = 75;
            int timeToBeginClapPrep = MoveToCentreEndTime + decelerateTimeArms + waitAfterArmsStop;
            int waitBeforeClap = 40;
            int clapTime = 24; // time move hands together

            if (time == timeToBeginClapPrep)
            {
                Targeting = true;
                TargetPosition = player.Position;
            }

            if (time >= timeToBeginClapPrep && time < timeToBeginClapPrep + waitBeforeClap) // wait before arms move
            {
                TargetRotation = TargetRotation + (PI / 6f);

                for (int i = 0; i < 2; i++)
                {
                    Vector2 vectorToTarget = TargetPosition - Leg(i).LowerClaw.Position;

                    Leg(i).PointLegInDirection(Utilities.VectorToAngle(vectorToTarget));
                }
            }

            if (time == timeToBeginClapPrep + waitBeforeClap) // time at which arms launch
            {
                Targeting = false;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 vectorToTarget = TargetPosition - Leg(i).LowerClaw.Position;

                    Leg(i).Velocity = (vectorToTarget) / clapTime * 1.3f; // travel the distance needed for the fist to touch the target, plus a little more

                    Leg(i).PointLegInDirection(Utilities.VectorToAngle(vectorToTarget));
                }
            }

            if (time > timeToBeginClapPrep + waitBeforeClap + clapTime) // after impact
            {
                for (int i = 0; i < 2; i++)
                {
                    Leg(i).Velocity = Leg(i).Velocity * 0.98f;
                }
            }

            if (time == timeToBeginClapPrep + waitBeforeClap + clapTime + 30)
            {
                Leg(0).Velocity = Vector2.Zero;
                Leg(1).Velocity = Vector2.Zero;

                CrabOwner.ReplaceAttackPattern(CrabOwner.PhaseTwoAttacks); // start phase 2
            }

            HandleBounces();
        }

        public override void ExtraDraw(SpriteBatch s)
        {
            if (Targeting)
            {
                Drawing.BetterDraw(Assets["TargetReticle"], TargetPosition, null, Color.White, TargetRotation, Vector2.One, SpriteEffects.None, 1);
            }
        }
    }
}
