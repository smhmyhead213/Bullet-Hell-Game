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
        public int HomingTime => 30;

        public override void Spawn(Vector2 position, Vector2 velocity, float damage, string texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            base.Spawn(position, velocity, damage, texture, acceleration, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);

            afterimagesPositions = new Vector2[22]; 
        }
        public override void AI()
        {
            NPC closestNPC = new NPC(); //the target
            float minDistance = float.MaxValue;

            Utilities.moveVectorArrayElementsUpAndAddToStart(ref afterimagesPositions, Position);

            if (AITimer > HomingTime)
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
                //Vector2 vectorToTarget = closestNPC.Position - Main.player.Position; //get a vector to the target
                Velocity = 0.4f * Vector2.Normalize(closestNPC.Position - Position) * (AITimer - HomingTime);
            }

            Position = Position + Velocity;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Drawing.BetterDraw(Main.player.Texture, Position, null, Colour * Opacity, Rotation, Size, SpriteEffects.None, 0f);

            for (int i = 0; i < afterimagesPositions.Length; i++)
            {
                float colourMultiplier = (float)(afterimagesPositions.Length - (i + 1)) / (float)(afterimagesPositions.Length + 1) - 0.2f;

                if (afterimagesPositions[i] != Vector2.Zero)
                {
                    Drawing.BetterDraw(Main.player.Texture, afterimagesPositions[i], null, Colour * colourMultiplier, Rotation, Size * (afterimagesPositions.Length - 1 - i) / afterimagesPositions.Length, SpriteEffects.None, 0f); //draw afterimages

                    // Draw another afterimage between this one and the last one, for a less choppy trail.

                    Vector2 positionOfAdditionalAfterImage = i == 0 ? Vector2.Lerp(Position, afterimagesPositions[i], 0.5f) : Vector2.Lerp(afterimagesPositions[i - 1], afterimagesPositions[i], 0.5f);

                    colourMultiplier = (float)(afterimagesPositions.Length - (i + 1) + 0.5f) / (float)(afterimagesPositions.Length + 1) - 0.2f;

                    Drawing.BetterDraw(Main.player.Texture, positionOfAdditionalAfterImage, null, Colour * colourMultiplier,
                        Rotation, Size * (afterimagesPositions.Length - 1 - i + 0.5f) / afterimagesPositions.Length, SpriteEffects.None, 0f); //draw afterimages

                }
            }
        }

    }
}
