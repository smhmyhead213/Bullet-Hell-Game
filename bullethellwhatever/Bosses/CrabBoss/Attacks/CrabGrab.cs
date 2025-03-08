﻿using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabGrab : CrabBossAttack
    {
        public CrabGrab(CrabBoss owner) : base(owner)
        {

        }
        public override void Execute(int AITimer)
        {
            int expandedi = Utilities.ExpandedIndex(1);

            int pullBackArmTime = 45;
            int waitTime = 30;
            int clawCloseTime = 10;

            int swingTime = 15;
            float pullBackAngle = -expandedi * PI / 2f;
            float finalSwingAngle = -expandedi * -PI / 18f;
            
            float initialarmLength = Arm(0).WristLength(); // kinda hacky but this gives the original non enlarged length
            float armLength = Arm(1).WristLength();

            if (AITimer < pullBackArmTime)
            {
                // calculate the size the arm must be to reach the player

                float distToPlayer = Arm(1).Position.Distance(player.Position);
                float scaleFactor = distToPlayer / initialarmLength;
                float fractionOfLengthHoldNearBody = 0.9f;

                Arm(1).SetScale(scaleFactor);

                float interpolant = EasingFunctions.EaseOutExpo(AITimer / (float)pullBackArmTime);

                Arm(1).LerpToPoint(Arm(1).Position + new Vector2(0f, armLength * fractionOfLengthHoldNearBody).Rotate(pullBackAngle), interpolant, true);

                Arm(1).LowerClaw.LerpRotation(0f, -expandedi * PI / 2f, interpolant);
                //Arm(1).UpperClaw.LerpRotation(0f, -expandedi * -PI / 2, interpolant);

            }

            // && AITimer < pullBackArmTime + swingTime + waitTime

            if (AITimer >= pullBackArmTime + waitTime)
            {
                int localTime = AITimer - (pullBackArmTime + waitTime);
                float progress = MathHelper.Clamp(localTime / (float)swingTime, 0f, 1f);
                float interpolant = EasingFunctions.EaseOutExpo(progress);
                float finalArmLength = armLength;
                float armLengthNow = MathHelper.Lerp(armLength, finalArmLength, interpolant);
                float armScale = Arm(1).UpperArm.GetSize().X;
                Vector2 finalPoint = player.Position;

                Arm(1).UpperArm.LerpRotation(-expandedi * PI / 2, 0f, interpolant);
                Arm(1).LowerClaw.LerpToZero(progress);
                Arm(1).LowerClaw.ContactDamage = false;
                Arm(1).UpperClaw.ContactDamage = false;
                //Arm(1).LowerArm.ContactDamage = false;

                // make a new flag for grabbed later
                if (Arm(1).LowerClaw.IsCollidingWith(player) || Arm(1).LowerClaw.IsCollidingWith(player) && !player.InputLocked)
                {
                    player.LockMovement();
                }

                if (player.InputLocked)
                {
                    player.Position = Arm(1).WristOffsetBy(new Vector2(15f * armScale, -15f * armScale));
                }
            }
        }

        public override bool SelectionCondition()
        {
            return Owner.DistanceFromPlayer() > 1200f;
        }
        public override BossAttack PickNextAttack()
        {
            return new CrabGrab(CrabOwner);
        }
    }
}
