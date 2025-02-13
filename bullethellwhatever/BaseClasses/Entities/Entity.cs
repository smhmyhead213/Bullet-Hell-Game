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
using SharpDX.Direct3D9;

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

        public bool HarmfulToPlayer;
        public bool HarmfulToEnemy;

        public int AITimer;
        public float Damage;
        public float Opacity
        {
            get;
            set;
        }
        public float InitialOpacity;

        public List<Circle> Hitbox = new List<Circle>();

        public Vector2 Scale;

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
                component.PreUpdate();
            }
        }

        /// <summary>
        /// An update that occurs AfTER the AI and collision run.
        /// </summary>
        public virtual void PostUpdate()
        {
            foreach (Component component in AdditionalComponents)
            {
                component.PostUpdate();
            }
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
            Scale = size;
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
            return Scale;
        }

        public float Width()
        {
            return Scale.X * Texture.Width;
        }

        public float Height()
        {
            return Scale.Y * Texture.Height;
        }

        public Vector2 Dimensions()
        {
            return new Vector2(Width(), Height());
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

        public virtual bool IsCollidingWith(Entity other, int raycastDirection = 0)
        {
            // read it, oh O(n^2) haters, and weep.
            foreach (Circle mine in Hitbox)
            {
                foreach (Circle notmine in other.Hitbox)
                {
                    if (mine.Intersects(notmine))
                    {
                        return true;
                    }
                }
            }

            if (raycastDirection == 1 || raycastDirection == -1)
            {
                List<Circle> raycast = Utilities.FillRectWithCircles(Position + Velocity / 2f, (int)Width(), (int)Velocity.Length(), Utilities.VectorToAngle(Velocity));

                foreach (Circle circle in raycast)
                {
                    foreach (Circle notmine in other.Hitbox)
                    {
                        if (circle.Intersects(notmine))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
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

        public void DrawHitbox()
        { 
            Texture2D circle = AssetRegistry.GetTexture2D("Circle");

            for (int i = 0; i < Hitbox.Count; i++)
            {
                Color colour = Color.Red * 0.5f;
                Vector2 size = Hitbox[i].Radius / circle.Width * 2f * Vector2.One;
                //Drawing.BetterDraw("box", Hitbox[i].Centre, null, colour, 0f, size * 1.4f, SpriteEffects.None, 0f);
                Drawing.BetterDraw(circle, Hitbox[i].Centre, null, colour, 0f, size, SpriteEffects.None, 0f);
                
            }

            //if (Raycast is not null)
            //{
            //    List<Circle> raycast = Utilities.FillRectWithCircles(Position + Velocity / 2f, (int)Width(), (int)Velocity.Length(), Utilities.VectorToAngle(Velocity));

            //    for (int i = 0; i < raycast.Count; i++)
            //    {
            //        Color colour = i == 0 ? Color.Pink : Color.Red;

            //        Drawing.BetterDraw("box", raycast[i].Centre, null, colour, 0f, size, SpriteEffects.None, 0f);

            //        for (int j = 0; j < 4; j++)
            //        {
            //            Drawing.BetterDraw("box", raycast[i].Centre + Utilities.RotateVectorClockwise(new Vector2(raycast[i].Radius, 0), PI / 2 * j), null, colour, 0f, size, SpriteEffects.None, 0f);
            //        }
            //    }
            //}
        }
        public void SetHitbox()
        {
            UpdateHitbox();
        }

        public virtual void UpdateHitbox() //call this before everything else so after AIs proper hitboxes get sent to EntityManager
        {
            Hitbox = Utilities.FillRectWithCircles(Position, (int)Width(), (int)Height(), Rotation);
        }
    }
}
