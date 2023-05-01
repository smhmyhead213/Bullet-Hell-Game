using Microsoft.Xna.Framework;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Player;
using Microsoft.Xna.Framework.Graphics;


namespace bullethellwhatever.Projectiles.Player
{
    public class PlayerHomingProjectile : Projectile
    {
        public float HomingFactor; //how strong the homing is
        public Vector2[] afterimagesPositions = new Vector2[22];
        public override void AI()
        {
            afterimagesPositions = Utilities.moveVectorArrayElementsUpAndAddToStart(afterimagesPositions, Position);

            NPC closestNPC = new NPC(); //the target
            float minDistance = float.MaxValue;
            TimeAlive++;

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
                //Vector2 vectorToTarget = closestNPC.Position - Main.player.Position; //get a vector to the target
                Velocity = Vector2.Normalize(closestNPC.Position - Position) * (TimeAlive - 30f);
            }

            Position = Position + Velocity;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Drawing.BetterDraw(Main.player.Texture, Position, null, Colour, Rotation, Size, SpriteEffects.None, 0f);

            for (int i = 0; i < afterimagesPositions.Length; i++)
            {
                if (afterimagesPositions[i] != Vector2.Zero)
                {
                    float colourMultiplier = ((float)(afterimagesPositions.Length - (i + 1)) / (float)(afterimagesPositions.Length + 1));
                    Drawing.BetterDraw(Main.player.Texture, afterimagesPositions[i], null, Colour * colourMultiplier, Rotation, Size * (afterimagesPositions.Length - 1 - i) / afterimagesPositions.Length, SpriteEffects.None, 0f); //draw afterimages
                }
            }
        }

    }
}
