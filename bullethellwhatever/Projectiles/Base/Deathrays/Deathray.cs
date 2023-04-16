using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using Microsoft.VisualBasic.Devices;

using System.Diagnostics.Eventing.Reader;
using SharpDX.MediaFoundation;

namespace bullethellwhatever.Projectiles.Base
{
    public class Deathray : Projectile
    {
        public Vector2 Origin;     
        public float Length;
        public float Width;
        public float AngularVelocity;
        public virtual void Spawn(Vector2 origin, float initialRotation, float damage, Texture2D texture, float width, float length, float angularVelocity, float angularAcceleration)
        {
            Origin = origin;
            Rotation = initialRotation;
            Width = width;
            Length = length;
            Texture = texture;
            AngularVelocity = angularVelocity;
            Acceleration = angularAcceleration; //Acceleration works DIFFERENTLY for rays.

            Main.enemyProjectilesToAddNextFrame.Add(this);
        }

        public override bool ShouldRemoveOnEdgeTouch() => false;

        public override void AI()
        {
            TimeAlive++;

            AngularVelocity = AngularVelocity + Acceleration;

            Rotation = (Rotation + MathF.PI * AngularVelocity / 21600f) % (MathF.PI * 2); //The rotation is always 0 < r < 360.
            
            //if (TimeAlive % AngularVelocity * 2 == 0) // Reset rotation after the beam has gone a full turn. This will need to be adjusted for rays whose Angular Velocity changes with time.
            //{
            //    Rotation = 0;
            //}
        }

        //I HATE DEATHRAY COLLISION, I HATE DEATHRAY COLLISION, I HATE DEATHRAY COLLISION, I HATE DEATHRAY COLLISION
        public override bool IsCollidingWithEntity(Projectile projectile, Entity entity) //dont forget to add a check to see if the player is within the beams length, to ensure that the beam doesnt have infinite range
        {
            //return IsAnXCoordinateInTheBeam(entity.Position.X, this) && IsAYCoordinateInTheBeam(entity.Position.X, entity.Position.Y, this);
            return IsThePlayerInTheBeam(entity.Position.X, entity.Position.Y, this);
        }

        public static bool IsThePlayerInTheBeam(float xcoord, float ycoord, Deathray deathray)
        {
            if (Utilities.DistanceBetweenVectors(new Vector2(xcoord, ycoord), deathray.Origin) < deathray.Length)
            {
                Vector2 playerVector = new Vector2(xcoord, ycoord);

                //Find the angle between the player and the vertical using the dot/scalar product.

                float dotProduct = Vector2.Dot(Vector2.UnitY, playerVector - deathray.Origin);

                float angle = MathF.PI - MathF.Acos(dotProduct / (playerVector - deathray.Origin).Length());


                // For the collision checks, small degrees of error like PI / 90 are used to account for Angle and Rotation never being exactly equal due to floating point jank.

                if (deathray.Rotation >= 0) // Check if the beam is moving clockwise. In this case, Rotation is positive.
                {
                    if (playerVector.X >= deathray.Origin.X) // If the player is on the right, do no adjustments as Rotation and Angle here are both angles clockwise from the vertical.
                    {
                        return deathray.Rotation > angle - MathF.PI / 90f && deathray.Rotation < angle + Math.PI / 90f;
                    }

                    else // Otherwise, adjust Angle to be an angle clockwise from the vertical due to the dot products nature of giving an angle 0 < theta < 180.
                    {
                        return deathray.Rotation > MathF.PI * 2 - angle - MathF.PI / 90f && deathray.Rotation < MathF.PI * 2 - angle + Math.PI / 90f;
                    }
                }

                else // The beam is moving counter-clockwise. Rotation is negative.
                {
                    if (playerVector.X >= deathray.Origin.X) // If the player is on the right, colliding values of Angle and Rotation differ by a full 360 degrees. 
                    {
                        return deathray.Rotation > angle - MathF.PI * 2 - MathF.PI / 90f && deathray.Rotation < angle - MathF.PI * 2 + Math.PI / 90f;
                    }

                    else // Otherwise, corresponding values of Angle and Rotation differ by sign.
                    {
                        return MathF.Abs(deathray.Rotation) > angle - MathF.PI / 90f && MathF.Abs(deathray.Rotation) < angle + Math.PI / 90f;
                    }
                }
            }

            else return false;
        }
        public static void DrawDeathray(SpriteBatch spritebatch, Deathray deathray)
        {
            spritebatch.End();

            spritebatch.Begin(SpriteSortMode.Immediate);

            Main.gradientShader.Parameters["uTime"]?.SetValue(deathray.TimeAlive);
            Main.gradientShader.Parameters["AngularVelocity"]?.SetValue(deathray.AngularVelocity);

            Main.gradientShader.CurrentTechnique.Passes[0].Apply();

            Vector2 size = new Vector2(deathray.Width / deathray.Texture.Width, deathray.Length / deathray.Texture.Height); //Scale the beam up to the required width and length.

            Vector2 originOffset = new Vector2(5f, 0f); //i have no idea why the value 5 works everytime i have genuinely no clue

            spritebatch.Draw(Main.player.Texture, deathray.Origin, null, Color.LightBlue, MathF.PI + deathray.Rotation, originOffset, size, SpriteEffects.None, 0);

            spritebatch.End();

            spritebatch.Begin(SpriteSortMode.Deferred);

            Utilities.drawTextInDrawMethod(deathray.Rotation.ToString(), new Vector2(Main._graphics.PreferredBackBufferWidth / 4, Main._graphics.PreferredBackBufferHeight / 4), spritebatch, Main.font, Color.White);

            Utilities.drawTextInDrawMethod(deathray.AngularVelocity.ToString(), new Vector2(Main._graphics.PreferredBackBufferWidth / 4 * 3, Main._graphics.PreferredBackBufferHeight / 4), spritebatch, Main.font, Color.White);

        }
    }
}
