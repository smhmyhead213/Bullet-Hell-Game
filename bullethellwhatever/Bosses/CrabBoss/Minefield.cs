﻿using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class Minefield : CrabBossAttack
    {
        public int TimeToStartForcingPlayerLeft;
        public int TimeToMoveRightFor;
        public int TimeToStartPreparingForcingPlayerRight;
        public int TimeToStartForcingPlayerRight;
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
            TimeToMoveRightFor = 600;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer % (TimeToStartForcingPlayerRight + TimeToMoveRightFor + TimeToStartPreparingForcingPlayerRight);

            int timeSpentMovingLeft = TimeToStartPreparingForcingPlayerRight - TimeToStartForcingPlayerLeft;

            if (time == 0)
            {
                CrabOwner.ResetArmRotations();
            }

            Vector2 position = new Vector2(ScreenWidth / 8 * 7, ScreenHeight / 8);
            if (time < TimeToStartForcingPlayerLeft)
            {
                MoveToPoint(position, AITimer, TimeToStartForcingPlayerLeft);
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
                    ray.SpawnDeathray(Leg(1).LowerClaw.Position, PI, 1f, TimeToStartPreparingForcingPlayerRight - TimeToStartForcingPlayerLeft, "box", 20, ScreenHeight, 0, 0, true, Color.White, "DeathrayShader2", Leg(1).LowerClaw);
                    ray.SetStayWithOwner(true);
                    CrabOwner.Velocity.X = ((ScreenWidth / 8) - (ScreenWidth / 8 * 7)) / (timeSpentMovingLeft - timeToOpenArm);
                }
                
                else
                {
                    if (AITimer % 5 == 0) //20 projectiles
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            Random rng = new Random();
                            Projectile proj = new Projectile();
                            int height = rng.Next((int)CrabOwner.Position.Y, ScreenHeight);
                            int widthOffset = rng.Next(-5, 5);

                            proj.Spawn(new Vector2(Leg(1).LowerClaw.Position.X + widthOffset, height), Vector2.Zero, 1f, 1, "box", 0, Vector2.One, Owner, true, Color.Red, true, false);
                        }
                    }
                }
            }

            else if (time == TimeToStartPreparingForcingPlayerRight)
            {
                CrabOwner.Velocity.X = 0; // NOOOOOOOOO DONT LEAVE MEEEEEEEE
            }
            else if (time >= TimeToStartPreparingForcingPlayerRight && time < TimeToStartForcingPlayerRight)
            {
                float totalAngleToRotateBy; //undo the previous rotation
                int timeToOpenArm = TimeToStartForcingPlayerRight - TimeToStartPreparingForcingPlayerRight;

                totalAngleToRotateBy = Abs(Leg(1).UpperArm.RotationConstant); // make arm completely straight by cancelling the rotation constant
                Leg(1).UpperArm.Rotate(-totalAngleToRotateBy / timeToOpenArm);

                totalAngleToRotateBy = Abs(Leg(1).LowerArm.RotationConstant);
                Leg(1).LowerArm.Rotate(totalAngleToRotateBy / timeToOpenArm);

                totalAngleToRotateBy = Abs(Leg(0).UpperArm.RotationConstant); // make arm completely straight by cancelling the rotation constant
                Leg(0).UpperArm.Rotate(-totalAngleToRotateBy / timeToOpenArm);

                totalAngleToRotateBy = Abs(Leg(1).LowerArm.RotationConstant);
                Leg(0).LowerArm.Rotate(totalAngleToRotateBy / timeToOpenArm);
            }
        }
    }
}
