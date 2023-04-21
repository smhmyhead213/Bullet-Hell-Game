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
        public bool IsActive;
        public virtual void SpawnDeathray(Vector2 origin, float initialRotation, float damage, Texture2D texture, float width, float length, float angularVelocity, float angularAcceleration, Entity owner, bool isHarmful, Color colour)
        {
            Origin = origin;
            Rotation = initialRotation;
            Width = width;
            Length = length;
            Texture = texture;
            AngularVelocity = angularVelocity;
            Acceleration = angularAcceleration; //Acceleration works DIFFERENTLY for rays.
            Owner = owner;
            Colour = colour;
            IsActive = true;
            Damage = damage;

            if (isHarmful)
                Main.enemyProjectilesToAddNextFrame.Add(this);
            else Main.friendlyProjectilesToAddNextFrame.Add(this);
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
            return IsTheTargetInTheBeam(entity, this);
        }

        public static bool IsTheTargetInTheBeam(Entity entity, Deathray deathray) 
        {
            if (deathray.IsActive)
            {
                if (Utilities.DistanceBetweenVectors(new Vector2(entity.Position.X, entity.Position.Y), deathray.Origin) < deathray.Length)
                {
                    Vector2 playerVector = new Vector2(entity.Position.X, entity.Position.Y);

                    // Find the angle between the player and the vertical using the dot/scalar product.

                    float dotProduct = Vector2.Dot(Vector2.UnitY, playerVector - deathray.Origin);
                    
                    // Find the angle that the deathray and entity must share approximtely for a collision.
                    
                    float angle = MathF.PI - MathF.Acos(dotProduct / (playerVector - deathray.Origin).Length());
                    
                    // Find the diagonal length of the entity to collide with.
                    
                    float diagonalOfTarget = MathF.Sqrt(MathF.Pow(entity.Hitbox.Width, 2) + MathF.Pow(entity.Hitbox.Height, 2));
                    
                    // Find the appropriate angle tolerance needed to perfectly encapsulate the entity.
                    
                    float angleTolerance = MathF.Atan(diagonalOfTarget / 2 / Utilities.DistanceBetweenVectors(deathray.Origin, entity.Position));

                    // For the collision checks, small degrees of error are used to account for Angle and Rotation never being exactly equal due to floating point jank.

                    if (deathray.Rotation >= 0) // Check if the beam is moving clockwise. In this case, Rotation is positive.
                    {
                        if (playerVector.X >= deathray.Origin.X) // If the player is on the right, do no adjustments as Rotation and Angle here are both angles clockwise from the vertical.
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
                        if (playerVector.X >= deathray.Origin.X) // If the player is on the right, colliding values of Angle and Rotation differ by a full 360 degrees. 
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

        public override void DealDamage()
        {
            foreach (NPC npc in Main.activeNPCs)
            {
                if (npc.IsHarmful != IsHarmful)
                {
                    if (IsCollidingWithEntity(this, npc) && npc.IFrames == 0)
                    {
                        if (npc.IFrames == 0)
                        {
                            npc.IFrames = 5f;

                            npc.Health = npc.Health - Damage;
                        }
                    }
                }
            }
        }

        public struct VertexPosition2DColor : IVertexType
        {
            public Vector2 _position;
            public Color _color;
            public Vector2 _textureCoordinates;

            public VertexDeclaration VertexDeclaration => _vertexDeclaration;

            // This is used via the interface for the FNA prim drawer to read the information about the point and properly
            // draw our primitive.
            private static readonly VertexDeclaration _vertexDeclaration = new(new VertexElement[]
            {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            });

            public VertexPosition2DColor(Vector2 position, Color color, Vector2 textureCoordinates)
            {
                _position = position;
                _color = color;
                _textureCoordinates = textureCoordinates;
            }
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            if (IsActive)
            {
                spritebatch.End();

                spritebatch.Begin(SpriteSortMode.Immediate);

                List<VertexPosition2DColor> vertices = new();
                // either set this here or use one you already have.
                // Whatever it is for you.
                Vector2 laserEndPos = Origin + (Utilities.AngleToVector(Rotation) * Length);
                    // set this to the rotation the laser is facing times the length added to the start pos

                for (int i = 0; i < 8; i++)
                {
                    float progress = (float)i / 8f;

                    Vector2 position = Vector2.Lerp(Origin, laserEndPos, progress) + Utilities.Normalise(Utilities.RotateVectorClockwise(laserEndPos - Origin, -MathHelper.PiOver2)) * Width;
                    Vector2 position2 = Vector2.Lerp(Origin, laserEndPos, progress) + Utilities.Normalise(Utilities.RotateVectorClockwise(laserEndPos - Origin, MathHelper.PiOver2)) * Width;

                    Vector2 textureCoords = new(progress, 0f);
                    Vector2 textureCoords2 = new(progress, 1f);

                    vertices.Add(new VertexPosition2DColor(position, Colour, textureCoords));
                    vertices.Add(new VertexPosition2DColor(position2, Colour, textureCoords2));
                }

                Main.deathrayShader.Parameters["uTime"]?.SetValue(TimeAlive);
                //Main.deathrayShader.Parameters["noiseMap"]?.SetValue(Main.deathrayNoiseMap);

                Main._graphics.GraphicsDevice.Textures[1] = Main.deathrayNoiseMap;

                Main.deathrayShader.CurrentTechnique.Passes[0].Apply();

                Vector2 size = new Vector2(Width / Texture.Width, Length / Texture.Height); //Scale the beam up to the required width and length.

                Vector2 originOffset = new Vector2(5f, 0f); //i have no idea why the value 5 works everytime i have genuinely no clue

                //spritebatch.Draw(Main.player.Texture, Origin, null, Colour, MathF.PI + Rotation, originOffset, size, SpriteEffects.None, 0);

                Main._graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

                spritebatch.End();

                spritebatch.Begin(SpriteSortMode.Deferred);

            }
        }
    }
}
