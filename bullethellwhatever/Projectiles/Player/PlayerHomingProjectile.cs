using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Player;

using System.Collections.Generic;



namespace bullethellwhatever.Projectiles.Player
{
    public class PlayerHomingProjectile : Projectile
    {
        public float HomingFactor; //how strong the homing is
        public Vector2[] afterimagesPositions = new Vector2[22];
        public bool ChosenDirection;
        public int Direction;
        public int HomingTime => 30;
        public override void AI()
        {
            NPC closestNPC = new NPC(); //the target
            float minDistance = float.MaxValue;
            TimeAlive++;

            afterimagesPositions = Utilities.moveVectorArrayElementsUpAndAddToStart(afterimagesPositions, Position);

            if (TimeAlive > HomingTime)
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
                Velocity = 0.4f * Vector2.Normalize(closestNPC.Position - Position) * (TimeAlive - 30f);
            }

            Position = Position + Velocity;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Drawing.BetterDraw(Main.player.Texture, Position, null, Colour * Opacity, Rotation, Size, SpriteEffects.None, 0f);

            for (int i = 0; i < afterimagesPositions.Length; i++)
            {
                if (afterimagesPositions[i] != Vector2.Zero)
                {
                    float colourMultiplier = (float)(afterimagesPositions.Length - (i + 1)) / (float)(afterimagesPositions.Length + 1) - 0.2f;
                    Drawing.BetterDraw(Main.player.Texture, afterimagesPositions[i], null, Colour * colourMultiplier, Rotation, Size * (afterimagesPositions.Length - 1 - i) / afterimagesPositions.Length, SpriteEffects.None, 0f); //draw afterimages
                }
            }
        }

    }
}
