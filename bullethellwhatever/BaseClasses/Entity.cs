global using static bullethellwhatever.MainFiles.Main;

using Microsoft.Xna.Framework;
using bullethellwhatever.DrawCode;
using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using bullethellwhatever.Projectiles.TelegraphLines;
using System.Collections.Generic;


namespace bullethellwhatever.BaseClasses
{
    public abstract class Entity //wait til he finds out he has to rework the hitbox system to work with different square sizes! get a load of this guy!
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Texture2D Texture;
        public float Depth;
        public bool DrawAfterimages;
        public bool isBoss;
        public bool isPlayer;
        public bool DealDamage;
        public bool IsHarmful;
        public int AITimer;
        public float Damage;
        public float Opacity;
        public RotatedRectangle Hitbox; //this hitbox system works only with squares, if you want to expand make Size a Vector2

        public Vector2 Size;

        public float RotationalVelocity;

        public float Health;
        public float MaxHP;
        public bool DeleteNextFrame;
        public float Rotation;
        public bool IsDesperationOver;
        public bool ShouldRemoveOnEdgeTouch;
        public Color Colour;
        public List<TelegraphLine> activeTelegraphs = new List<TelegraphLine>();
        public Effect? Shader;
        public int Updates;

        public Action OnDeath;
        public Action ExtraAI;

        public float[] afterimagesRotations;
        public Vector2[] afterimagesPositions;

        public virtual void Update()
        {
            if (DrawAfterimages)
            {
                Utilities.moveArrayElementsUpAndAddToStart(ref afterimagesPositions, Position);
                Utilities.moveArrayElementsUpAndAddToStart(ref afterimagesRotations, Rotation);
            }
        }
        public virtual void SetUpdates(int updates)
        {
            Updates = updates;
        }

        public virtual void SetSizeNoScaling(Vector2 size)
        {
            Size = size;
        }
        public virtual void SetDepth(float depth)
        {
            Depth = MathHelper.Clamp(depth, 0f, 1f);
        }

        public virtual float DepthFactor()
        {
            return MathHelper.Lerp(1, 0.1f, Depth);
        }
        public virtual Vector2 GetSize() // get a size that corresponds to the current depth
        {
            return Vector2.LerpPrecise(Size, Size / 10f, Depth) * ScaleFactor();
        }

        public void SetVelocity(Vector2 vel)
        {
            Velocity = vel;
        }

        public void SetVelocityX(float x)
        {
            Velocity.X = x;
        }

        public void SetVelocityY(float y)
        {
            Velocity.Y = y;
        }

        public static void DustExplosion(int numberOfParticles, Vector2 position, Color colour)
        {
            for (int i = 0; i < numberOfParticles; i++)
            {
                float rotation = Utilities.RandomFloat(0, Tau);

                Particle p = new Particle();

                Vector2 velocity = 10f * Utilities.RotateVectorClockwise(-Vector2.UnitY, rotation);
                int lifetime = 20;

                p.Spawn("box", position, velocity, -velocity / 2f / lifetime, Vector2.One * 0.45f, rotation, colour, 1f, 20);
            }
        }

        public static bool touchingBottom(Entity entity) //hieght is height of texture
        {
            if (entity.Position.Y + entity.Texture.Height * entity.GetSize().Y / 2 >= _graphics.PreferredBackBufferHeight)
                return true;
            else return false;
            //if at the bottom
        }

        public static bool touchingTop(Entity entity)
        {
            if (entity.Position.Y  - entity.Texture.Height * entity.GetSize().Y / 2 <= 0)
                return true;
            else return false;
        }

        public static bool touchingRight(Entity entity)
        {
            if (entity.Position.X + entity.Texture.Width * entity.GetSize().X / 2 >= _graphics.PreferredBackBufferWidth)
                return true;
            else return false;
        }

        public static bool touchingLeft(Entity entity)
        {
            if (entity.Position.X - entity.Texture.Width * entity.GetSize().X / 2 <= 0)
                return true;
            else return false;
        }

        public static bool touchingAnEdge(Entity entity)
        {
            if (touchingTop(entity) || touchingBottom(entity) ||
                    touchingLeft(entity) || touchingRight(entity))
            {
                return true;
            }

            return false;
        }
        public abstract void AI();

        public virtual void SetOnDeath(Action action)
        {
            OnDeath = action;
        }
        public virtual void SetExtraAI(Action action)
        {
            ExtraAI = action;
        }
        public virtual void Die()
        {
            //death behaviour

            if (OnDeath is not null)
            {
                OnDeath();
            }

            DeleteNextFrame = true;
        }

        public virtual void SetDrawAfterimages(int length)
        {
            DrawAfterimages = true;

            afterimagesPositions = new Vector2[length];
            afterimagesRotations = new float[length];
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Drawing.BetterDraw(Texture, Position, null, Colour * Opacity, Rotation, GetSize(), SpriteEffects.None, 0f);

            if (DrawAfterimages)
            {
                DrawAfterImages();
            }
        }

        public virtual void DrawAfterImages()
        {
            for (int i = 0; i < afterimagesPositions.Length; i++)
            {
                float colourMultiplier = (float)(afterimagesPositions.Length - (i + 1)) / (float)(afterimagesPositions.Length + 1) - 0.2f;

                if (afterimagesPositions[i] != Vector2.Zero)
                {
                    Drawing.BetterDraw(Texture, afterimagesPositions[i], null, Colour * colourMultiplier * Opacity, afterimagesRotations[i], GetSize() * (afterimagesPositions.Length - 1 - i) / afterimagesPositions.Length, SpriteEffects.None, 0f); //draw afterimages

                    // Draw another afterimage between this one and the last one, for a less choppy trail.
                    // The first afterimage is between the entity and the first saved position (i = 0).

                    Vector2 positionOfAdditionalAfterImage = i == 0 ? Vector2.Lerp(Position, afterimagesPositions[i], 0.5f) : Vector2.Lerp(afterimagesPositions[i - 1], afterimagesPositions[i], 0.5f);

                    colourMultiplier = (float)(afterimagesPositions.Length - (i + 1) + 0.5f) / (float)(afterimagesPositions.Length + 1) - 0.2f;

                    float rotation = i != afterimagesRotations.Length - 1 ? MathHelper.Lerp(afterimagesRotations[i], afterimagesRotations[i + 1], 0.5f) : afterimagesRotations[i];

                    Drawing.BetterDraw(Texture, positionOfAdditionalAfterImage, null, Colour * colourMultiplier * Opacity,
                        rotation, GetSize() * (afterimagesPositions.Length - 1 - i + 0.5f) / afterimagesPositions.Length, SpriteEffects.None, 0f); //draw afterimages

                }
            }
        }
        public void SetHitbox()
        {
            UpdateHitbox();
        }
        public virtual void UpdateHitbox() //call this before everything else so after AIs proper hitboxes get sent to EntityManager
        {
            Hitbox.UpdateRectangle(Rotation, Texture.Width * GetSize().X, Texture.Height * GetSize().Y, Position);

            Hitbox.UpdateVertices();
        }



    }
}
