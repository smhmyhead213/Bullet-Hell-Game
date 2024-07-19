using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles;
 
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class TestAttack : CrabBossAttack
    {
        public Vector2 PositionToMoveTo;
        public TestAttack(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            CrabOwner.Velocity = Vector2.Zero;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {

            //Leg(0).LowerClaw.Rotate(PI / 60f);
            //Leg(0).UpperClaw.Rotate(PI / 60f);

            //CrabOwner.Rotation = CrabOwner.Rotation + PI / 60f; //keep this commented out

            //----------

            float cosine = Cos(AITimer / 10f);

            CrabOwner.Legs[0].UpperArm.Rotate(0.075f * 2f * cosine);
            CrabOwner.Legs[0].LowerArm.Rotate(0.125f * 2f * cosine);
            CrabOwner.Legs[0].UpperClaw.Rotate(0.125f * 2f * cosine);

            //CrabOwner.Legs[0].LowerClaw.RotationConstant = -PI / 3; // keep claw open

            CrabOwner.Legs[0].LowerClaw.Rotate(PI / 90 * cosine);

            CrabOwner.Legs[0].LowerClaw.Rotate(0.125f * cosine);

            //CrabOwner.Rotation = PI / 2f;

            int positionChangeTime = Utilities.ValueFromDifficulty(200, 180, 120, 90);
            int otherHandClapFrequency = 75;
            int otherHandClapOpenTime = 50;
            int otherHandClapCloseTime = otherHandClapFrequency - otherHandClapOpenTime;
            float otherHandClawOpenAngle = PI / 3;

            if (AITimer % positionChangeTime == 0) // every 3 seconds, pick a new location to move to
            {
                Random rng = new Random();

                int xpos = rng.Next(ScreenWidth / 8, ScreenWidth / 8 * 7);
                int ypos = rng.Next(ScreenHeight / 8, ScreenHeight / 8 * 7);

                PositionToMoveTo = new Vector2(xpos, ypos);

                Owner.Velocity = (PositionToMoveTo - Owner.Position) / positionChangeTime;
            }

            int timeSinceClapReset = AITimer % otherHandClapFrequency;

            Func<float, float> clawOpenFunction = x => Sqrt(x * otherHandClawOpenAngle * otherHandClawOpenAngle / otherHandClapOpenTime); // a square root function that increases from x = 0 to 90 and y = 0 to open angle
            //Func<float, float> clawCloseFuncton = x => 
            if (timeSinceClapReset < otherHandClapOpenTime) // if we are opening the claw
            {
                float angleToOpenByThisFrame = clawOpenFunction(timeSinceClapReset + 1) - clawOpenFunction(timeSinceClapReset); // the amount needed to open this frame, f(1) - f(0)

                Leg(1).UpperClaw.Rotate(angleToOpenByThisFrame);
                Leg(1).LowerClaw.Rotate(-angleToOpenByThisFrame);
            }
            else
            {
                float howManyTimesFasterDoWeCloseThanOpen = otherHandClapFrequency / (otherHandClapFrequency - otherHandClapOpenTime) - 1;

                int timeSpentClosing = timeSinceClapReset - otherHandClapOpenTime;

                // we go backwards now, from f(90) to f(87) then f(87) to f(84) and so on

                float functionInitialInput = otherHandClapOpenTime - howManyTimesFasterDoWeCloseThanOpen * timeSpentClosing;

                float angleToCloseByThisFrame = clawOpenFunction(functionInitialInput) - clawOpenFunction(functionInitialInput - howManyTimesFasterDoWeCloseThanOpen); // close with bigger jumps than opening to go faster

                Leg(1).UpperClaw.Rotate(-angleToCloseByThisFrame);
                Leg(1).LowerClaw.Rotate(angleToCloseByThisFrame);//rotate based on formula sitting in desmos
            }

            if (timeSinceClapReset > otherHandClapFrequency - 10)
            {
                Projectile proj = SpawnProjectile(Leg(1).LowerClaw.Position, 8f * (player.Position - Leg(1).LowerClaw.Position), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);

                proj.SetExtraAI(new Action(() =>
                {
                    int timeToStopHoming = 60;
                    float projSpeed = 12f;

                    if (proj.AITimer < timeToStopHoming)
                    {
                        Vector2 vectorToTarget = Main.player.Position - proj.Position; //get a vector to the target

                        proj.Velocity = projSpeed * Utilities.SafeNormalise(Vector2.Lerp(proj.Velocity, vectorToTarget, 0.003f), Vector2.Zero);

                    }
                }));
            }

            // move towards that position

            int timeSinceLastPositionChoose = AITimer % positionChangeTime;

            //rotate to face the player

            CrabOwner.Rotation = Utilities.VectorToAngle(CrabOwner.Position - player.Position);

            int timeBetweenShots = Utilities.ValueFromDifficulty(2, 2, 1, 1);

            if (cosine > 0.5 && AITimer % timeBetweenShots == 0)
            {
                Vector2 direction = Utilities.AngleToVector(Leg(0).LowerArm.RotationFromV());

                SpawnProjectile(CrabOwner.Legs[0].LowerClaw.Position, 8f * direction, 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
            }

            HandleBounces(); //to prevent something funny happening

            if (AITimer == EndTime)
            {
                Owner.Velocity = Vector2.Zero;
            }
        }
    }
}
