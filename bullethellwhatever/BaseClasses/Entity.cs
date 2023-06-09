﻿global using static bullethellwhatever.MainFiles.Main;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using bullethellwhatever.Projectiles.TelegraphLines;
using System.Collections.Generic;
using System.Security.Policy;

namespace bullethellwhatever.BaseClasses
{
    public abstract class Entity //wait til he finds out he has to rework the hitbox system to work with different square sizes! get a load of this guy!
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Texture2D Texture;
        public bool isBoss;
        public bool isPlayer;
        public bool IsHarmful;
        public int AITimer;
        public float Damage;
        public float Opacity;
        public Rectangle Hitbox; //this hitbox system works only with squares, if you want to expand make Size a Vector2
        public Vector2 Size; //relative to player being 1
        public float Health;
        public float MaxHP;
        public bool DeleteNextFrame;
        public float Rotation;
        public bool IsDesperationOver;
        public bool ShouldRemoveOnEdgeTouch;
        public Color Colour;
        public List<TelegraphLine> activeTelegraphs = new List<TelegraphLine>();
        public Effect? Shader;


        public Vector2[] afterimagesPositions; //when using afterimages, this needs to be initialised in the Spawn method of whatever has them.

        public static void SetHitbox(Entity entity)
        {
            entity.Hitbox = new((int)entity.Position.X - entity.Texture.Width / 2 * (int)entity.Size.X, (int)entity.Position.Y - entity.Texture.Height / 2 * (int)entity.Size.Y, entity.Texture.Width * (int)entity.Size.X,
                entity.Texture.Height * (int)entity.Size.Y);
        }
        public static bool touchingBottom(Entity entity) //hieght is height of texture
        {
            if (entity.Position.Y + entity.Hitbox.Height / 2 >= _graphics.PreferredBackBufferHeight)
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

        public static bool touchingRight(Entity entity)
        {
            if (entity.Position.X + entity.Hitbox.Width / 2 >= _graphics.PreferredBackBufferWidth)
                return true;
            else return false;
        }

        public static bool touchingLeft(Entity entity)
        {

            if (entity.Position.X - entity.Hitbox.Width / 2 <= 0)
                return true;
            else return false;
        }

        public static bool touchingAnEdge(Entity entity)
        {
            if (touchingTop(entity) || touchingBottom(entity) ||
                    touchingLeft(entity) || touchingRight(entity))
            {
                return true;
            }

            return false;
        }

        public bool isCollidingWithPlayer() //you need to refactor this whole thing if you wanna use rectangles
        {
            float totalwidth = Hitbox.Width + player.Hitbox.Width;

            if (Math.Abs(Position.X - player.Position.X) <= totalwidth && Math.Abs(Position.Y - player.Position.Y) <= totalwidth)
                return true;

            return false;
        }
        public abstract void AI();
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
