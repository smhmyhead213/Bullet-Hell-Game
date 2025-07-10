using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.NPCs;

namespace bullethellwhatever.Bosses
{
    public abstract class BossAttack
    {
        public NPC Owner;
        public bool HasResetAITimerForDesperation;
        public bool EndNow;

        /// <summary>
        /// Useful int to hold a value to help choose the next attack.
        /// </summary>
        public int NextAttack;

        public float[] ExtraData;
        public BossAttack(NPC owner)
        {
            EndNow = false;
            Owner = owner;

            ExtraData = new float[4];
        }

        public virtual void InitialiseAttackValues()
        {

        }
        public virtual void Execute(int AITimer)
        {

        }

        public virtual void ExtraDraw(SpriteBatch s, int AITimer)
        {

        }

        public virtual void End()
        {
            EndNow = true;
            //Owner.PreviousAttack = Owner.CurrentAttack;
            Owner.CurrentAttack = PickNextAttack();
            Owner.AITimer = -1; // will be increased in update() immediately after

            ClearExtraData();
        }

        public virtual void ClearExtraData()
        {
            for (int i = 0; i < ExtraData.Length; i++)
            {
                ExtraData[i] = 0;
            }
        }

        public virtual void ExtraAttackEnd()
        {

        }

        public virtual bool SelectionCondition()
        {
            return true;
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

        public virtual BossAttack PickNextAttack()
        {
            return this;
        }

        public virtual List<Type> BannedFollowUps()
        {
            return new List<Type>();
        }
    }
}