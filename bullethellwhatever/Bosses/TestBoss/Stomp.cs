using bullethellwhatever.MainFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.DrawCode;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class Stomp : BossAttack
    {
        public int CompletedStomps;
        public int TargetingDuration;
        public int StompDuration;
        public int StompFrequency = 120;
        public float DistanceToBottom;
        public Stomp(int endTime) : base(endTime)
        {
            EndTime = endTime;
            CompletedStomps = 0;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {           
            if (AITimer == 0)
            {
                CompletedStomps = 0; //reset
            }

            if (AITimer > (CompletedStomps * StompFrequency) && AITimer < (CompletedStomps * StompFrequency + StompFrequency / 2))
            {
                MoveToPoint(Main.player.Position + new Vector2(0, -600), AITimer, StompFrequency / 2);
                Owner.ContactDamage = false;
            }

            if (AITimer > (CompletedStomps * StompFrequency + StompFrequency / 2) && AITimer < ((CompletedStomps + 1) * StompFrequency))
            {
                Owner.ContactDamage = true;

                if (AITimer == (CompletedStomps * StompFrequency + StompFrequency / 2) + 1)
                {
                    DistanceToBottom = Main.ScreenHeight - Owner.Position.Y;
                }

                //float neededVerticalSpeed = DistanceToBottom / (((CompletedStomps + 1) * StompFrequency) - AITimer);

                //neededVerticalSpeed = 0.1f * (((CompletedStomps + 1) * StompFrequency) - (CompletedStomps * StompFrequency + StompFrequency / 2));

                float accel = 2 * DistanceToBottom / (StompFrequency * StompFrequency);

                int timeSinceDrop = (StompFrequency / 2) - ((CompletedStomps + 1) * StompFrequency) - (120 - AITimer % StompFrequency);

                Owner.Velocity = new Vector2(0f, accel * timeSinceDrop);
            }

            if (AITimer == CompletedStomps * StompFrequency + StompFrequency)
            {
                CompletedStomps++;
                Drawing.ScreenShake(5, 20);
            }
        }
    }
}
