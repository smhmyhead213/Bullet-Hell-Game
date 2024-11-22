using bullethellwhatever.BaseClasses;
using bullethellwhatever.Bosses.CrabBoss.Attacks;
using bullethellwhatever.Projectiles;

using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.UtilitySystems;
using Microsoft.VisualBasic.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
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
            int pullBackArmTime = 20;
            int swingDuration = 10;

            ref float swingTime = ref ExtraData[1]; // the farthest point in the attack the swing can happen. will be set to a smaller number if the boss is close to the player.
            int chargeTrackingTime = 5;
            int accelerateTime = 25;
            int maxChargeTime = 36;

            float angleToPullBackArm = PI / 2f;
            float angleToSwingThrough = PI;
            float topChargeSpeed = 0f;

            Vector2 toPlayer = player.Position - Owner.Position;
            float angleToPlayer = Utilities.VectorToAngle(toPlayer);

            ref float initialSpeed = ref ExtraData[2];
            ref float HasSetSwingTime = ref ExtraData[0]; // if this is 0, swing has not been set. if 1, swing has been set

            if (AITimer == 0)
            {
                // clear owner ai variables
                ClearExtraData();
                Owner.ClearExtraData();

                initialSpeed = Owner.Velocity.Length();
                swingTime = maxChargeTime;
                HasSetSwingTime = 0f;

                if (Utilities.DistanceBetweenVectors(Leg(0).Position, player.Position) > Utilities.DistanceBetweenVectors(Leg(1).Position, player.Position))
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

            if (AITimer < accelerateTime)
            {
                float trackingCompletionRatio = AITimer / (float)accelerateTime;
                float differenceBetweenTopAndInitialChargeSpeed = topChargeSpeed - initialSpeed;

                float chargeSpeed = MathHelper.Lerp(initialSpeed, topChargeSpeed, EasingFunctions.EaseInOutQuart(trackingCompletionRatio));

                if (AITimer < chargeTrackingTime)
                {
                    Owner.Velocity = chargeSpeed * Utilities.SafeNormalise(toPlayer);
                }
                else
                {
                    Owner.Velocity = chargeSpeed * Utilities.SafeNormalise(Owner.Velocity);
                }
            }

            if (AITimer < pullBackArmTime)
            {
                // figure out what angle to swing through this frame.
                // easeoutquad
                RotateArm(ChosenArmIndex(), expandedi * angleToPullBackArm, AITimer, pullBackArmTime, EasingFunctions.Linear);
            }

            float swingProximity = 300f;

            if (AITimer > pullBackArmTime && HasSetSwingTime == 0) // if the arm is fully pulled back and a swing time hasnt been chosen, the boss can choose when to punch
            {
                if (Utilities.DistanceBetweenEntities(player, Owner) < swingProximity)
                {
                    //Owner.Colour = Owner.Colour == Color.White ? Color.Blue : Color.White;
                    HasSetSwingTime = 1f;
                    swingTime = AITimer + 1; // swing on the next frame
                }
            }

            if (AITimer >= swingTime && AITimer < swingTime + swingDuration)
            {
                int localTime = AITimer - (int)swingTime;

                //easeoutexpo
                RotateArm(ChosenArmIndex(), -expandedi * angleToSwingThrough, localTime, swingDuration, EasingFunctions.Linear);
            }

            if (AITimer < swingTime)
            {
                CrabOwner.Rotation = Utilities.VectorToAngle(-toPlayer);
            }
            else if (AITimer >= swingTime && AITimer < swingTime + swingDuration)
            {
                //CrabOwner.Rotation += expandedi * totalSwingAngle / swingDuration;
            }

            int timeToDecelAfterSwing = 12;
            int decelerateTime = 10;

            if (AITimer == swingTime + timeToDecelAfterSwing)
            {
                // set initial speed to decelerate from 
                initialSpeed = Owner.Velocity.Length();
            }

            if (AITimer > swingTime + timeToDecelAfterSwing && AITimer <= swingTime + swingDuration + timeToDecelAfterSwing)
            {
                float localTime = AITimer - swingTime - timeToDecelAfterSwing;

                float speedInterpolant = (float)localTime / decelerateTime;

                Owner.Velocity = Utilities.SafeNormalise(Owner.Velocity) * MathHelper.Lerp(initialSpeed, 0, speedInterpolant);
            }

            if (AITimer == swingTime + timeToDecelAfterSwing + decelerateTime)
            {
                End();
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabPunchToNeutralTransition(CrabOwner);
            int nextAttack = Utilities.RandomInt(1, 3);
            if (nextAttack == 1 || nextAttack == 2 || true)
                return new CrabPunchToNeutralTransition(CrabOwner);
            else
                return new CrabPunchToProjectileSpreadTransition(CrabOwner);
        }

        public override void ExtraDraw(SpriteBatch s)
        {
            Utilities.drawTextInDrawMethod("attacking", player.Position - new Vector2(0f, 50f), _spriteBatch, font, Color.White, 2);
            Utilities.drawTextInDrawMethod(ExtraData[1].ToString(), player.Position + new Vector2(0f, 50f), _spriteBatch, font, Color.White, 2);
        }
    }

    public class CrabPunchToNeutralTransition : CrabBossAttack
    {
        public CrabPunchToNeutralTransition(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int armRotateBackToNeutralTime = 60;

            float totalSwingAngle = PI / 2f;

            // this always occurs after the crab punch so the owners extradata[0] will still be the chosen arm
            ref float chosenArm = ref Owner.ExtraData[0];
            int expandedi = Utilities.ExpandedIndex((int)chosenArm);

            if (AITimer < armRotateBackToNeutralTime)
            {
                float interpolant = AITimer / (float)armRotateBackToNeutralTime;

                Vector2 toPlayer = Owner.Position - player.Position;
                float angleToPlayer = Utilities.VectorToAngle(toPlayer);
                float angleToPlayerMinusTwoPi = angleToPlayer - 2 * PI;

                // these angles are functionally the same

                float angleToUse;
                
                // determine which direction to turn towards to minimise turn angle
                if (Abs(Owner.Rotation - angleToPlayer) < Abs(Owner.Rotation - angleToPlayerMinusTwoPi))               
                {
                    angleToUse = angleToPlayer;
                }
                else
                {
                    angleToUse = angleToPlayerMinusTwoPi;
                }

                Owner.Rotation = MathHelper.Lerp(Owner.Rotation, angleToUse, interpolant);

                // i think this is the problem?

                RotateArm(ChosenArmIndex(), -expandedi * totalSwingAngle, AITimer, armRotateBackToNeutralTime, EasingFunctions.Linear);
            }

            if (AITimer == armRotateBackToNeutralTime)
            {
                End();
            }
        }
        public override BossAttack PickNextAttack()
        {
            return new CrabPunch(CrabOwner);
            int nextAttack = Utilities.RandomInt(1, 2);
            //if (nextAttack == 1 && CrabOwner.CanPerformCrabPunch())
            if (true)
                return new DoNothing(CrabOwner);
            else
                return new CrabBombThrow(CrabOwner);
        }
        public override void ExtraDraw(SpriteBatch s)
        {
            Utilities.drawTextInDrawMethod("resetting", player.Position - new Vector2(0f, 50f), _spriteBatch, font, Color.White, 2);
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
                float interpolant = AITimer / (float)armRotateBackToNeutralTime;

                Vector2 toPlayer = Owner.Position - player.Position;
                float angleToPlayer = Utilities.VectorToAngle(toPlayer);
                float angleToPlayerMinusTwoPi = angleToPlayer - 2 * PI;

                // these angles are functionally the same

                float angleToUse;

                // determine which direction to turn towards to minimise turn angle
                if (Abs(Owner.Rotation - angleToPlayer) < Abs(Owner.Rotation - angleToPlayerMinusTwoPi))
                {
                    angleToUse = angleToPlayer;
                }
                else
                {
                    angleToUse = angleToPlayerMinusTwoPi;
                };

                Owner.Rotation = MathHelper.Lerp(Owner.Rotation, angleToUse, interpolant);

                //Leg((int)chosenArm).RotateLeg(expandedi * totalSwingAngle / armRotateBackToNeutralTime);
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
