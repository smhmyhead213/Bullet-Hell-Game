﻿using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class PhaseTwoTransition : CrabBossAttack
    {
        public int SlowDownTime;
        public int MoveToCentreEndTime;
        public float RotationToUndo;
        public PhaseTwoTransition(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            SlowDownTime = 120;
            MoveToCentreEndTime = 180;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer % 1000;

            if (time == 0)
            {
                foreach (Projectile p in activeProjectiles)
                {
                    p.Die(); //clear all projectiles
                }
            }

            if (time < SlowDownTime)
            {
                Owner.Velocity = Owner.Velocity * 0.98f; 
            }

            if (time == SlowDownTime)
            {
                Owner.Velocity = Vector2.Zero;
                RotationToUndo = Owner.Rotation;
            }

            if (time > SlowDownTime && time < MoveToCentreEndTime)
            {
                MoveToPoint(Utilities.CentreOfScreen(), MoveToCentreEndTime - time, MoveToCentreEndTime - SlowDownTime);
                Owner.Rotation = Owner.Rotation - (RotationToUndo / (MoveToCentreEndTime - SlowDownTime));
            }

            HandleBounces();
        }
    }
}
