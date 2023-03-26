using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever
{
    public class PlayerHomingProjectile : FriendlyProjectile
    {
        public float HomingFactor; //how strong the homing is
        
        public override void HandleMovement()
        {

            Updates--;

            NPC closestNPC = new NPC(); //the target
            float minDistance = float.MaxValue;

            if (TimeAlive > 30f)
            {
                foreach (NPC npc in Main.activeNPCs)
                {
                    float distance = Utilities.DistanceBetweenEntities(this, npc);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestNPC = npc;
                    }
                }

                Vector2 vectorToTarget = closestNPC.Position - Main.player.Position; //get a vector to the target



                



                Velocity = Vector2.Normalize(closestNPC.Position - Position) * (TimeAlive - 30f);
            }

            Position = Position + Velocity;


        }

        public override Color Colour() => Color.LimeGreen;

    }
}
