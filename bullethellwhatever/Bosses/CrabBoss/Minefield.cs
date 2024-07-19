using bullethellwhatever.Projectiles;
using bullethellwhatever.Projectiles.Base;
 
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace bullethellwhatever.Bosses.CrabBoss
{
    public class Minefield : CrabBossAttack
    {
        public int TimeToStartForcingPlayerLeft;
        public int TimeToMoveRightFor;
        public int TimeToStartPreparingForcingPlayerRight;
        public int TimeToStartForcingPlayerRight;
        private int StoredTime; //debug remove later
        public int Time;
        public Minefield(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            CrabOwner.Velocity = Vector2.Zero;
            TimeToStartForcingPlayerLeft = 50; //spend frames moving to top right, and start at this time
            TimeToStartPreparingForcingPlayerRight = 300; //start moving through minefield to the right at this time
            TimeToStartForcingPlayerRight = 350;
            TimeToMoveRightFor = 1200;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer % (TimeToMoveRightFor + TimeToStartForcingPlayerRight);

            Time = time;
            int timeSpentMovingLeft = TimeToStartPreparingForcingPlayerRight - TimeToStartForcingPlayerLeft;

            if (time == 0)
            {
                CrabOwner.ResetArmRotations();
                CrabOwner.SetBoosters(false);
            }

            if (time < TimeToStartForcingPlayerLeft)
            {
                Vector2 position = new Vector2(ScreenWidth / 8 * 7, ScreenHeight / 8);
                MoveToPoint(position, time, TimeToStartForcingPlayerLeft);
            }

            if (time == TimeToStartForcingPlayerLeft)
            {
                CrabOwner.Velocity = Vector2.Zero;
            }

            else if (time >= TimeToStartForcingPlayerLeft && time < TimeToStartPreparingForcingPlayerRight) // force player left
            {
                int timeToOpenArm = 60;

                if (time < TimeToStartForcingPlayerLeft + timeToOpenArm)
                {
                    float totalAngleToOpenBy;

                    totalAngleToOpenBy = Abs(Leg(1).UpperArm.RotationConstant); // make arm completely straight by cancelling the rotation constant
                    Leg(1).UpperArm.Rotate(totalAngleToOpenBy / timeToOpenArm);

                    totalAngleToOpenBy = Abs(Leg(1).LowerArm.RotationConstant);
                    Leg(1).LowerArm.Rotate(-totalAngleToOpenBy / timeToOpenArm);
                }

                else if (time == TimeToStartForcingPlayerLeft + timeToOpenArm)
                {
                    Deathray ray = new Deathray();
                    // duration has to account for 2 arm opening times and stop a bit earlier
                    ray.SpawnDeathray(Leg(1).LowerClaw.Position, PI, 1f, TimeToStartPreparingForcingPlayerRight - TimeToStartForcingPlayerLeft - timeToOpenArm, "box", 20, ScreenHeight, 0, true, Color.White, "DeathrayShader2", Leg(1).LowerClaw);                    
                    ray.SetStayWithOwner(true);
                    CrabOwner.Velocity.X = ((ScreenWidth / 8) - (ScreenWidth / 12 * 11)) / (timeSpentMovingLeft - timeToOpenArm);
                }
                
                else
                {
                    if (AITimer % 5 == 0) //20 projectiles
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            Random rng = new Random();

                            int height = rng.Next((int)CrabOwner.Position.Y, ScreenHeight);
                            int widthOffset = rng.Next(-15, 15);

                            SpawnProjectile(new Vector2(Leg(1).LowerClaw.Position.X + widthOffset, height), Vector2.Zero, 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                        }
                    }
                }
            }

            else if (time == TimeToStartPreparingForcingPlayerRight) // spend 60 opening arm
            {
                CrabOwner.Velocity.X = 0; // NOOOOOOOOO DONT LEAVE MEEEEEEEE
                SpawnTelegraphLine(PI, 0, 20, ScreenHeight, TimeToStartForcingPlayerRight - TimeToStartPreparingForcingPlayerRight, Leg(0).LowerClaw.Position, Color.White, "box", Leg(0).LowerClaw, true);
            }

            else if (time >= TimeToStartPreparingForcingPlayerRight && time < TimeToStartForcingPlayerRight)
            {
                float totalAngleToRotateBy; //undo the previous rotation
                int timeToOpenArm = TimeToStartForcingPlayerRight - TimeToStartPreparingForcingPlayerRight;

                totalAngleToRotateBy = Abs(Leg(1).UpperArm.RotationConstant); // make arm completely straight by cancelling the rotation constant
                Leg(1).UpperArm.Rotate(-totalAngleToRotateBy / timeToOpenArm);

                totalAngleToRotateBy = Abs(Leg(1).LowerArm.RotationConstant);
                Leg(1).LowerArm.Rotate(totalAngleToRotateBy / timeToOpenArm);

                totalAngleToRotateBy = Abs(Leg(0).UpperArm.RotationConstant); // open up left arm
                Leg(0).UpperArm.Rotate(-totalAngleToRotateBy / timeToOpenArm);

                totalAngleToRotateBy = Abs(Leg(0).LowerArm.RotationConstant);
                Leg(0).LowerArm.Rotate(totalAngleToRotateBy / timeToOpenArm);
            }

            else if (time == TimeToStartForcingPlayerRight)
            {
                Deathray ray = new Deathray();

                ray.SpawnDeathray(Leg(0).LowerClaw.Position, PI, 1f, TimeToMoveRightFor, "box", 20, ScreenHeight, 0, true, Color.White, "DeathrayShader2", Leg(0).LowerClaw);
                ray.SetStayWithOwner(true);
                CrabOwner.Velocity.X = ((ScreenWidth / 50f * 49) - (ScreenWidth / 8f)) / (TimeToMoveRightFor);
            }

            else if (time > TimeToStartForcingPlayerRight && time < TimeToStartForcingPlayerRight + TimeToMoveRightFor)
            {
                if (AITimer % 10 == 0)
                {
                    Deathray ray = new Deathray();

                    ray.SpawnDeathray(new Vector2(Leg(0).LowerClaw.Position.X, 0), PI, 1f, TimeToMoveRightFor, "box", 40, ScreenHeight, 0, true, Color.White, "DeathrayShader2", Leg(0).LowerClaw);
                }
            }

            if (time == TimeToStartForcingPlayerRight + TimeToMoveRightFor - 1)
            {
                foreach (Projectile p in activeProjectiles)
                {
                    p.Die();
                }    
            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {
            //if (activeProjectiles.Count > 0)
            //{
            //    Utilities.drawTextInDrawMethod(activeProjectiles[0].AITimer.ToString(), Utilities.CentreOfScreen(), _spriteBatch, font, Color.White);
            //    Utilities.drawTextInDrawMethod(((Deathray)activeProjectiles[0]).Duration.ToString(), Utilities.CentreOfScreen() + new Vector2(0, 20), _spriteBatch, font, Color.White);
            //    Utilities.drawTextInDrawMethod((Time - TimeToStartForcingPlayerLeft - 60).ToString(), Utilities.CentreOfScreen() + new Vector2(0, 40), _spriteBatch, font, Color.White);

            //}
        }
    }
}
