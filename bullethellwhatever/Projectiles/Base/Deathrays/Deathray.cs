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
using bullethellwhatever.DrawCode;
using bullethellwhatever.BaseClasses.Hitboxes;

namespace bullethellwhatever.Projectiles.Base
{
    public class Deathray : Projectile
    {    
        public float Length;
        public float Width;
        public float InitialWidth;
        public bool ThinOut;
        public float AngularVelocity;
        public bool IsActive;
        public bool IsSpawned;
        public int Duration;
        public bool StayWithOwner;

        public virtual void SpawnDeathray(Vector2 position, float initialRotation, float damage, int duration, string texture, float width,
            float length, float angularVelocity, float angularAcceleration, bool isHarmful, Color colour, string? shader, Entity owner)
        {
            CreateDeathray(position, initialRotation, damage, duration, texture, width, length, angularVelocity, angularAcceleration, isHarmful, colour, shader, owner);

            AddDeathrayToActiveProjectiles();
        }

        public virtual void SpawnDeathray(Vector2 position, float initialRotation, float damage, int duration, string texture, float width,
            float length, float angularVelocity, float angularAcceleration, bool isHarmful, Color colour, Effect shader, Entity owner)
        {
            CreateDeathray(position, initialRotation, damage, duration, texture, width, length, angularVelocity, angularAcceleration, isHarmful, colour, shader, owner);

            AddDeathrayToActiveProjectiles();
        }

        public virtual void SpawnDeathray(Vector2 position, float initialRotation, float damage, int duration, Texture2D texture, float width,
    float length, float angularVelocity, float angularAcceleration, bool isHarmful, Color colour, Effect shader, Entity owner)
        {
            CreateDeathray(position, initialRotation, damage, duration, texture, width, length, angularVelocity, angularAcceleration, isHarmful, colour, shader, owner);

            AddDeathrayToActiveProjectiles();
        }

        public virtual Deathray CreateDeathray(Vector2 position, float initialRotation, float damage, int duration, string texture, float width,
            float length, float angularVelocity, float angularAcceleration, bool isHarmful, Color colour, string? shader, Entity owner)
        {
            Updates = 1;

            Position = position;
            Rotation = initialRotation;

            Width = width;
            Length = length;

            Hitbox = new RotatedRectangle(Rotation, Width, Length, Position - Utilities.RotateVectorClockwise(new Vector2(0f, Length / 2f), Rotation), this);

            SetHitbox();
            
            Duration = duration;
            Texture = Assets[texture];
            AngularVelocity = angularVelocity;
            Acceleration = angularAcceleration; //Acceleration works DIFFERENTLY for rays.
            Owner = owner;
            Colour = colour;
            IsActive = true;
            IsHarmful = isHarmful;
            Damage = damage;

            if (shader != null)
                Shader = Shaders[shader];
            else Shader = null;

            RemoveOnHit = false;

            InitialWidth = Width;
            ThinOut = false;

            return this;
        }

        public virtual Deathray CreateDeathray(Vector2 position, float initialRotation, float damage, int duration, string texture, float width,
            float length, float angularVelocity, float angularAcceleration, bool isHarmful, Color colour, Effect? shader, Entity owner)
        {
            Updates = 1;

            Position = position;
            Rotation = initialRotation;

            Width = width;
            Length = length;

            Hitbox = new RotatedRectangle(Rotation, Width, Length, Position - Utilities.RotateVectorClockwise(new Vector2(0f, Length / 2f), Rotation), this);

            SetHitbox();

            Duration = duration;
            Texture = Assets[texture];
            AngularVelocity = angularVelocity;
            Acceleration = angularAcceleration; //Acceleration works DIFFERENTLY for rays.
            Owner = owner;
            Colour = colour;
            IsActive = true;
            IsHarmful = isHarmful;
            Damage = damage;

            if (shader != null)
                Shader = shader;
            else Shader = null;

            RemoveOnHit = false;

            InitialWidth = Width;
            ThinOut = false;

            return this;
        }

        public virtual void CreateDeathray(Vector2 position, float initialRotation, float damage, int duration, Texture2D texture, float width,
            float length, float angularVelocity, float angularAcceleration, bool isHarmful, Color colour, Effect? shader, Entity owner)
        {
            Updates = 1;

            Position = position;
            Rotation = initialRotation;

            Width = width;
            Length = length;

            Hitbox = new RotatedRectangle(Rotation, Width, Length, Position - Utilities.RotateVectorClockwise(new Vector2(0f, Length / 2f), Rotation), this);

            SetHitbox();

            Duration = duration;
            Texture = texture;
            AngularVelocity = angularVelocity;
            Acceleration = angularAcceleration; //Acceleration works DIFFERENTLY for rays.
            Owner = owner;
            Colour = colour;
            IsActive = true;
            IsHarmful = isHarmful;
            Damage = damage;

            if (shader != null)
                Shader = shader;
            else Shader = null;

            RemoveOnHit = false;

            InitialWidth = Width;
            ThinOut = false;
        }

