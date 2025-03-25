using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Bosses.CrabBoss.Attacks;
using bullethellwhatever.Bosses.EyeBoss;
using bullethellwhatever.DrawCode;
using bullethellwhatever.DrawCode.Particles;
using bullethellwhatever.Projectiles;

using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.UtilitySystems;
using Microsoft.VisualBasic.Logging;
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
    public class CrabPunch : CrabBossAttack
    {
        public CrabPunch(CrabBoss owner) : base(owner)
        {

        }
        public override void Execute(int AITimer)
        {
            int pullBackArmTime = 23; // 20
            int swingDuration = 5;

            ref float swingTime = ref ExtraData[1]; // the farthest point in the attack the swing can happen. will be set to a smaller number if the boss is close to the player.
            int chargeTrackingTime = 5;
            int startSpawningParticlesTime = 5;
            int accelerateTime = 10; // 25
            int maxChargeTime = 36; // 36

            float angleToPullBackArm = PI / 2f;
            float angleToSwingThrough = angleToPullBackArm + PI / 4f;
            float initialChargeSpeed = 6f;
            float topChargeSpeed = 50f; // 35

            int timeToDecelAfterSwing = 10; // 12
            int decelerateTime = 7; // 18

            Vector2 toPlayer = player.Position - Owner.Position;
            float angleToPlayer = Utilities.VectorToAngle(toPlayer);

            ref float initialSpeed = ref ExtraData[2];
            ref float HasSetSwingTime = ref ExtraData[0]; // if this is 0, swing has not been set. if 1, swing has been set

            if (AITimer == 0)
            {
                // clear owner ai variables
                ClearExtraData();
                Owner.ClearExtraData();
                
                swingTime = pullBackArmTime + maxChargeTime;
                HasSetSwingTime = 0f;

                if (Utilities.DistanceBetweenVectors(Arm(0).Position, player.Position) > Utilities.DistanceBetweenVectors(Arm(1).Position, player.Position))
                {
                    // set the chosenArm npc data slot to the number representing representing the closer arm
                    ChooseMainArm(1);
                }
                else
                {
                    ChooseMainArm(0);
                }
            }

            int expandedi = -Utilities.ExpandedIndex(ChosenArmIndex());

            if (AITimer <= pullBackArmTime)
            {
                // figure out what angle to swing through this frame.
                // easeoutquad
                RotateArm(ChosenArmIndex(), expandedi * angleToPullBackArm, AITimer, pullBackArmTime, EasingFunctions.EaseInQuad);

                // speed up just a little towards the player

                float interpolant = AITimer / (float)pullBackArmTime;
                Owner.Velocity = Owner.Position.DirectionToPlayer() * MathHelper.Lerp(0f, initialChargeSpeed, interpolant);
            }

            if (AITimer == pullBackArmTime)
            {
                initialSpeed = Owner.Velocity.Length();
            }

            if (AITimer >= pullBackArmTime && AITimer < pullBackArmTime + accelerateTime)
            {
                int localTime = AITimer - pullBackArmTime;
                float trackingCompletionRatio = localTime / (float)accelerateTime;
                float differenceBetweenTopAndInitialChargeSpeed = topChargeSpeed - initialSpeed;

                float chargeSpeed = MathHelper.Lerp(initialSpeed, topChargeSpeed, EasingFunctions.EaseInOutQuart(trackingCompletionRatio));

                if (localTime < chargeTrackingTime)
                {
                    Owner.Velocity = chargeSpeed * Utilities.SafeNormalise(toPlayer);
                }
                else
                {
                    Owner.Velocity = chargeSpeed * Utilities.SafeNormalise(Owner.Velocity);
                }
            }

            float swingProximity = 300f;

            bool checkingForSwing = AITimer > pullBackArmTime && HasSetSwingTime == 0 && AITimer < pullBackArmTime + maxChargeTime;
            //bool swingEnd = AITimer == pullBackArmTime + maxChargeTime && HasSetSwingTime == 0;

            if (checkingForSwing) // if the arm is fully pulled back and a swing time hasnt been chosen, the boss can choose when to punch
            {
                // if we havent swung yet, speed up
                float acceleration = 0.6f;
                Owner.Velocity = Owner.Velocity.SetLength(Owner.Velocity.Length() + acceleration);

                if (Utilities.DistanceBetweenEntities(player, Owner) < swingProximity)
                {
                    HasSetSwingTime = 1f;
                    swingTime = AITimer + 1; // swing on the next frame
                }
            }

            if (AITimer >= swingTime && AITimer < swingTime + swingDuration)
            {
                int localTime = AITimer - (int)swingTime;

                RotateArm(ChosenArmIndex(), -expandedi * angleToSwingThrough, localTime, swingDuration, EasingFunctions.EaseOutExpo);
                ChosenArm().LowerArm.Rotate(-expandedi * angleToSwingThrough / 2f / swingDuration);
            }

            if (AITimer < swingTime)
            {
                CrabOwner.Rotation = Utilities.VectorToAngle(-toPlayer);
            }
            else if (AITimer >= swingTime && AITimer < swingTime + swingDuration)
            {
                CrabOwner.Rotation += expandedi * -angleToSwingThrough / swingDuration;
            }

            if (AITimer == swingTime + timeToDecelAfterSwing)
            {
                // set initial speed to decelerate from 
                initialSpeed = Owner.Velocity.Length();
            }

            if (AITimer > swingTime + timeToDecelAfterSwing && AITimer <= swingTime + decelerateTime + timeToDecelAfterSwing)
            {
                float localTime = AITimer - swingTime - timeToDecelAfterSwing;

                float speedInterpolant = EasingFunctions.EaseInCirc((float)localTime / decelerateTime);

                Owner.Velocity = Utilities.SafeNormalise(Owner.Velocity) * MathHelper.Lerp(initialSpeed, 0, speedInterpolant);                
            }

            if (AITimer == swingTime + timeToDecelAfterSwing + decelerateTime)
            {
                End();
            }

            // spawn particles
            if (AITimer > startSpawningParticlesTime && AITimer < swingTime)
            {
                float angleVariation = 0f * PI / 36f;
                float rotation = Owner.Velocity.ToAngle() + Utilities.RandomAngle(angleVariation);
                Vector2 spawnPos = Owner.Position + new Vector2(Owner.Width() * Utilities.RandomFloat(-0.5f, 0.5f), 0f).Rotate(rotation);
                Particle p = new Particle();
                float scaleLength = Owner.Velocity.Length() / 4f;
                p.Spawn("box", spawnPos, Vector2.Zero, Vector2.Zero, new Vector2(0.25f, scaleLength), rotation, Color.White, 1f, 6);
            }

            Assert(!float.IsNaN(Owner.Rotation));
        }

        public override BossAttack PickNextAttack()
        {
            int nextAttack = Utilities.RandomInt(1, 5);
            if (nextAttack == 1 || nextAttack == 2)
                return new CrabPunchToNeutralTransition(CrabOwner);
            else
                return new CrabPunchToProjectileSpreadTransition(CrabOwner);
        }
    }

    public class CrabPunchToNeutralTransition : CrabBossAttack
    {
        public CrabPunchToNeutralTransition(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int armRotateBackToNeutralTime = 10;

            float totalSwingAngle = PI / 2f;

            // this always occurs after the crab punch so the owners extradata[0] will still be the chosen arm
            int expandedi = Utilities.ExpandedIndex(ChosenArmIndex());

            if (AITimer <= armRotateBackToNeutralTime)
            {
                float interpolant = AITimer / (float)armRotateBackToNeutralTime;

                //CrabOwner.LerpToFacePlayer();

                Vector2 toPlayer = player.Position - Owner.Position;
                float angleToPlayer = Utilities.VectorToAngle(toPlayer);

                float rotationToPlayer = Utilities.SmallestAngleTo(Owner.Rotation + PI, Utilities.AngleToPlayerFrom(Owner.Position));

                Owner.Rotation += 0.2f * rotationToPlayer;

                RotateArm(ChosenArmIndex(), -expandedi * totalSwingAngle, AITimer, armRotateBackToNeutralTime, EasingFunctions.Linear);

                ChosenArm().LerpToRestPosition(EasingFunctions.EaseOutQuad(interpolant), true, true);
            }

            if (AITimer == armRotateBackToNeutralTime)
            {
                End();
            }
        }
    }

    public class CrabPunchToProjectileSpreadTransition : CrabBossAttack
    {
        public CrabPunchToProjectileSpreadTransition(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int armRotateBackToNeutralTime = 10;

            // this always occurs after the crab punch so the owners extradata[0] will still be the chosen arm
            ref float chosenArm = ref Owner.ExtraData[0];
            int expandedi = -Utilities.ExpandedIndex((int)chosenArm);

            if (AITimer > 0 && AITimer <= armRotateBackToNeutralTime)
            {
                float interpolant = EasingFunctions.EaseOutQuad(AITimer / (float)armRotateBackToNeutralTime);

                Vector2 toPlayer = Owner.Position - player.Position;
                float angleToPlayer = Utilities.VectorToAngle(toPlayer);
                Owner.Rotation = Utilities.LerpRotation(Owner.Rotation, angleToPlayer, interpolant);
            }

            if (AITimer == armRotateBackToNeutralTime)
            {
                End();
            }
        }
        public override BossAttack PickNextAttack()
        {
            return new CrabProjectileSpread(CrabOwner);
        }
    }
}
