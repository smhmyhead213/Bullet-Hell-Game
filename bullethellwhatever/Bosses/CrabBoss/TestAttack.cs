using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Enemy;
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

            float cosine = Cos(AITimer / 10f);
            CrabOwner.Velocity = Vector2.Zero;

            //CrabOwner.Rotation = CrabOwner.Rotation + PI / 60f; //keep this commented out

            CrabOwner.Legs[0].UpperArm.Rotate(0.075f * 2f * cosine);
            CrabOwner.Legs[0].LowerArm.Rotate(0.125f * 2f * cosine);
            CrabOwner.Legs[0].UpperClaw.Rotate(0.125f * 2f * cosine);

            //CrabOwner.Legs[0].LowerClaw.RotationConstant = -PI / 3; // keep claw open

            CrabOwner.Legs[0].LowerClaw.Rotate(PI / 90 * cosine);

            CrabOwner.Legs[0].LowerClaw.Rotate(0.125f * cosine);

            //CrabOwner.Rotation = PI / 2f;

            int positionChangeTime = 180;
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
            }

            int timeSinceClapReset = AITimer % otherHandClapFrequency;

            Func<float, float> clawOpenFunction = x => Sqrt(x * otherHandClawOpenAngle * otherHandClawOpenAngle / otherHandClapOpenTime); // a square root function that increases from x = 0 to 90 and y = 0 to open angle
            //Func<float, float> clawCloseFuncton = x => 
            if (timeSinceClapReset < otherHandClapOpenTime) // if we are opening the claw
            {
                float angleToOpenByThisFrame = clawOpenFunction(timeSinceClapReset + 1) - clawOpenFunction(timeSinceClapReset); // the amount needed to open this frame, f(1) - f(0)

                Leg(1).UpperClaw.Rotate(angleToOpenByThisFrame);
                Leg(1).LowerClaw.Rotate(-angleToOpenByThisFrame);//rotate based on formula sitting in desmos
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
                WeakHomingProjectile proj = new WeakHomingProjectile(12f, 60);

                proj.Spawn(Leg(1).LowerClaw.Position, 8f * (player.Position - Leg(1).LowerClaw.Position), 1f, 1, "box", 1f, Vector2.One, Owner, true, Color.Red, true, false);
            }
            // move towards that position

            int timeSinceLastPositionChoose = AITimer % positionChangeTime;

            CrabOwner.Position = Vector2.Lerp(CrabOwner.Position, PositionToMoveTo, 1f / (positionChangeTime - timeSinceLastPositionChoose));

            //rotate to face the player

            CrabOwner.Rotation = Utilities.VectorToAngle(CrabOwner.Position - player.Position);

            if (cosine > 0.5)
            {
                Projectile proj = new Projectile();

                Vector2 direction = Utilities.AngleToVector(Leg(0).LowerArm.RotationFromV());

                proj.Spawn(CrabOwner.Legs[0].LowerClaw.Position, 8f * direction, 1f, 1, "box", 1f, Vector2.One, Owner, true, Color.Red, true, false);
                //proj.Spawn(CrabOwner.Position, 5f * Utilities.AngleToVector(PI / 2f), 1f, 1, "box", 1f, Vector2.One, Owner, true, Color.Red, true, false);
            }

            HandleBounces(); //to prevent something funny happening

        }
    }
}
