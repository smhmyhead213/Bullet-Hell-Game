using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.MediaFoundation;

namespace bullethellwhatever.Projectiles.Enemy
{
    public class WeakHomingProjectile : Projectile
    {
        public float MoveSpeed;
        public int HomingStopTime;

        public WeakHomingProjectile(float moveSpeed, int timeToStopHomingAndFlyAway)
        {
            MoveSpeed = moveSpeed;
            HomingStopTime = timeToStopHomingAndFlyAway;
        }
        public override void AI()
        {
            TimeAlive++;

            if (TimeAlive < HomingStopTime)
            {
                if (!IsHarmful)
                {
                    NPC closestNPC = new NPC(); //the target
                    float minDistance = float.MaxValue;
                    

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


                    //Velocity = 0.4f * Vector2.Normalize(closestNPC.Position - Position) * (TimeAlive - 30f);

                    Velocity = MoveSpeed * Utilities.SafeNormalise(Vector2.Lerp(Velocity, vectorToTarget, 0.003f), Vector2.Zero);
                }
                else
                {
                    Vector2 vectorToTarget = Main.player.Position - Position; //get a vector to the target


                    //Velocity = 0.4f * Vector2.Normalize(closestNPC.Position - Position) * (TimeAlive - 30f);

                    Velocity = MoveSpeed * Utilities.SafeNormalise(Vector2.Lerp(Velocity, vectorToTarget, 0.003f), Vector2.Zero);
                }
            }
            else
            {
                Colour = Color.White;
                Velocity = Velocity * 1.02f;
            }

            Position = Position + Velocity;
        }
    }
}
