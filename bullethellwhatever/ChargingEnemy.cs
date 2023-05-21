using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using System.Threading.Tasks;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.DrawCode;
using bullethellwhatever.UtilitySystems.Dialogue;


namespace bullethellwhatever
{   
    public class ChargingEnemy : NPC
    {
        public int TimeToBeginActivity;
        public float ChargeSpeed;
        public bool HasChosenChargeDirection;
        public int ChargeFrequency;
        public ChargingEnemy(int timeToBegin, int chargeFrequency)
        {
            TimeToBeginActivity = timeToBegin;
            IsDesperationOver = true;
            ChargeFrequency = chargeFrequency;

            MaxIFrames = 0;

            dialogueSystem = new DialogueSystem(this);
        }

        public override void AI()
        {

            if (AITimer < TimeToBeginActivity)
            {
                Velocity = Velocity * 0.99f;
            }

            else
            {
                if (AITimer % 120 == 0)
                {
                    HasChosenChargeDirection = false;
                }

                if (!HasChosenChargeDirection)
                {
                    ChargeSpeed = 0.4f;
                    Velocity = ChargeSpeed * Utilities.SafeNormalise(Main.player.Position - Position, Vector2.Zero);

                    HasChosenChargeDirection = true;
                }

                ChargeSpeed = ChargeSpeed * MathF.Cos(AITimer % ChargeFrequency / (ChargeFrequency / 2)) + 0.5f; //the velocity follows a sine curve, so the acceleration follows its derived graph, cos x

                Velocity = ChargeSpeed * Utilities.SafeNormalise(Velocity, Vector2.Zero);
            }

            
        }
    }
}
