using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using bullethellwhatever.MainFiles;
using bullethellwhatever.DrawCode;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.NPCs;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses.Entities;
using bullethellwhatever.DrawCode.Particles;

namespace bullethellwhatever.Projectiles.Base
{
    public class Deathray : Projectile
    {    
        public float Length;
        public float InitialLength;
        public float Width;
        public float InitialWidth;
        private bool ThinOut;
        public float AngularVelocity;
        public bool IsActive;
        public bool IsSpawned;
        public int Duration;
        public bool DieAfterDuration;
        public bool StayWithOwner;
        public int FadeOutTime;
        public virtual void SpawnDeathray(Vector2 position, float initialRotation, float damage, int duration, string texture, float width,
            float length, float angularVelocity, bool harmfulToPlayer, bool harmfulToEnemy, Color colour, string? shader, Entity owner)
        {
            CreateDeathray(position, initialRotation, damage, duration, texture, width, length, angularVelocity, harmfulToPlayer, harmfulToEnemy, colour, shader, owner);

            AddDeathrayToActiveProjectiles();
        }

        public virtual Deathray CreateDeathray(Vector2 position, float initialRotation, float damage, int duration, string texture, float width,
            float length, float angularVelocity, bool harmfulToPlayer, bool harmfulToEnemy, Color colour, string? shader, Entity owner)
        {
            DieAfterDuration = true;

            Updates = 1;

            ContactDamage = harmfulToPlayer;

            Position = position;
            Rotation = initialRotation;

            Width = width;
            Length = length;
            InitialLength = Length;

            SetHitbox();
            
            Duration = duration;
            Texture = AssetRegistry.GetTexture2D(texture);
            AngularVelocity = angularVelocity;
            Owner = owner;
            Colour = colour;
            IsActive = true;
            HarmfulToPlayer = harmfulToPlayer;
            HarmfulToEnemy = harmfulToEnemy;
            Damage = damage;

            if (shader != null)
                Shader = new Shader(shader, Colour);
            else Shader = null;

            RemoveOnHit = false;

            InitialWidth = Width;
            ThinOut = false;

            Participating = true;
            Friendly = !harmfulToPlayer;
            FadeOutTime = 0;

            return this;
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

        public virtual void SetDieAfterDuration(bool die)
        {
            DieAfterDuration = die;
        }
        public virtual void AddDeathrayToActiveProjectiles()
        {
            if (!IsSpawned)
            {
                IsSpawned = true;

                EntityManager.projectilesToAdd.Add(this);
            }
        }
        public override void AI()
        {
            Length = InitialLength;

            if (ExtraAI is not null)
            {
                ExtraAI();
            }

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

            if (ThinOut && AITimer > Duration - FadeOutTime)
            {
                Width = MathHelper.Lerp(0, Width, MathHelper.Clamp((float)(Duration - AITimer) / FadeOutTime, 0f, 1f));
            }

            if (DieAfterDuration && AITimer == Duration)
            {
                Die();
            }
        }

        public void DrawWithShader(SpriteBatch spriteBatch)
        {

        }
        public override void UpdateHitbox()
        {
            Vector2 centre = Position - Utilities.RotateVectorClockwise(new Vector2(0f, Length / 2f), Rotation);

            // to do: make work with circles

            if (Width >= 2f)
            {
                Hitbox = Utilities.FillRectWithCircles(centre, (int)Width, (int)Length, Rotation);
            }
            else
            {
                Hitbox.Clear();
            }
        }

        public override void CheckForHits()
        {
            if (IsActive) //only bother to check for collision if active
            {
                base.CheckForHits();
            }          
        }

        public virtual void SetLength(float length)
        {
            if (length >= 0)
            {
                Length = length;
            }
        }

        public virtual void OnHitObstacle(Vector2 obstaclePosition)
        {
            SetLength(Utilities.DistanceBetweenVectors(obstaclePosition, Position));

            if (OnEdgeTouch is not null)
            {
                OnEdgeTouch();
            }
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
        public virtual void SetThinOut(bool thinOut, int fadeOutTime = 20)
        {
            ThinOut = thinOut;
            FadeOutTime = fadeOutTime;
        }

        public List<Vector2> GenerateVertices(int points = 30)
        {
            List<Vector2> vertices = new List<Vector2>();

            Vector2 rotationVec = Rotation.ToVector();

            for (int i = 0; i < points + 1; i++) // + 1 so we include end point, this might be a little odd though and cause issues so be cautious
            {
                Vector2 centrePoints = Position + ((float)i / points) * Length * rotationVec;
                float widthHere = Width; // change to use width function of y and t
                vertices.Add(centrePoints + widthHere / 2 * rotationVec.Rotate(PI / 2));
                vertices.Add(centrePoints + widthHere / 2 * rotationVec.Rotate(-PI / 2));
            }

            return vertices;
        }
        public override void Draw(SpriteBatch spritebatch)
        {
            if (IsActive)
            {               
                //ApplyShaderParameters();

                //Shader.Apply();

                //Vector2 size = new Vector2(Width / Texture.Width, Length / Texture.Height); // Scale the beam up to the required width and length.

                //Vector2 originOffset = new Vector2(Texture.Width / 2, 0f);

                //spritebatch.Draw(Texture, Position, null, Colour, PI + Rotation, originOffset, size, SpriteEffects.None, 0);

                foreach (Vector2 point in GenerateVertices())
                {
                    Drawing.DrawBox(point, Colour, 1f);
                }
            }
        }
    }
}