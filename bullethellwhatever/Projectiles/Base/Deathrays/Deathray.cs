﻿using System;
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
        public float Length;
        public float Width;
        public float AngularVelocity;
        public bool IsActive;
        public Effect Shader;
        public int Duration;
        public virtual void SpawnDeathray(Vector2 position, float initialRotation, float damage, int duration, Texture2D texture, float width, float length, float angularVelocity, float angularAcceleration, bool isHarmful, Color colour, Effect shader, Entity owner)
        {
            Position = position;
            Rotation = initialRotation;
            Width = width;
            Length = length;
            Duration = duration;
            Texture = texture;
            AngularVelocity = angularVelocity;
            Acceleration = angularAcceleration; //Acceleration works DIFFERENTLY for rays.
            Owner = owner;
            Colour = colour;
            IsActive = true;
            IsHarmful = isHarmful;
            Damage = damage;
            Shader = shader;
            RemoveOnHit = false;

            if (isHarmful)
                Main.enemyProjectilesToAddNextFrame.Add(this);
            else Main.friendlyProjectilesToAddNextFrame.Add(this);
        }

        public override void AI()
        {
            TimeAlive++;

            AngularVelocity = AngularVelocity + Acceleration;

            Rotation = (Rotation + MathF.PI * AngularVelocity / 21600f) % (MathF.PI * 2); //The rotation is always 0 < r < 360.
            
            if (TimeAlive > Duration && Owner is not BaseClasses.Player)
            {
                DeleteNextFrame = true;
            }
        }

        //I HATE DEATHRAY COLLISION, I HATE DEATHRAY COLLISION, I HATE DEATHRAY COLLISION, I HATE DEATHRAY COLLISION
        public override bool IsCollidingWithEntity(Entity entity) //dont forget to add a check to see if the player is within the beams length, to ensure that the beam doesnt have infinite range
        {
            //return IsAnXCoordinateInTheBeam(entity.Position.X, this) && IsAYCoordinateInTheBeam(entity.Position.X, entity.Position.Y, this);
            return IsTheTargetInTheBeam(entity, this);
        }

        public static bool IsTheTargetInTheBeam(Entity entity, Deathray deathray) 
        {
            if (deathray.IsActive)
            {
                if (Utilities.DistanceBetweenVectors(new Vector2(entity.Position.X, entity.Position.Y), deathray.Position) < deathray.Length)
                {
                    Vector2 playerVector = new Vector2(entity.Position.X, entity.Position.Y);

                    // Find the angle between the player and the vertical using the dot/scalar product.

                    float dotProduct = Vector2.Dot(Vector2.UnitY, playerVector - deathray.Position);
                    
                    // Find the angle that the deathray and entity must share approximtely for a collision.
                    
                    float angle = MathF.PI - MathF.Acos(dotProduct / (playerVector - deathray.Position).Length());
                    
                    // Find the diagonal length of the entity to collide with.
                    
                    float diagonalOfTarget = MathF.Sqrt(MathF.Pow(entity.Hitbox.Width, 2) + MathF.Pow(entity.Hitbox.Height, 2));
                    
                    // Find the appropriate angle tolerance needed to perfectly encapsulate the entity.
                    
                    float angleTolerance = MathF.Atan(diagonalOfTarget / 2 / Utilities.DistanceBetweenVectors(deathray.Position, entity.Position));

                    // For the collision checks, small degrees of error are used to account for Angle and Rotation never being exactly equal due to floating point jank.

                    if (deathray.Rotation >= 0) // Check if the beam is moving clockwise. In this case, Rotation is positive.
                    {
                        if (playerVector.X >= deathray.Position.X) // If the player is on the right, do no adjustments as Rotation and Angle here are both angles clockwise from the vertical.
                        {
                            return deathray.Rotation > angle - angleTolerance && deathray.Rotation < angle + angleTolerance;
                        }

                        else // Otherwise, adjust Angle to be an angle clockwise from the vertical due to the dot products nature of giving an angle 0 < theta < 180.
                        {
                            return deathray.Rotation > MathF.PI * 2 - angle - angleTolerance && deathray.Rotation < MathF.PI * 2 - angle + angleTolerance;
                        }
                    }

                    else // The beam is moving counter-clockwise. Rotation is negative.
                    {
                        if (playerVector.X >= deathray.Position.X) // If the player is on the right, colliding values of Angle and Rotation differ by a full 360 degrees. 
                        {
                            return deathray.Rotation > angle - MathF.PI * 2 - angleTolerance && deathray.Rotation < angle - MathF.PI * 2 + angleTolerance;
                        }

                        else // Otherwise, corresponding values of Angle and Rotation differ by sign.
                        {
                            return MathF.Abs(deathray.Rotation) > angle - angleTolerance && MathF.Abs(deathray.Rotation) < angle + angleTolerance;
                        }
                    }
                }

                else return false;
            }

            return false;
        }

        //public override void DealDamage()
        //{
        //    foreach (NPC npc in Main.activeNPCs)
        //    {
        //        if (npc.IsHarmful != IsHarmful)
        //        {
        //            if (IsCollidingWithEntity(this, npc) && npc.IFrames == 0)
        //            {
        //                if (npc.IFrames == 0)
        //                {
        //                    npc.IFrames = 5f;

        //                    npc.Health = npc.Health - Damage;
        //                }
        //            }
        //        }
        //    }
        //}

        public override void Draw(SpriteBatch spritebatch)
        {
            if (IsActive)
            {
                spritebatch.End();

                spritebatch.Begin(SpriteSortMode.Immediate);

                //List<VertexPosition2DColor> vertices = new();
                //// either set this here or use one you already have.
                //// Whatever it is for you.
                //Vector2 laserEndPos = Origin + (Utilities.AngleToVector(Rotation) * Length);
                //// set this to the rotation the laser is facing times the length added to the start pos

                //for (int i = 0; i < 8; i++)
                //{
                //    float progress = (float)i / 8f;

                //    Vector2 position = Vector2.Lerp(Origin, laserEndPos, progress) + Utilities.Normalise(Utilities.RotateVectorClockwise(laserEndPos - Origin, -MathHelper.PiOver2)) * Width;
                //    Vector2 position2 = Vector2.Lerp(Origin, laserEndPos, progress) + Utilities.Normalise(Utilities.RotateVectorClockwise(laserEndPos - Origin, MathHelper.PiOver2)) * Width;

                //    Vector2 textureCoords = new(progress, 0f);
                //    Vector2 textureCoords2 = new(progress, 1f);

                //    vertices.Add(new VertexPosition2DColor(position, Colour, textureCoords));
                //    vertices.Add(new VertexPosition2DColor(position2, Colour, textureCoords2));
                //}

                Shader.Parameters["uTime"]?.SetValue(TimeAlive);
                Shader.Parameters["duration"]?.SetValue(Duration);

                Main._graphics.GraphicsDevice.Textures[1] = Main.deathrayNoiseMap;

                Shader.CurrentTechnique.Passes[0].Apply();

                Vector2 size = new Vector2(Width / Texture.Width, Length / Texture.Height); //Scale the beam up to the required width and length.

                //spritebatch.Draw(Main.player.Texture, Position, null, Colour, MathF.PI + Rotation, Vector2.Zero, size, SpriteEffects.None, 0);

                Vector2 originOffset = new Vector2(Texture.Width / 2, 0f); //i have no idea why the value 5 works everytime i have genuinely no clue

                spritebatch.Draw(Main.player.Texture, Position, null, Colour, MathF.PI + Rotation, originOffset, size, SpriteEffects.None, 0);

                //Drawing.BetterDraw(Main.player.Texture, Position, null, Colour, MathF.PI + Rotation, size, SpriteEffects.None, 0);

                //Main._graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

                spritebatch.End();

                spritebatch.Begin(SpriteSortMode.Deferred);

            }
        }
    }
}
