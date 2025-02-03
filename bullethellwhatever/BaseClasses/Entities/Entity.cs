global using static bullethellwhatever.MainFiles.Main;

using Microsoft.Xna.Framework;
using bullethellwhatever.DrawCode;
using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using bullethellwhatever.Projectiles.TelegraphLines;
using System.Collections.Generic;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.NPCs;
using bullethellwhatever.AssetManagement;
using System.Linq;
using SharpDX.MediaFoundation;

namespace bullethellwhatever.BaseClasses.Entities
{
    public abstract class Entity
    {
        public Vector2 Position;
        public Vector2 Velocity;

        public Texture2D Texture;
        public Action ExtraDraw;

        public EntityLabels Label;

        public Action ApplyExtraShaderParameters;

        public bool ContactDamage;
        public bool IsHarmful;
        public int AITimer;
        public float Damage;
        public float Opacity
        {
            get;
            set;
        }
        public float InitialOpacity;

        public RotatedRectangle Hitbox;

        public Vector2 Size;

        public float RotationalVelocity;

        public bool Participating; // whether to do anything with collision
        
        public float MaxHP;
        public bool DeleteNextFrame;
        public float Rotation
        {
            get;
            set;
        }

        public bool ShouldRemoveOnEdgeTouch;
        public Color Colour;
        public List<TelegraphLine> activeTelegraphs = new List<TelegraphLine>();
        public Shader Shader;
        public int Updates;

        public Action OnDeath;
        public Action ExtraAI;

        public float[] ExtraData
        {
            get;
            set;
        } // small array of floats each entity can use

        public RaycastData Raycast;

        public List<Component> AdditionalComponents = new List<Component>();
        public abstract void AI();

        /// <summary>
        /// An update that occurs BEFORE the AI runs.
        /// </summary>
        public virtual void PreUpdate()
        {
            foreach (Component component in AdditionalComponents)
            {
                component.Update();
            }
        }

        /// <summary>
        /// An update that occurs AfTER the AI and collision run.
        /// </summary>
        public virtual void PostUpdate()
        {

        }

        public void ClearExtraData()
        {
            ExtraData = new float[ExtraData.Length];
        }
        public virtual void SetUpdates(int updates)
        {
            Updates = updates;
        }

        public virtual void SetSizeNoScaling(Vector2 size)
        {
            Size = size;
        }

        public virtual void SetNoiseMap(string fileName, float scrollSpeed)
        {
            Shader.SetNoiseMap(fileName, scrollSpeed);
        }
        public void SetExtraData(int index, float value)
        {
            ExtraData[index] = value;
        }

        public virtual Vector2 GetSize() // get a size that corresponds to the current depth
        {
            return Size;
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
        public virtual void SetParticipating(bool participating)
        {
            Participating = participating;
        }

        public bool TouchingBottom()
        {
            if (Position.Y + Texture.Height * GetSize().Y / 2 >= MainCamera.VisibleArea.Bottom)
                return true;
            else return false;
            //if at the bottom
        }

        public bool TouchingTop()
        {
            if (Position.Y - Texture.Height * GetSize().Y / 2 <= MainCamera.VisibleArea.Top)
                return true;
            else return false;
        }

        public bool TouchingRight()
        {
            if (Position.X + Texture.Width * GetSize().X / 2 >= MainCamera.VisibleArea.Right)
                return true;
            else return false;
        }

        public bool TouchingLeft()
        {
            if (Position.X - Texture.Width * GetSize().X / 2 <= MainCamera.VisibleArea.Left)
                return true;
            else return false;
        }

        public bool TouchingAnEdge()
        {
            if (TouchingTop() || TouchingBottom() ||
                    TouchingLeft() || TouchingRight())
            {
                return true;
            }

            return false;
        }

        ///<summary>
        /// Sets the entity's opacity AND initial opacity. To avoid the latter, directly set Opacity.
        ///</summary>
        public virtual void SetOpacity(float opacity)
        {
            Opacity = opacity;
            Opacity = InitialOpacity;
        }

        public virtual void SetRotation(float rotation)
        {
            Rotation = rotation;
            UpdateHitbox();
        }
        public virtual void SetTexture(string texture)
        {
            Texture = AssetRegistry.GetTexture2D(texture);
        }
        public virtual void SetTexture(Texture2D texture)
        {
            Texture = texture;
        }
        public virtual void SetOnDeath(Action action)
        {
            OnDeath = action;
        }
        public virtual void SetExtraAI(Action action)
        {
            ExtraAI = action;
        }

        public virtual void SetExtraDraw(Action action)
        {
            ExtraDraw = action;
        }

        public virtual void ApplyShaderParameters()
        {
            // assumption made that we want to pass into the shader the colour of the entity
            Shader.UpdateShaderParameters(AITimer);

            if (ApplyExtraShaderParameters is not null)
            {
                ApplyExtraShaderParameters();
            }
        }
        public virtual void SetApplyShaderParameters(Action action)
        {
            ApplyExtraShaderParameters = action;
        }

        public virtual void ApplyRandomNoise()
        {
            Shader.SetParameter("randomNoiseMap", AssetRegistry.GetTexture2D("RandomNoise"));
        }


        public virtual void SetShader(string filename)
        {
            Shader = new Shader(filename, Color.White);
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

        /// <summary>
        /// Completely removes an entity from the game, without triggering any on death behaviour.
        /// </summary>
        public abstract void Delete();

        public virtual void DealDamage(NPC target)
        {
            target.TakeDamage(Damage);
        }

        public virtual void AddTrail(int length, string shader = null)
        {
            PrimitiveTrail trail = new PrimitiveTrail(this, length, shader);

            AdditionalComponents.Add(trail);
        }

        public virtual bool IsCollidingWith(Entity other, bool backwardsRaycast = true)
        {
            if (Raycast is not null)
            {
                return Hitbox.IntersectsWithRaycast(other.Hitbox, Velocity, Raycast.Direction).Collided;
            }

            return Hitbox.Intersects(other.Hitbox).Collided;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (ExtraDraw is not null)
            {
                ExtraDraw();
            }

            if (Shader is not null)
            {
                ApplyShaderParameters();

                Shader.Apply();
            }

            Drawing.BetterDraw(Texture, Position, null, Colour * Opacity, Rotation, GetSize(), SpriteEffects.None, 0f);

            foreach (Component component in AdditionalComponents)
            {
                component.Draw(spriteBatch);
            }
        }

        public void DrawHitbox(SpriteBatch s, float width)
        {
            Hitbox.Draw(width);
            
            if (Raycast is not null)
            {
                Hitbox.GenerateRaycast(Raycast).Draw(width);
            }
        }
        public void SetHitbox()
        {
            UpdateHitbox();
        }
        public virtual void UpdateHitbox() //call this before everything else so after AIs proper hitboxes get sent to EntityManager
        {
            Hitbox.UpdateRectangle(Rotation, Texture.Width * GetSize().X, Texture.Height * GetSize().Y, Position);

            if (Raycast is not null)
            {
                Raycast.DescribingVector = Velocity;
            }

            Hitbox.UpdateVertices();
        }
    }
}
