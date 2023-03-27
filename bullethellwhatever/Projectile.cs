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
        public bool DeleteNextFrame;
        public float TimeAlive;
        public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, Texture2D texture, int updates)
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Updates = updates;
            Main.activeProjectiles.Add(this);
            HandleMovement();
            DeleteNextFrame = false;

            
        }

        public override void HandleMovement() //and drawing
        {
            Position = Position + Velocity;

            if (Updates > 1)
            {
                Main._spriteBatch.Begin();
                Main._spriteBatch.Draw(Texture, Position, null, Colour(), 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0f);
                Main._spriteBatch.End();
            }

            
        }
        public override bool ShouldRemoveOnEdgeTouch() => true;
        public override void AI()
        {
            TimeAlive++;
            HandleMovement();
        }

        
    }
}
