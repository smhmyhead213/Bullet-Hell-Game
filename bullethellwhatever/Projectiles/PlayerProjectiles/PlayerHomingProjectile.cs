using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Player;
using bullethellwhatever.DrawCode;
using System.Collections.Generic;

namespace bullethellwhatever.Projectiles.Player
{
    public class PlayerHomingProjectile : Projectile
    {
        public float HomingFactor; //how strong the homing is
        public bool ChosenDirection;
        public int Direction;
        public int HomingTime;
        public int TimeWithNoTarget;

        public override void Spawn(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            base.Spawn(position, velocity, damage, pierce, texture, acceleration, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);

            SetUpdates(2);

            SetDrawAfterimages(22);

            TimeWithNoTarget = 0;
        }

        public override void OnHitEffect(Vector2 position)
        {
            if (Owner == player)
            {
                int numberOfParticles = Utilities.RandomInt(1, 4);

                for (int i = 0; i < numberOfParticles; i++)
                {
                    float rotation = Utilities.RandomFloat(0, Tau);

                    Particle p = new Particle();

                    Vector2 velocity = 10f * Utilities.RotateVectorClockwise(-Vector2.UnitY, rotation);
                    int lifetime = 20;

                    p.Spawn("box", position, velocity, -velocity / 2f / lifetime, Vector2.One * 0.45f, rotation, Colour, 1f, 20);
                }
            }

            base.OnHitEffect(position);
        }
        public override void AI()
        {
            HomingTime = 30 * Updates;

            NPC closestNPC = new NPC(); //the target
            float minDistance = float.MaxValue;         

            if (AITimer > HomingTime)
            {
                bool validTarget = false;

                if (activeNPCs.Count > 0)
                {
                    Opacity = 1f; // come back to full opacity if a target is found while fading out

                    foreach (NPC npc in activeNPCs)
                    {
                        if (npc.TargetableByHoming)
                        {
                            float distance = Utilities.DistanceBetweenEntities(this, npc);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                closestNPC = npc;
                            }

                            validTarget = true;
                        }
                    }

                    if (validTarget)
                        Velocity = 0.4f / Updates * Vector2.Normalize(closestNPC.Position - Position) * (AITimer - HomingTime);
                }

                else
                {
                    TimeWithNoTarget++;
                    Velocity = Velocity * 0.98f; // slow down if no target
                }

                if (!validTarget)
                {
                    TimeWithNoTarget++;
                    Velocity = Velocity * 0.98f; // slow down if no target
                }

                //Vector2 vectorToTarget = closestNPC.Position - Main.player.Position; //get a vector to the target
               
            }

            int beginFading = 50;

            if (TimeWithNoTarget >= beginFading) //after a second of not finding a target
            {
                // if almost expired, start fading out

                int fadeOutTime = 60;

                Opacity = MathHelper.Lerp(1f, 0f, ((float)TimeWithNoTarget - beginFading) / fadeOutTime);

                if (TimeWithNoTarget > fadeOutTime + beginFading)
                {
                    DeleteNextFrame = true;

                    OnHitEffect(Position);
                }
            }

            Rotation = Utilities.VectorToAngle(Velocity);

            Position = Position + Velocity;
        }

    }
}
