﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bullethellwhatever;

public class Player : Entity
{
    #region Spawning

    public void Spawn(Vector2 position, Vector2 initialVelocity, float damage,
        Texture2D texture) //initialise player data
    {
        Position = position;
        Velocity = initialVelocity;
        Texture = texture;
        isPlayer = true;
        isBoss = false;
        IFrames = 0;
        Health = 15; //make all these values changeable
        MaxHP = Health;
        Size = Vector2.One;
        ShotCooldown = 20f;
        ShotCooldownRemaining = 0f;
        ActiveWeapon = Weapons.Sharpshooter;
        MoveSpeed = 5.5f;
        ScrollCooldown = 0f;
    }

    #endregion

    public override bool ShouldRemoveOnEdgeTouch()
    {
        return false;
    }

    #region AI

    public override void AI() //cooldowns and iframes and stuff are handled here
    {
        var kstate = Keyboard.GetState();
        var mouseState = Mouse.GetState();

        if (ScrollCooldown > 0) ScrollCooldown--;

        Velocity = Vector2.Zero;

        if (kstate.IsKeyDown(Keys.W) &&
            !touchingTop(this)) //fix the movement so you dont move faster diagonally, future you's problem
            Velocity.Y = Velocity.Y - 1f;

        if (kstate.IsKeyDown(Keys.S) && !touchingBottom(this, Main._graphics.PreferredBackBufferHeight))
            Velocity.Y = Velocity.Y + 1f;

        if (kstate.IsKeyDown(Keys.A) && !touchingLeft(this)) Velocity.X = Velocity.X - 1f;

        if (kstate.IsKeyDown(Keys.D) && !touchingRight(this, Main._graphics.PreferredBackBufferWidth))
            Velocity.X = Velocity.X + 1f;

        Position = Position + MoveSpeed * Utilities.SafeNormalise(Velocity, Vector2.Zero);
        if (GameState.WeaponSwitchControl) //if scroll wheel controls
        {
            if (mouseState.ScrollWheelValue / 120 % 3 == 0 &&
                ScrollCooldown == 0) //are you happy now gemma??????????????????????
            {
                ActiveWeapon = Weapons.Sharpshooter;
                ScrollCooldown = 3f;
            }

            if (mouseState.ScrollWheelValue / 120 % 3 == 1 && ScrollCooldown == 0)
            {
                ActiveWeapon = Weapons.MachineGun;
                ScrollCooldown = 3f;
            }

            if (mouseState.ScrollWheelValue / 120 % 3 == 2 && ScrollCooldown == 0)
            {
                ActiveWeapon = Weapons.Homing;
                ScrollCooldown = 3f;
            }

            if (kstate.IsKeyDown(Keys.Q) && Main.activeNPCs.Count == 0)
            {
                Health = MaxHP;
                EntityManager.SpawnBoss();
            }
        }

        else
        {
            if (kstate.IsKeyDown(Keys.D1)) //are you happy now gemma??????????????????????
            {
                ActiveWeapon = Weapons.Sharpshooter;
                ScrollCooldown = 3f;
            }

            if (kstate.IsKeyDown(Keys.D2))
            {
                ActiveWeapon = Weapons.MachineGun;
                ScrollCooldown = 3f;
            }

            if (kstate.IsKeyDown(Keys.D3))
            {
                ActiveWeapon = Weapons.Homing;
                ScrollCooldown = 3f;
            }
        }


        if (kstate.IsKeyDown(Keys.Q) && Main.activeNPCs.Count == 0)
        {
            Health = MaxHP;
            EntityManager.SpawnBoss();
        }


        //I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU
        Hitbox = new Rectangle((int)Position.X - Texture.Width / 2, (int)Position.Y - Texture.Height / 2, Texture.Width,
            Texture.Height);

        if (Health > 0)
        {
            if (IFrames > 0) IFrames--;

            if (ShotCooldownRemaining > 0) ShotCooldownRemaining--;

            foreach (var projectile in Main.activeProjectiles)
                if (projectile.isCollidingWithPlayer() && IFrames == 0f)
                    TakeDamage(projectile);

            foreach (var npc in Main.activeNPCs)
                if (npc.isCollidingWithPlayer() && IFrames == 0f && npc.ContactDamage)
                    TakeDamage(npc);

            if (mouseState.LeftButton == ButtonState.Pressed && ShotCooldownRemaining == 0)
            {
                ShotCooldownRemaining = ShotCooldown;
                Shoot();
            }
        }
        else
        {
            Health = MaxHP;
            Position = new Vector2(Main._graphics.PreferredBackBufferWidth / 2,
                Main._graphics.PreferredBackBufferHeight / 2);
            Main.activeNPCs.Clear();
            Main.activeProjectiles.Clear();
            Main.activeFriendlyProjectiles.Clear();
        }
    }

    #endregion

    #region Shooting

    public void Shoot()
    {
        var mouseState = Mouse.GetState();

        var mousePosition = new Vector2(mouseState.X, mouseState.Y);

        if (ActiveWeapon == Weapons.Sharpshooter)
        {
            ShotCooldown = 20f;

            var playerProjectile = new PlayerSharpShooterProjectile();


            playerProjectile.Spawn(Position, 30f * Utilities.Normalise(mousePosition - Position), 2f,
                Main.player.Texture, 0, Vector2.One);
        }

        else if (ActiveWeapon == Weapons.MachineGun)
        {
            ShotCooldown = 3f;

            var playerProjectile = new PlayerProjectile();

            var rnd = new Random();

            playerProjectile.Spawn(Position,
                20f * Utilities.RotateVectorClockwise(Utilities.Normalise(mousePosition - Position),
                    Utilities.ToRadians(rnd.Next(-10, 10))), 0.25f, Main.player.Texture, 0, Vector2.One);
        }

        else if (ActiveWeapon == Weapons.Homing)
        {
            ShotCooldown = 10f;
            var initialVelocity = 7f;
            var projectile = new PlayerHomingProjectile();


            projectile.Spawn(Position, initialVelocity * Utilities.Normalise(mousePosition - Position), 0.4f,
                Main.player.Texture, 0, Vector2.One);
        }
    }

    #endregion

    public void TakeDamage(Entity entity) //take damage from an entity
    {
        Health = Health - entity.Damage;
        IFrames = 20f;
    }

    #region Fields

    public float IFrames;
    public float ShotCooldown;
    public float ShotCooldownRemaining;
    public float ScrollCooldown;
    public float MoveSpeed;

    public enum Weapons
    {
        Sharpshooter,
        MachineGun,
        Homing
    }

    public Weapons ActiveWeapon;

    #endregion

    #region Input Handling

    #endregion
}