        //public virtual void SetNoiseMap(string fileName, float scrollSpeed)
        //{
        //    Map = Assets[fileName];
        //    ScrollSpeed = scrollSpeed;
        //}
        public virtual void SetStayWithOwner(bool stay)
        {
            StayWithOwner = stay;
        }
        public virtual void AddDeathrayToActiveProjectiles()
        {
            if (!IsSpawned)
            {
                IsSpawned = true;
                if (IsHarmful)
                {
                    enemyProjectilesToAddNextFrame.Add(this);
                }
                else friendlyProjectilesToAddNextFrame.Add(this);
            }
        }
        public override void AI()
        {
            if (ExtraAI is not null)
            {
                ExtraAI();
            }

            if (Acceleration != 0f)
                AngularVelocity = AngularVelocity + Acceleration;

            Rotation = Rotation + AngularVelocity;
            Rotation = Rotation % (PI * 2); //The rotation is always 0 < r < 360.
                
            while (Rotation < 0f)
            {
                Rotation = Rotation + PI * 2f; //you got a better idea?
            }

            if (StayWithOwner)
            {
                Position = Owner.Position;
            }

            int fadeOutTime = 20;

            if (ThinOut && AITimer > Duration - fadeOutTime)
            {
                Width = MathHelper.Lerp(0, Width, (float)(Duration - AITimer) / fadeOutTime);
            }

            if (AITimer > Duration && Owner is not BaseClasses.Player)
            {
                Die();
            }

            if (BeamEdgeTouch().Collided && OnEdgeTouch is not null) // if we are touching an edge
            {
                OnEdgeTouch();
            }
        }
        public void DrawWithShader(SpriteBatch spriteBatch)
        {

        }
        public override void UpdateHitbox()
        {
            Vector2 centre = Position - Utilities.RotateVectorClockwise(new Vector2(0f, Length / 2f), Rotation);
            
            Hitbox.UpdateRectangle(Rotation, Width, Length, centre);

            Hitbox.UpdateVertices();
        }

        public override void CheckForHits()
        {
            if (IsActive) //only bother to check for collision if active
            {
                base.CheckForHits();
            }          
        }

        public Collision BeamEdgeTouch()
        {
            Vector2 bucket = EdgeTouchPointGivenX(ScreenWidth);

            if (Hitbox.IsVec2WithinMyRectangle(bucket) && Utilities.IsVectorWithinScreen(bucket)) // touching right?
            {
                return new Collision(EdgeTouchPointGivenX(ScreenWidth), true);
            }

            bucket = EdgeTouchPointGivenX(0);

            if (Hitbox.IsVec2WithinMyRectangle(bucket) && Utilities.IsVectorWithinScreen(bucket)) // toucing left?
            {
                return new Collision(EdgeTouchPointGivenX(0), true);
            }

            bucket = EdgeTouchPointGivenY(0);

            if (Hitbox.IsVec2WithinMyRectangle(bucket) && Utilities.IsVectorWithinScreen(bucket)) // touching top?
            {
                return new Collision(EdgeTouchPointGivenY(0), true);
            }

            bucket = EdgeTouchPointGivenY(ScreenHeight);

            if (Hitbox.IsVec2WithinMyRectangle(bucket) && Utilities.IsVectorWithinScreen(bucket)) // touching bottom?
            {
                return new Collision(EdgeTouchPointGivenY(ScreenHeight), true);
            }

            else return new Collision(Vector2.Zero, false);
        }

        public Vector2 EdgeTouchPointGivenX(float x)
        {
            return new Vector2(x, Hitbox.CalculateGradient() * x + Hitbox.CalculateC());
        }
        public Vector2 EdgeTouchPointGivenY(float y)
        {
            return new Vector2((y - Hitbox.CalculateC()) / Hitbox.CalculateGradient(), y);
        }
        public override void OnHitEffect(Vector2 position)
        {
            if (Owner == player && AITimer % 3 == 0)
            {
                int numberOfParticles = 1;

                for (int i = 0; i < numberOfParticles; i++)
                {
                    float rotation = Utilities.RandomFloat(0, Tau);

                    Particle p = new Particle();

                    int lifeTime = Utilities.RandomInt(10, 25);

                    p.Spawn("box", position, 7f * Utilities.RotateVectorClockwise(-Vector2.UnitY, rotation), new Vector2(0, 0.6f), Vector2.One * 0.45f, rotation, Colour, 1f, 30);
                }
            }

            base.OnHitEffect(position);
        }
        public override void Die()
        {
            DeleteNextFrame = true;
        }
        public virtual void SetThinOut(bool thinOut)
        {
            ThinOut = thinOut;
        }
        public override void ApplyShaderParameters()
        {
            base.ApplyShaderParameters();
           
            Shader.Parameters["duration"]?.SetValue(Duration);

            Shader.Parameters["colour"]?.SetValue(Colour.ToVector3());

            if (Map is not null)
            {
                Shader.Parameters["noiseMap"]?.SetValue(Map.Texture);
                Shader.Parameters["scrollSpeed"]?.SetValue(Map.ScrollSpeed);
            }            
        }
        public override void Draw(SpriteBatch spritebatch)
        {
            if (BeamEdgeTouch().Collided)
            {
                //Drawing.BetterDraw(Assets["box"], BeamEdgeTouch().CollisionPoint, null, Color.AliceBlue, 0, Vector2.One * 5f, SpriteEffects.None, 1);
            }

            if (IsActive)
            {
                ApplyShaderParameters();

                Shader.CurrentTechnique.Passes[0].Apply();

                Vector2 size = new Vector2(Width / Texture.Width, Length / Texture.Height); // Scale the beam up to the required width and length.

                Vector2 originOffset = new Vector2(Texture.Width / 2, 0f); 

                spritebatch.Draw(Texture, Position, null, Colour, PI + Rotation, originOffset, size, SpriteEffects.None, 0);
            }
        }
    }
}