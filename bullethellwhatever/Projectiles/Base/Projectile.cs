using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace bullethellwhatever
{
    public class Projectile : Entity
    {
        public float Acceleration;
        public float TimeAlive;
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, Texture2D texture, float acceleration, Vector2 size)
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Acceleration = acceleration;
            Main.enemyProjectilesToAddNextFrame.Add(this);
            DeleteNextFrame = false;
            Size = size;
        }

         //and drawing
        
        public override bool ShouldRemoveOnEdgeTouch() => true;
        public override void AI()
        {
            TimeAlive++;
            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            Position = Position + Velocity;
        }


    }
}
