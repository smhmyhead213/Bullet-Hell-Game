﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever;

public class NPC : Entity
{
    public bool ContactDamage;

    public float IFrames;

    public virtual void Spawn(Vector2 position, Vector2 velocity, float damage, Texture2D texture, Vector2 size,
        float MaxHealth)
    {
        Position = position;
        Velocity = velocity;
        Damage = damage;
        Texture = texture;
        Main.NPCsToAddNextFrame.Add(this);
        Size = size;
        Hitbox = new Rectangle((int)position.X - texture.Width / 2, (int)position.Y - texture.Height / 2, texture.Width,
            texture.Height);
        Hitbox.Width = Hitbox.Width * (int)size.X;
        Hitbox.Height = Hitbox.Height * (int)size.Y;
        Health = MaxHealth;
        MaxHP = MaxHealth;
        ContactDamage = false;
    }

    public override void AI()
    {
    }

    public bool isCollidingWithPlayerProjectile(FriendlyProjectile projectile)
    {
        float totalwidth = Hitbox.Width + projectile.Hitbox.Width;

        if (Math.Abs(Position.X - projectile.Position.X) <= totalwidth &&
            Math.Abs(Position.Y - projectile.Position.Y) <= totalwidth)
            return true;

        return false;
    }
}