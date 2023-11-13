using bullethellwhatever.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.MainFiles;

namespace bullethellwhatever.DrawCode
{
    public class Particle : Entity
    {
        public int Lifetime;
        public Vector2 Acceleration;
        public float InitialOpacity;
        public void Spawn(string texture, Vector2 position, Vector2 velocity, Vector2 acceleration, Vector2 size, float rotation, Color colour, float opacity, int lifetime)
        {
            Texture = Assets[texture];

            Lifetime = lifetime;

            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;

            Size = size;

            Rotation = rotation;
            Colour = colour;
            Opacity = MathHelper.Clamp(opacity, 0f, 1f);
            InitialOpacity = Opacity;

            EntityManager.ParticlesToAdd.Add(this);
        }

        public override void Update()
        {
            base.Update();

            AITimer++;
        }
        public override void AI()
        {
            Velocity = Velocity + Acceleration;
            Position = Position + Velocity;

            int fadeOutTime = 15;

            if (AITimer > Lifetime - fadeOutTime)
            {
                Opacity = MathHelper.Lerp(InitialOpacity, 0, (float)(Lifetime - AITimer) / fadeOutTime);
            }

            if (AITimer == Lifetime)
            {
                Die();
            }
        }

        public override void Die()
        {
            EntityManager.ParticlesToRemove.Add(this);
        }
    }
}
