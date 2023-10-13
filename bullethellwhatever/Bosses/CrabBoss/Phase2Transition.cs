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
        public PhaseTwoTransition(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            SlowDownTime = 120;
            MoveToCentreEndTime = 180;
            Targeting = false;
            TargetRotation = 0;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer % 1000;

            if (time == 1) // i dont know why it has to be 1 not 0 it really annoys me but its late at night and i dont want to see what the problem is
            {
                foreach (Projectile p in activeProjectiles)
                {
                    p.Die(); //clear all projectiles
                }

                CrabOwner.ResetArmRotations();
            }

            if (time < SlowDownTime)
            {
                Owner.Velocity = Owner.Velocity * 0.98f; 
            }
            int timeToWaitBeforeArmDetach = 90;

            if (time == SlowDownTime)
            {
                Owner.Velocity = Vector2.Zero;
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
            }

            if (time == timeToBeginClapPrep + waitBeforeClap) // time at which arms launch
            {
                Targeting = false;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 vectorToTarget = TargetPosition - Leg(i).LowerClaw.Position;
                    Leg(i).Velocity = (vectorToTarget) / clapTime; // travel the distance needed for the fist to touch the target
                    Leg(i).UpperArm.Rotate(Utilities.VectorToAngle(vectorToTarget) - angleToRotate); // upper arms are already rotated
                    Leg(i).LowerArm.Rotate(Utilities.VectorToAngle(vectorToTarget));
                }
            }

            if (time == timeToBeginClapPrep + waitBeforeClap + clapTime) // when the arms hit the target
            {
                for (int i = 0; i < 2; i++)
                {
                    Leg(i).Velocity = Leg(i).Velocity * 0.5f;
                }
            }

            if (time > timeToBeginClapPrep + waitBeforeClap + clapTime) // after impact
            {
                for (int i = 0; i < 2; i++)
                {
                    Leg(i).Velocity = Leg(i).Velocity * 0.98f;
                }
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
