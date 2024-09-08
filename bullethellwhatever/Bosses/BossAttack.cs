﻿using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.Bosses
{
    public abstract class BossAttack
    {
        public Boss Owner;
        public bool HasResetAITimerForDesperation;
        public bool EndNow;
        public BossAttack()
        {
            EndNow = false;
        }

        public virtual void InitialiseAttackValues()
        {

        }
        public virtual void Execute(ref int AITimer, ref int AttackNumber)
        {

        }

        public virtual void ExtraDraw(SpriteBatch s)
        {

        }

        public virtual void End()
        {
            EndNow = true;
        }

        public virtual void ExtraAttackEnd()
        {

        }

        public virtual void TryEndAttack(ref int AITimer, ref int AttackNumber)
        {
            if (EndNow)
            {
                Owner.AITimer = -1; //to prevent jank with EndAttack taking a frame, allows attacks to start on 0, change back to -1 if cringe things happen

                EndNow = false;

                Owner.Rotation = 0;

                if (Owner.AttackNumber != Owner.BossAttacks.Length - 1)
                {
                    Owner.AttackNumber++;
                    Owner.BossAttacks[Owner.AttackNumber].InitialiseAttackValues();
                    ExtraAttackEnd();
                }
                else
                {
                    Owner.AttackNumber = 0;
                    Owner.BossAttacks[Owner.AttackNumber].InitialiseAttackValues();
                    ExtraAttackEnd();
                }
            }

            if (Owner.Health <= 0 && !Owner.CanDie && !HasResetAITimerForDesperation)
            {
                HasResetAITimerForDesperation = true;
            }
        }

        public virtual void HandleBounces()
        {
            if (Owner.TouchingLeft())
            {
                if (Owner.Velocity.X < 0)
                    Owner.Velocity.X = Owner.Velocity.X * -1;
            }

            if (Owner.TouchingRight())
            {
                if (Owner.Velocity.X > 0)
                    Owner.Velocity.X = Owner.Velocity.X * -1;
            }

            if (Owner.TouchingTop())
            {
                if (Owner.Velocity.Y < 0)
                    Owner.Velocity.Y = Owner.Velocity.Y * -1f;

            }

            if (Owner.TouchingBottom())
            {
                if (Owner.Velocity.Y > 0)
                    Owner.Velocity.Y = Owner.Velocity.Y * -1f;
            }
        }

        public void MoveToCentre(int AITImer, int duration)
        {
            Vector2 vectorToCentre = Utilities.CentreOfScreen() - Owner.Position;
            float distanceToTravel = vectorToCentre.Length();
            //Velocity = Utilities.SafeNormalise(vectorToCentre, Vector2.Zero) * distanceToTravel / (timeToStartAt - AITimer);

            // top 5 integration moments
            Owner.Velocity = Utilities.SafeNormalise(vectorToCentre, Vector2.Zero) * (2f * MathF.PI * distanceToTravel / duration) * MathF.Sin(MathF.PI * Owner.AITimer / duration);
        }

        public void Accelerate(Vector2 finalVel, int duration)
        {
            Owner.Velocity = Owner.Velocity + finalVel / duration;
        }
        public void MoveToPoint(Vector2 point, int movementTimer, int duration)
        {
            Vector2 vectorToPoint = point - Owner.Position;
            float distanceToTravel = vectorToPoint.Length();

            // top 5 integration moments
            Owner.Velocity = Utilities.SafeNormalise(vectorToPoint, Vector2.Zero) * (2f * PI * distanceToTravel / duration) * Sin(PI * movementTimer / duration);
        }
    }
}
