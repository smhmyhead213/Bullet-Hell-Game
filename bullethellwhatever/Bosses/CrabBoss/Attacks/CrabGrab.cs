using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using System.Transactions;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabGrab : CrabBossAttack
    {
        public bool GrabbedPlayer;
        public CrabGrab(CrabBoss owner) : base(owner)
        {
            GrabbedPlayer = false;
        }
        public override void Execute(int AITimer)
        {
            int armIndex = CrabBoss.GrabbingArm;
            int expandedi = Utilities.ExpandedIndex(armIndex);

            int pullBackArmTime = 45;
            int waitTime = 30;
            int clawCloseTime = 10;

            int swingTime = 5;
            float pullBackAngle = -expandedi * 3 * PI / 4;
            float finalSwingAngle = -expandedi * -PI / 18f;
            
            float initialarmLength = Arm(CrabBoss.GrabPunishArm).WristLength(); // kinda hacky but this gives the original non enlarged length
            float armLength = Arm(armIndex).WristLength();

            // face player

            //if (AITimer == 0)
            //{
            //    CrabOwner.FacePlayer();
            //}

            if (AITimer < pullBackArmTime)
            {
                // calculate the size the arm must be to reach the player

                float distToPlayer = Arm(armIndex).Position.Distance(player.Position);
                float maxScale = 8f;
                float fractionOfFullLength = AITimer / (float)pullBackArmTime;
                float scaleFactor = MathHelper.Clamp(distToPlayer / initialarmLength * fractionOfFullLength, 1f, maxScale);
                float fractionOfLengthHoldNearBody = 0.9f;

                Arm(armIndex).SetScale(scaleFactor);

                float interpolant = EasingFunctions.EaseOutExpo(AITimer / (float)pullBackArmTime);

                Arm(armIndex).LerpToPoint(Arm(armIndex).Position + new Vector2(0f, armLength * fractionOfLengthHoldNearBody).Rotate(Owner.Rotation + PI - pullBackAngle), interpolant, true);

                Arm(armIndex).LowerClaw.LerpRotation(0f, -expandedi * PI / 2f, interpolant);
                //Arm(1).UpperClaw.LerpRotation(0f, -expandedi * -PI / 2, interpolant);

                Owner.Velocity = Owner.Velocity * MathHelper.Lerp(1f, 0f, interpolant);

                Vector2 toPlayer = Owner.Position - player.Position;
                float angleToPlayer = Utilities.VectorToAngle(toPlayer);
                Owner.Rotation = Utilities.LerpRotation(Owner.Rotation, angleToPlayer, interpolant);
            }

            if (AITimer >= pullBackArmTime + waitTime && AITimer < pullBackArmTime + swingTime + waitTime)
            {
                Owner.ContactDamage = false;
                int localTime = AITimer - (pullBackArmTime + waitTime);
                float progress = MathHelper.Clamp(localTime / (float)swingTime, 0f, 1f);
                float interpolant = EasingFunctions.EaseOutExpo(progress);
                float finalArmLength = armLength;
                float armLengthNow = MathHelper.Lerp(armLength, finalArmLength, interpolant);
                float armScale = Arm(armIndex).UpperArm.GetSize().X;
                Vector2 finalPoint = player.Position;

                Arm(armIndex).UpperArm.LerpRotation(-expandedi * PI / 2, 0f, interpolant);
                Arm(armIndex).LowerClaw.LerpToZero(progress);
                Arm(armIndex).LowerClaw.ContactDamage = false;
                Arm(armIndex).UpperClaw.ContactDamage = false;
                //Arm(1).LowerArm.ContactDamage = false;

                // make a new flag for grabbed later
                if (Arm(armIndex).LowerClaw.IsCollidingWith(player) || Arm(armIndex).LowerClaw.IsCollidingWith(player) && !player.InputLocked)
                {
                    player.LockMovement();
                    player.Velocity = Vector2.Zero;
                    GrabbedPlayer = true;
                }

                if (GrabbedPlayer)
                {
                    player.Position = Arm(armIndex).WristOffsetBy(new Vector2(15f * armScale, -15f * armScale));
                }
            }

            if (AITimer == pullBackArmTime + swingTime + waitTime)
            {
                End();
            }
        }
        public override bool SelectionCondition()
        {
            return Owner.Position.Distance(player.Position) < 1000f;
        }
        public override BossAttack PickNextAttack()
        {
            if (GrabbedPlayer)
                return new CrabGrabPunish(CrabOwner);
            else return new CrabGrabMiss(CrabOwner);
        }
    }
}
