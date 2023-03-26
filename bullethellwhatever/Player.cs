using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever
{
    public class Player : Entity
    {
        
        public float IFrames;
        public float ShotCooldown;
        public float ShotCooldownRemaining;
        
        public enum Weapons
        {
            Sharpshooter,
            MachineGun,
            Homing,

        }

        public Weapons ActiveWeapon;
        public void Spawn(Vector2 position, Vector2 initialVelocity, float damage, Texture2D texture) //initialise player data
        {
            Position = position;
            Velocity = initialVelocity;
            Texture = texture;
            isPlayer = true;
            isBoss = false;
            IFrames = 0;
            Health = 15; //make all these values changeable
            Size = 1f;
            ShotCooldown = 20f;
            ShotCooldownRemaining = 0f;
            Hitbox = new((int)position.X - (texture.Width / 2), (int)position.Y - (texture.Height / 2), texture.Width, texture.Height);
        }
        public override void HandleMovement() //and input
        {
            var kstate = Keyboard.GetState();


            if (kstate.IsKeyDown(Keys.W) && !touchingTop(this))
            {
                Position.Y -= Velocity.Y;
            }

            if (kstate.IsKeyDown(Keys.S) && !touchingBottom(this, Main._graphics.PreferredBackBufferHeight))
            {
                Position.Y += Velocity.Y;
            }

            if (kstate.IsKeyDown(Keys.A) && !touchingLeft(this))
            {
                Position.X -= Velocity.X;
            }

            if (kstate.IsKeyDown(Keys.D) && !touchingRight(this, Main._graphics.PreferredBackBufferWidth))
            {
                Position.X += Velocity.X;
            }

            if (kstate.IsKeyDown(Keys.D1))
            {
                ActiveWeapon = Weapons.Sharpshooter;
            }

            if (kstate.IsKeyDown(Keys.D2))
            {
                ActiveWeapon = Weapons.MachineGun;
            }

            if (kstate.IsKeyDown(Keys.D3))
            {
                ActiveWeapon = Weapons.Homing;
            }

            if (kstate.IsKeyDown(Keys.Q) && Main.activeNPCs.Count == 0)
            {
                Health = 15f;
                EntityManager.SpawnBoss();
            }

        }

        public override bool ShouldRemoveOnEdgeTouch() => false;

        public override void AI() //cooldowns and iframes and stuff are handled here
        {
            var mouseState = Mouse.GetState();

            HandleMovement();

            if (Health > 0)
            {
                if (IFrames > 0)
                {
                    IFrames--;
                }

                if (ShotCooldownRemaining > 0)
                {
                    ShotCooldownRemaining--;

                }

                foreach (Projectile projectile in Main.activeProjectiles)
                {
                    if (projectile.isCollidingWithPlayer() && IFrames == 0f)
                    {
                        Health = Health - projectile.Damage;
                        IFrames = 20f;
                    }

                }

                if (mouseState.LeftButton == ButtonState.Pressed && ShotCooldownRemaining == 0)
                {
                    ShotCooldownRemaining = ShotCooldown;
                    Shoot();
                }
            }
            else
            {
                Health = 15;
                Position = new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2);
                Main.activeNPCs.Clear();
                Main.activeProjectiles.Clear();
                Main.activeFriendlyProjectiles.Clear();
            }
        }

        public void Shoot()
        {
            var mouseState = Mouse.GetState();

            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            if (ActiveWeapon == Weapons.Sharpshooter)
            {
                ShotCooldown = 20f;

                PlayerSharpShooterProjectile playerProjectile = new PlayerSharpShooterProjectile();

                

                playerProjectile.Spawn(Position, 20f * Utilities.Normalise(mousePosition - Position), 3f, Main.player.Texture, 1);
            }

            else if (ActiveWeapon == Weapons.MachineGun)
            {
                ShotCooldown = 3f;

                PlayerProjectile playerProjectile = new PlayerProjectile();

                Random rnd = new Random();

                playerProjectile.Spawn(Position, 20f * Utilities.RotateVectorClockwise(Utilities.Normalise(mousePosition - Position), Utilities.ToRadians(rnd.Next(-10, 10))), 0.5f, Main.player.Texture, 1);
            }

            else if (ActiveWeapon == Weapons.Homing)
            {
                ShotCooldown = 10f;
                float initialVelocity = 7f;
                PlayerHomingProjectile projectile = new PlayerHomingProjectile();

                

                projectile.Spawn(Position, initialVelocity * Utilities.Normalise(mousePosition - Position), 0.3f, Main.player.Texture, 20);


            }
        }
    }
}

