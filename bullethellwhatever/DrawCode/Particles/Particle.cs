using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.MainFiles;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses.Entities;

namespace bullethellwhatever.DrawCode.Particles
{
    public class Particle : Entity
    {
        public int Lifetime;
        public Vector2 Acceleration;
        public Vector2 InitialSize;
        public bool Shrink;
        public bool FadeOut;
        public void Spawn(string texture, Vector2 position, Vector2 velocity, Vector2 acceleration, Vector2 size, float rotation, Color colour, float opacity, int lifetime)
        {
            Texture = AssetRegistry.GetTexture2D(texture);

            Lifetime = lifetime;

            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;

            Size = size;

            InitialSize = Size;

            Rotation = rotation;
            Colour = colour;
            Opacity = MathHelper.Clamp(opacity, 0f, 1f);
            InitialOpacity = Opacity;

            Shrink = false;
            FadeOut = true;

            EntityManager.ParticlesToAdd.Add(this);
        }

        public override void PostUpdate()
        {
            base.PostUpdate();

            AITimer++;
        }

        public void SetShrink(bool shrink)
        {
            Shrink = shrink;
        }
        public override void AI()
        {
            Velocity = Velocity + Acceleration;
            Position = Position + Velocity;

            if (FadeOut)
            {
                int fadeOutTime = 15;

                if (AITimer > Lifetime - fadeOutTime)
                {
                    Opacity = MathHelper.Lerp(0, InitialOpacity, (float)(Lifetime - AITimer) / fadeOutTime);
                }

                if (Shrink)
                {
                    Size = Vector2.Lerp(InitialSize, Vector2.Zero, (float)AITimer / Lifetime);
                }
            }

            if (AITimer == Lifetime)
            {
                Die();
            }

            if (ExtraAI is not null)
            {
                ExtraAI();
            }
        }

        public override void Die()
        {
            EntityManager.ParticlesToRemove.Add(this);
        }

        public override void Delete()
        {
            Die();
        }
    }
}
