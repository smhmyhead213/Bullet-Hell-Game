﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever
{
    public class FriendlyProjectile : Projectile
    {
        public int Pierce;
        public override bool IsHarmful() => false;

        public override void Spawn(Vector2 position, Vector2 velocity, float damage, Texture2D texture, float acceleration, Vector2 size) //this is overriden as friendly projectiles add themselves to a different list
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Main.friendlyProjectilesToAddNextFrame.Add(this);
            Size = size;
        }

        
    }
}