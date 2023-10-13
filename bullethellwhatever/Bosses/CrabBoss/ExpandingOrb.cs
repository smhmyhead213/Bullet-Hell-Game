using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Bosses.CrabBoss.Projectiles;
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace bullethellwhatever.Bosses.CrabBoss
{
    public class ExpandingOrb : CrabBossAttack
    {
        public int OrbExpansionTime;
        public int CentreMovingTime;
        public int TimeToStartOpeningHands;
        public int TimeToStopOpeningHands;
        public int TimeToStopClosingHands;
        public int TimeToThrowOrb;
        public int ProjectilesPerRing;
        public int TimeToWaitBeforeOrbThrow;
        public int ThrowAnimationDuration;
        public int TimeToWaitAfterThrow;
        public int OrbBounceAroundTime;
        public float UpperArmRotationBeforeThrow;
        public float LowerArmRotationBeforeThrow;
        public BigMassiveOrb Orb;
        public ExpandingOrb(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            CrabOwner.Velocity = Vector2.Zero;
            OrbExpansionTime = 300;
            CentreMovingTime = 30;

            TimeToStopClosingHands = 90;
            TimeToStartOpeningHands = 150;
            TimeToStopOpeningHands = 290;
            TimeToThrowOrb = 700;
            TimeToWaitBeforeOrbThrow = 120;
            ThrowAnimationDuration = 5;
            ProjectilesPerRing = 40;
            TimeToWaitAfterThrow = 400;
            OrbBounceAroundTime = 600;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer % (TimeToThrowOrb + ThrowAnimationDuration + TimeToWaitAfterThrow + OrbBounceAroundTime);

            CrabOwner.SetBoosters(false);

            CrabOwner.FacePlayer();

            //Owner.Rotation = Owner.Rotation + (AITimer / 60000f);

            //Owner.Velocity = 3f * Sin(AITimer / 40f) * -Vector2.UnitY * 2.5f;

            if (time < CentreMovingTime)
            {
                Orb = new BigMassiveOrb(0.06f, TimeToThrowOrb - TimeToStartOpeningHands + TimeToWaitAfterThrow + OrbBounceAroundTime);
                CrabOwner.ResetArmRotations();
                Owner.DealDamage = false;
                MoveToPoint(Utilities.CentreOfScreen(), time, CentreMovingTime);
            }

            if (time == CentreMovingTime)
            {
                Owner.Velocity = Vector2.Zero;
            }

            else Owner.DealDamage = true;

            float distanceBetweenHands = Utilities.DistanceBetweenVectors(Leg(0).Position, Leg(1).Position);

            float handClaspingTime = TimeToStopClosingHands - CentreMovingTime;

            float angleToRotate = Acos(distanceBetweenHands / 2 / Leg(0).Length()); // assume that both arms are the same length, if not this needs fixed

            if (time > CentreMovingTime && time < TimeToStopClosingHands) // put hands together
            {
                Leg(0).UpperArm.Rotate(-angleToRotate / handClaspingTime);
                Leg(0).LowerArm.Rotate(-angleToRotate / handClaspingTime);
                Leg(1).UpperArm.Rotate(angleToRotate / handClaspingTime);
                Leg(1).LowerArm.Rotate(angleToRotate / handClaspingTime);
            }

            if (time == TimeToStartOpeningHands)
            {
                Orb.Spawn(CrabOwner.Position + Utilities.RotateVectorClockwise(new Vector2(0, 100), CrabOwner.Rotation), Vector2.Zero, 1f, 1, "box", 0f, Vector2.One, Owner, true, Color.Red, false, false);
            }

            if (time > TimeToStartOpeningHands && time < TimeToThrowOrb)
            {
                Orb.Position = CrabOwner.Position + Utilities.RotateVectorClockwise(new Vector2(0, 100), CrabOwner.Rotation); // keep orb in front of boss
            }

            if (time > TimeToStartOpeningHands && time < TimeToStopOpeningHands)
            {
                if (time < TimeToStartOpeningHands + (TimeToStopOpeningHands - TimeToStartOpeningHands) / 2) // if we are halfway through this if block
                {
                    Leg(0).UpperArm.Rotate(angleToRotate * 2f / handClaspingTime);
                    Leg(0).LowerArm.Rotate(angleToRotate / 2f / handClaspingTime);
                    Leg(1).UpperArm.Rotate(-angleToRotate * 2f / handClaspingTime);
                    Leg(1).LowerArm.Rotate(-angleToRotate / 2f / handClaspingTime);
                }
            }

            if (time == TimeToStopOpeningHands)
            {
                Orb.SetExpanding(false);
                UpperArmRotationBeforeThrow = angleToRotate;
                LowerArmRotationBeforeThrow = angleToRotate;
            }

            if (time > TimeToStopOpeningHands && time < TimeToThrowOrb - TimeToWaitBeforeOrbThrow)
            {
                if (time % 30 == 0)
                {
                    for (int i = 0; i < ProjectilesPerRing; i++)
                    {
                        ExplodingProjectileFragment projectile = new ExplodingProjectileFragment();

                        float offset = Owner.Rotation;

                        projectile.Spawn(Orb.Position, 3f * Utilities.RotateVectorClockwise(Utilities.SafeNormalise(-Vector2.UnitY, Vector2.Zero), (PI * 2 / ProjectilesPerRing * i) + offset),
                            1f, 1, "box", 1f, Vector2.One, Owner, true, Color.Red, false, false);
                    }

                }
            }

            if (time >= TimeToThrowOrb && time < TimeToThrowOrb + ThrowAnimationDuration)
            {
                // numbers within brackets need to be the same as the ones at the hand opening
                Leg(0).UpperArm.Rotate(-angleToRotate * 2f / ThrowAnimationDuration);
                Leg(0).LowerArm.Rotate(-angleToRotate / 2f / ThrowAnimationDuration);
                Leg(1).UpperArm.Rotate(angleToRotate * 2f / ThrowAnimationDuration);
                Leg(1).LowerArm.Rotate(angleToRotate / 2f / ThrowAnimationDuration);
            }

            if (time == TimeToThrowOrb + (ThrowAnimationDuration / 2)) // let go halfway through the animation
            {
                Orb.Velocity = 17f * Utilities.SafeNormalise(player.Position - Owner.Position);
                Owner.Velocity = -Orb.Velocity * 0.7f; // recoil, and as funny as not dampening it is i cant have him yeet himself out the area
            }

            int timeToDecelerate = 40;

            if (time >= TimeToThrowOrb + (ThrowAnimationDuration / 2) && time < TimeToThrowOrb + ThrowAnimationDuration + TimeToWaitAfterThrow - timeToDecelerate) // spend a second decelerating
            {
                Owner.Velocity = Owner.Velocity * 0.98f;
            }

            if (time == TimeToThrowOrb + ThrowAnimationDuration + TimeToWaitAfterThrow - timeToDecelerate)
            {
                Owner.Velocity = Vector2.Zero;
            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {
           
        }
    }
}
