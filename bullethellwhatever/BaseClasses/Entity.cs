﻿global using static bullethellwhatever.MainFiles.Main;

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

namespace bullethellwhatever.BaseClasses
{
    public abstract class Entity
    {
        public Vector2 Position;
        public Vector2 Velocity;

        public Texture2D Texture;
        public NoiseMap Map;
        public Action ExtraDraw;

        public Action ApplyExtraShaderParameters;

        public float Depth;

        public bool DealDamage;
        public bool IsHarmful;
        public int AITimer;
        public float Damage;
        public float Opacity;
        public float InitialOpacity;

        public RotatedRectangle Hitbox;

        public Vector2 Size;

        public float RotationalVelocity;

        public bool Participating; // whether to do anything with collision
        public float Health;
        public float MaxHP;
        public bool DeleteNextFrame;
        public float Rotation;

        public bool ShouldRemoveOnEdgeTouch;
        public Color Colour;
        public List<TelegraphLine> activeTelegraphs = new List<TelegraphLine>();
        public Effect? Shader;
        public int Updates;

        public Action OnDeath;
        public Action ExtraAI;

        public float[] ExtraData
        {
            get;
            set;
        } // small array of floats each entity can use

        public List<Component> AdditionalComponents = new List<Component>();
        public abstract void AI();

        public virtual void Update()
        {
            foreach (Component component in AdditionalComponents)
            {
                component.Update();
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
            Size = size;
        }
        public virtual void SetDepth(float depth)
        {
            Depth = MathHelper.Clamp(depth, 0f, 1f);
        }
        public virtual void SetNoiseMap(string fileName, float scrollSpeed)
        {
            Map = new NoiseMap(AssetRegistry.GetTexture2D(fileName), scrollSpeed);
        }
        public void SetExtraData(int index, float value)
        {
            ExtraData[index] = value;
        }
        public virtual float DepthFactor()
        {
            return MathHelper.Lerp(1, 0.1f, Depth);
        }
        public virtual Vector2 GetSize() // get a size that corresponds to the current depth
        {
            return Vector2.LerpPrecise(Size, Size / 10f, Depth);
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
        public virtual void DealDamageTo(NPC npc, Collision collision)
        {
            npc.IFrames = npc.MaxIFrames;

            npc.Health = npc.Health - Damage;
        }

        public virtual void DamagePlayer()
        {
            player.IFrames = 20f;

            player.Health = player.Health - Damage;

            Drawing.ScreenShake(3, 10);
        }
        public bool TouchingBottom()
        {
            if (Position.Y + Texture.Height * GetSize().Y / 2 >= GameHeight)
                return true;
            else return false;
            //if at the bottom
        }

        public bool TouchingTop()
        {
            if (Position.Y - Texture.Height * GetSize().Y / 2 <= 0)
                return true;
            else return false;
        }

        public bool TouchingRight()
        {
            if (Position.X + Texture.Width * GetSize().X / 2 >= GameWidth)
                return true;
            else return false;
        }

        public bool TouchingLeft()
        {
            if (Position.X - Texture.Width * GetSize().X / 2 <= 0)
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
        public virtual void Heal(float amount)
        {
            Health = Health + amount;

            if (Health > MaxHP)
            {
                Health = MaxHP;
            }
        }
        public virtual void ApplyShaderParameters()
        {
            Shader.Parameters["uTime"]?.SetValue(AITimer);

            if (Map is not null)
            {
                Shader.Parameters["noiseMap"]?.SetValue(Map.Texture);
                Shader.Parameters["scrollSpeed"]?.SetValue(Map.ScrollSpeed);
            }

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
            Shader.Parameters["randomNoiseMap"]?.SetValue(AssetRegistry.GetTexture2D("RandomNoise"));
        }

        public virtual void ApplyRandomNoise(Effect shader)
        {
            shader.Parameters["randomNoiseMap"]?.SetValue(AssetRegistry.GetTexture2D("RandomNoise"));
        }

        public virtual void SetShader(string filename)
        {
            Shader = AssetRegistry.GetShader(filename);
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

        public virtual void AddTrail(int length, string shader = null)
        {
            PrimitiveTrail trail = new PrimitiveTrail(this, length, shader);

            AdditionalComponents.Add(trail);
        }

        public virtual bool IsCollidingWith(Entity other)
        {
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

                Shader.CurrentTechnique.Passes[0].Apply();
            }

            Drawing.BetterDraw(Texture, Position, null, Colour * Opacity, Rotation, GetSize(), SpriteEffects.None, 0f);

            foreach (Component component in AdditionalComponents)
            {
                component.Draw(spriteBatch);
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
