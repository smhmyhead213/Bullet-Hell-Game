﻿using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Bosses.CrabBoss.Projectiles;
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class SpinningAtPlayer : CrabBossAttack
    {
        private float LeftArmInitialRotation;
        private float RightArmInitialRotation;
        private int PushContactTime;
        private float CrabSpinSpeed;
        private bool PushOccured;
        private float[] LaserFireAngles;
        public SpinningAtPlayer(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            LeftArmInitialRotation = 0;
            RightArmInitialRotation = 0;
            PushContactTime = 0;
            CrabSpinSpeed = 0;
            PushOccured = false;
            LaserFireAngles = new float[2];
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer;

            int leftArmMoveTime = 20;
            int leftArmShoveTime = 20;
            int delayBeforeLeftArmShove = 10;
            int shortDelayBeforePush = 10;
            int delayAfterSpinStartBeforePush = 50;

            int rightArmPushTime = leftArmMoveTime + leftArmShoveTime + delayBeforeLeftArmShove + shortDelayBeforePush + delayAfterSpinStartBeforePush;

            // ---- left arm code ----

            if (time == 1)
            {
                Owner.Velocity = Vector2.Zero;

                LeftArmInitialRotation = Leg(0).UpperArm.RotationFromV();
                RightArmInitialRotation = Leg(1).UpperArm.RotationFromV();

                Vector2 offsetFromCentre = new Vector2(Owner.Texture.Width / 2f * Owner.GetSize().X, -Owner.Texture.Height / 2f * Owner.GetSize().Y);

                Vector2 leftArmPositionToMoveTo = Owner.Position + offsetFromCentre;

                Vector2 rightArmPositionToMoveTo = Owner.Position + (400f * -Utilities.SafeNormalise(player.Position - Owner.Position));

                Leg(0).Velocity = (leftArmPositionToMoveTo - Leg(0).Position) / leftArmMoveTime;
                Leg(1).Velocity = (rightArmPositionToMoveTo - Leg(1).Position) / leftArmMoveTime;
            }

            if (time < leftArmMoveTime + 1)
            {
                float leftArmTargetRotation = PI * 1.5f;

                Leg(0).PointLegInDirection(MathHelper.Lerp(LeftArmInitialRotation, leftArmTargetRotation, (float)time / leftArmMoveTime));
            }

            if (time == leftArmMoveTime + 1) // when it gets to boss
            {
                Leg(0).Velocity = Vector2.Zero;
                Leg(1).Velocity = Vector2.Zero;
            }

            if (time == leftArmMoveTime + 1 + delayBeforeLeftArmShove) // pull back
            {
                Leg(0).Velocity = 5f * Vector2.UnitX;
            }

            if (time == leftArmMoveTime + 1 + delayBeforeLeftArmShove + leftArmShoveTime + shortDelayBeforePush) // push in
            {
                Leg(0).Velocity = -25f * Vector2.UnitX;
            }

            if (Leg(0).LowerClaw.Hitbox.Intersects(Owner.Hitbox) && time > leftArmMoveTime + 1 + delayBeforeLeftArmShove + leftArmShoveTime + shortDelayBeforePush) // spin up while being pushed
            {
                PushContactTime++;

                CrabSpinSpeed = CrabSpinSpeed - (PushContactTime * PI / 300);
            }

            int timeAfterPushToMoveIntoPlace = 40;

            if (time > leftArmMoveTime + 1 + delayBeforeLeftArmShove + leftArmShoveTime + shortDelayBeforePush && time < rightArmPushTime + timeAfterPushToMoveIntoPlace) // please just slow down man
            {
                Leg(0).Velocity = Leg(0).Velocity * 0.95f;
            }

            // ---- right arm code ----

            if (time < leftArmMoveTime + 1)
            {
                float rightArmTargetRotation = Utilities.VectorToAngle(player.Position - Owner.Position);

                Leg(1).PointLegInDirection(MathHelper.Lerp(RightArmInitialRotation, rightArmTargetRotation, (float)time / leftArmMoveTime));

                Leg(1).Position = Utilities.ClampWithinScreen(Leg(1).Position); // stay inside screen
            }

            int rightArmPullBackTime = 30;

            if (time == rightArmPushTime - rightArmPullBackTime)
            {
                Leg(1).Velocity = 5f * Utilities.SafeNormalise(Leg(1).Position - Owner.Position);
            }

            if (time == rightArmPushTime)
            {
                Vector2 halfOfBossHeight = Utilities.RotateVectorClockwise(new Vector2(0, -Owner.Texture.Height / 2f * Owner.GetSize().Y), Owner.Rotation); // ensure we hit the back of the boss instead of its centre

                Leg(1).Velocity = -25f * Utilities.SafeNormalise(Leg(1).Position - Owner.Position + halfOfBossHeight);
            }

            if (time > rightArmPushTime && Leg(1).LowerClaw.Hitbox.Intersects(Owner.Hitbox) && !PushOccured && time < rightArmPushTime + timeAfterPushToMoveIntoPlace)
            {
                Owner.Velocity = Leg(1).Velocity; // push boss HARD
                Leg(1).Velocity = Leg(1).Velocity * -0.1f; // recoil slightly
                PushOccured = true;
            }

            // ---- both arms code ----
       
            int movementTime = 40;
            int timeAfterPushToBeginFiring = timeAfterPushToMoveIntoPlace + movementTime;
            float distanceFromPlayer = 450f;

            if (time == rightArmPushTime + timeAfterPushToMoveIntoPlace)
            {
                
            }

            if (time > rightArmPushTime + timeAfterPushToMoveIntoPlace && time < timeAfterPushToBeginFiring + rightArmPushTime)
            {

            }
            
            if (time > timeAfterPushToBeginFiring + rightArmPushTime)
            {
                int rayTeleTime = 90;
                int rayTeleDuration = 50;
                int rayDuration = 60;

                int cycleTotalTime = rayTeleTime + rayTeleDuration + rayDuration;

                int localTime = time - (timeAfterPushToBeginFiring + rightArmPushTime); // cycles start on 0

                for (int i = 0; i < 2; i++)
                {
                    int timeThroughCycle = localTime % cycleTotalTime;

                    int horizDistance = 200;

                    Vector2 targetPos = player.Position + new Vector2(horizDistance - (i * 2f * horizDistance), -500);                    
                }
            }
            //----body code----

            if (time > leftArmMoveTime + 1 + delayBeforeLeftArmShove + leftArmShoveTime + shortDelayBeforePush)
            {
                CrabSpinSpeed = MathHelper.Clamp(CrabSpinSpeed, -PI / 10f, 0f); // spinning counterclockwise remember
                Owner.Rotation = Owner.Rotation + CrabSpinSpeed;
            }

            if (time > timeAfterPushToBeginFiring + rightArmPushTime && time % 50 == 0)
            {
                ExplodingProjectile p = new ExplodingProjectile(15, 120, 0, true, false, true);

                p.Spawn(Owner.Position, Utilities.SafeNormalise(Owner.Velocity), 1f, 1, "box", 0, Vector2.One * 1.5f, Owner, true, Color.Red, true, false);
            }

            HandleBounces();
        }

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
