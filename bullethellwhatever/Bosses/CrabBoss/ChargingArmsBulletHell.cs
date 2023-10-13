using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using bullethellwhatever.MainFiles;
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
    public class ChargingArmBulletHell : CrabBossAttack
    {
        public bool Targeting;
        public float TargetRotation;
        public Vector2 TargetPosition;
        public Vector2[] ArmVelocities;
        public int DecelerationTimer;
        public ChargingArmBulletHell(int endTime) : base(endTime)
        {
            EndTime = endTime;
            ArmVelocities = new Vector2[2];
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            Targeting = false;
            TargetRotation = 0;
            DecelerationTimer = 0;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int waitAfterArmsStop = 75;
            int timeToBeginClapPrep = waitAfterArmsStop;
            int waitBeforeClap = 40;
            int clapTime = 24; // time move hands together
            int handDecelTime = 45;

            int armTime = Owner.AITimer % (timeToBeginClapPrep + waitBeforeClap + clapTime + handDecelTime);
            int bodyTime = Owner.AITimer;

            // ------------- code for arms -----------------
            if (armTime == timeToBeginClapPrep)
            {
                Targeting = true;
                TargetPosition = player.Position;
                DecelerationTimer = 0;
            }

            if (armTime >= timeToBeginClapPrep && armTime < timeToBeginClapPrep + waitBeforeClap) // wait before arms move
            {
                TargetRotation = TargetRotation + (PI / 6f);

                for (int i = 0; i < 2; i++)
                {
                    int otherArm = (int)Abs(i - 1);

                    if (Utilities.DistanceBetweenVectors(Leg(0).Position, Leg(1).Position) < 300)
                    {
                        Leg(i).Velocity = -(Leg(otherArm).Position - Leg(i).Position) * 0.02f; // move arms away from each other so they dont overlap
                    }

                    else Leg(i).Velocity = Leg(i).Velocity * 0.97f;
                    
                    Vector2 vectorToTarget = TargetPosition - Leg(i).LowerClaw.Position;

                    Leg(i).PointLegInDirection(Utilities.VectorToAngle(vectorToTarget));

                    TelegraphLine t = new TelegraphLine(Utilities.VectorToAngle(vectorToTarget), 0, 0, Leg(i).UpperArm.Texture.Width, 3000, 1, Leg(i).LowerClaw.Position, Color.White, "box", Owner, false);

                    t.ChangeShader("OutlineTelegraphShader");

                }
            }

            if (armTime == timeToBeginClapPrep + waitBeforeClap) // time at which arms launch
            {
                Targeting = false;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 vectorToTarget = TargetPosition - Leg(i).LowerClaw.Position;

                    Leg(i).Velocity = vectorToTarget / clapTime * 0.75f; // travel the distance needed for the fist to touch the target

                    ArmVelocities[i] = Leg(i).Velocity;

                    Leg(i).PointLegInDirection(Utilities.VectorToAngle(vectorToTarget));
                }
            }

            int timeToStartDecelerating = timeToBeginClapPrep + waitBeforeClap + clapTime;

            if (armTime > timeToStartDecelerating)
            {
                for (int i = 0; i < 2; i++)
                {
                    Leg(i).Velocity = Leg(i).Velocity - (Leg(i).Velocity / (handDecelTime - DecelerationTimer));
                }

                DecelerationTimer++;
            }

            if (armTime == timeToStartDecelerating + handDecelTime)
            {
                for (int i = 0; i < 2; i++)
                {
                    Leg(i).Velocity = Leg(i).Velocity * 0.05f;
                }
            }

            // ------------------------------------
            // -------- code for body -------------

            int moveToCentreTime = 30;

            if (bodyTime < moveToCentreTime)
            {
                MoveToPoint(Utilities.CentreOfScreen(), bodyTime, moveToCentreTime);
            }

            int spinUpTime = 45;

            if (bodyTime > moveToCentreTime && bodyTime < moveToCentreTime + spinUpTime)
            {
                Owner.Rotation = Owner.Rotation - (PI / 1080 * (spinUpTime - (bodyTime - moveToCentreTime)));
            }

            if (bodyTime > moveToCentreTime + spinUpTime)
            {
                float maxAngularVelocity = PI / 15;

                float angularVelocity = PI / 60 * (bodyTime - (moveToCentreTime + spinUpTime));

                if (angularVelocity > maxAngularVelocity)
                {
                    Owner.Rotation = Owner.Rotation + maxAngularVelocity; // dont exceed a maximum angular speed so we dont get too crazy
                }

                else Owner.Rotation = Owner.Rotation + angularVelocity;
            }
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

