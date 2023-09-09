using bullethellwhatever.BaseClasses;
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
        public int EndTime;
        public Boss Owner;
        public bool HasResetAITimerForDesperation;
        public BossAttack(int endTime)
        {
            EndTime = endTime;
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
        public virtual void TryEndAttack(ref int AITimer, ref int AttackNumber)
        {
            if (Owner.AITimer == Owner.BossAttacks[Owner.AttackNumber].EndTime && AttackNumber != 0)
            {
                Owner.AITimer = 0; //to prevent jank with EndAttack taking a frame, allows attacks to start on 0, change back to -1 if cringe things happen

                Owner.Rotation = 0;
                if (Owner.AttackNumber != Owner.BossAttacks.Length - 1)
                    Owner.AttackNumber++;
                else
                    Owner.AttackNumber = 1;
            }

            if (Owner.Health <= 0 && !Owner.IsDesperationOver && !HasResetAITimerForDesperation)
            {
                HasResetAITimerForDesperation = true;
                Owner.AttackNumber = 0;
                Owner.AITimer = -1;
            }

            if (Owner.IsDesperationOver)
            {
                Owner.Die();
            }
        }
        public void HandleBounces()
        {
            if (Entity.touchingLeft(Owner))
            {
                if (Owner.Velocity.X < 0)
                    Owner.Velocity.X = Owner.Velocity.X * -1;
            }

            if (Entity.touchingRight(Owner))
            {
                if (Owner.Velocity.X > 0)
                    Owner.Velocity.X = Owner.Velocity.X * -1;
            }

            if (Entity.touchingTop(Owner))
            {
                if (Owner.Velocity.Y < 0)
                    Owner.Velocity.Y = Owner.Velocity.Y * -1f;

            }

            if (Entity.touchingBottom(Owner))
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


        public void MoveToPoint(Vector2 point, int movementTimer, int duration)
        {
            Vector2 vectorToPoint = point - Owner.Position;
            float distanceToTravel = vectorToPoint.Length();
            //Velocity = Utilities.SafeNormalise(vectorToCentre, Vector2.Zero) * distanceToTravel / (timeToStartAt - AITimer);


            // top 5 integration moments
            Owner.Velocity = Utilities.SafeNormalise(vectorToPoint, Vector2.Zero) * (2f * MathF.PI * distanceToTravel / duration) * MathF.Sin(MathF.PI * movementTimer / duration);
        }
        public void SpinUpClockwise(ref float rotation, float accel) //as accel parameter increases, the actual accel decreases
        {
            rotation = Owner.Rotation + MathF.PI / 90 * Owner.AITimer / 80f; //spin up
        }

        public void SpinUpCounterClockwise(ref float rotation, float accel) //as accel parameter increases, the actual accel decreases
        {
            rotation = Owner.Rotation - MathF.PI / 90 * Owner.AITimer / 80f; //spin up
        }

    }
}
