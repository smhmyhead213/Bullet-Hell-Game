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

        public override void Spawn(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            base.Spawn(position, velocity, damage, pierce, texture, acceleration, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);

            afterimagesPositions = new Vector2[22 * Updates];

            DrawAfterimages = true;
        }
        public override void AI()
        {
            SetUpdates(2);

            HomingTime = 30 * Updates;

            NPC closestNPC = new NPC(); //the target
            float minDistance = float.MaxValue;         

            if (AITimer > HomingTime)
            {
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
                    }
                }
                //Vector2 vectorToTarget = closestNPC.Position - Main.player.Position; //get a vector to the target
                Velocity = 0.4f / Updates * Vector2.Normalize(closestNPC.Position - Position) * (AITimer - HomingTime);
            }

            Position = Position + Velocity;
        }

    }
}
