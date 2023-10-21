using bullethellwhatever.BaseClasses;
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
using FMOD;
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
        private Vector2[] ArmDestinations;
        private float[] InitialRotations; // for second part of attack
        private bool DrawTargets;
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
            ArmDestinations = new Vector2[2];
            InitialRotations = new float[2];
            DrawTargets = false;
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

            if (time > leftArmMoveTime + 1 + delayBeforeLeftArmShove + leftArmShoveTime + shortDelayBeforePush)
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

            if (time > rightArmPushTime && Leg(1).LowerClaw.Hitbox.Intersects(Owner.Hitbox) && !PushOccured)
            {
                Owner.Velocity = Leg(1).Velocity; // push boss HARD
                Leg(1).Velocity = Leg(1).Velocity * -0.1f; // recoil slightly
                PushOccured = true;
            }

            // ---- both arms code ----

            int timeAfterPushToBeginFiring = 40;

            if (time > timeAfterPushToBeginFiring + rightArmPushTime)
            {               
                int moveWaitTime = 30;
                int travelTime = 30;
                int waitBeforeRayTelegraph = 20;
                int waitAfterRay = 60;

                int cycleTotalTime = moveWaitTime + travelTime + waitBeforeRayTelegraph + waitAfterRay;

                int localTime = time - (timeAfterPushToBeginFiring + rightArmPushTime); // cycles start on 0

                for (int i = 0; i < 2; i++)
                {
                    int timeThroughCycle = localTime % cycleTotalTime;

                    if (timeThroughCycle == 0)
                    {
                        Leg(i).Velocity = Vector2.Zero;

                        ArmDestinations[i] = new Vector2(Utilities.RandomFloat(0, ScreenWidth), Utilities.RandomFloat(0, ScreenHeight)); // pick a random spot

                        InitialRotations[i] = Leg(i).UpperArm.RotationFromV();

                        TelegraphLine t = new TelegraphLine(Utilities.VectorToAngle(ArmDestinations[i] - Leg(i).Position), 0, 0, Leg(i).UpperArm.Texture.Width, (Leg(i).Position - ArmDestinations[i]).Length(), moveWaitTime, Leg(i).Position, Color.White, "box", Leg(i).UpperArm, true);

                        DrawTargets = true;

                        t.ChangeShader("OutlineTelegraphShader");
                    }

                    if (timeThroughCycle > 0 && timeThroughCycle < moveWaitTime)
                    {
                        Leg(i).PointLegInDirection(MathHelper.Lerp(InitialRotations[i], Utilities.VectorToAngle(ArmDestinations[i] - Leg(i).Position), (float)timeThroughCycle / moveWaitTime));
                    }

                    if (timeThroughCycle == moveWaitTime)
                    {
                        DrawTargets = false;

                        Leg(i).Velocity = (ArmDestinations[i] - Leg(i).Position) / travelTime;
                    }

                    if (timeThroughCycle > moveWaitTime + travelTime && timeThroughCycle < moveWaitTime + travelTime + waitBeforeRayTelegraph)
                    {
                        Leg(i).Velocity = Leg(i).Velocity * 0.97f;
                    }

                    if (timeThroughCycle == moveWaitTime + travelTime + waitBeforeRayTelegraph)
                    {
                        Leg(i).Velocity = Vector2.Zero;

                        float angleToPlayer = Utilities.VectorToAngle(player.Position - Leg(i).Position);

                        Leg(i).PointLegInDirection(angleToPlayer);

                        TelegraphLine t = new TelegraphLine(angleToPlayer, 0, 0, Leg(i).UpperArm.Texture.Width * 2.5f, 4000, 30, Leg(i).Position, Color.White, "box", Leg(i).UpperArm, false);

                        Deathray toSpawn = new Deathray().CreateDeathray(Leg(i).Position, angleToPlayer, 1f, 20, "box", t.Width, t.Length, 0, 0, true, Color.Red, "DeathrayShader2", t.Owner);

                        t.SpawnDeathrayOnDeath(toSpawn);
                    }

                    Leg(i).Position = Utilities.ClampWithinScreen(Leg(i).Position);
                }
            }
            // ---- body code ----

            if (time > leftArmMoveTime + 1 + delayBeforeLeftArmShove + leftArmShoveTime + shortDelayBeforePush)
            {
                CrabSpinSpeed = MathHelper.Clamp(CrabSpinSpeed, -PI / 10f, 0f); // spinning counterclockwise remember
                Owner.Rotation = Owner.Rotation + CrabSpinSpeed;
            }

            //if (time < spinUpTime)
            //{
            //    Owner.Rotation = Owner.Rotation + (PI / 720);
            //}

            HandleBounces();
        }

        public override void ExtraDraw(SpriteBatch s)
        {
            if (DrawTargets)
            {
                for (int i = 0; i < 2; i++)
                {
                    Drawing.BetterDraw(Assets["TargetReticle"], ArmDestinations[i], null, Color.White, 0, Vector2.One, SpriteEffects.None, 1);
                }
            }
        }
    }
}
