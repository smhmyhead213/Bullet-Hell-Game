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
        public int Updates; //typically 1, use a higher number for accurate homing
        
        public float Acceleration;
        public float TimeAlive;
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, Texture2D texture, float acceleration)
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Acceleration = acceleration;
            Main.activeProjectiles.Add(this);
            DeleteNextFrame = false;
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
