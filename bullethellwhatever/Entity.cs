using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bullethellwhatever
{
    public abstract class Entity
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Texture2D Texture;
        public bool isBoss;
        public bool isPlayer;
        public float AITimer;
        public float Damage;
        public Rectangle Hitbox; //this hitbox system works only with squares, if you want to expand make Size a Vector2
        public float Size; //relative to player being 1
        public float Health;
        public float MaxHP;
        public bool DeleteNextFrame;

        public static bool touchingBottom(Entity entity, int screenHeight) //hieght is height of texture
        {
            if (entity.Position.Y + entity.Hitbox.Height / 2 >= screenHeight)
                return true;
            else return false;
            //if at the bottom
        }

        public static bool touchingTop(Entity entity)
        {
            if (entity.Position.Y  - entity.Hitbox.Height / 2 <= 0)
                return true;
            else return false;
        }

        public static bool touchingRight(Entity entity,  int screenWidth)
        {
            if (entity.Position.X + entity.Hitbox.Width / 2 >= screenWidth)
                return true;
            else return false;
        }

        public static bool touchingLeft(Entity entity)
        {

            if (entity.Position.X - entity.Hitbox.Width / 2 <= 0)
                return true;
            else return false;
        }

        public static bool touchingAnEdge(Entity entity, int screenwidth, int screenHeight)
        {
            if (touchingTop(entity) || touchingBottom(entity, screenHeight) ||
                    touchingLeft(entity) || touchingRight(entity, screenwidth))
            {
                return true;
            }

            return false;
        }

        public bool isCollidingWithPlayer() //you need to refactor this whole thing if you wanna use rectangles
        {
            float totalwidth = Hitbox.Width + Main.player.Hitbox.Width;

            if (Math.Abs(Position.X - Main.player.Position.X) <= totalwidth && Math.Abs(Position.Y - Main.player.Position.Y) <= totalwidth)
                return true;

            return false;
        }
        public virtual bool ShouldRemoveOnEdgeTouch() => false;

        public virtual bool IsHarmful() => false; //is harmful to player

        public virtual bool hasDesperation() => false; 
        public virtual Color Colour() => Color.White;
        public abstract void HandleMovement();
        public abstract void AI();
    }
}
