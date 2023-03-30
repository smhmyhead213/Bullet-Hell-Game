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
        #region Fields 
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

        #endregion
        #region Spawning
        public void Spawn(Vector2 position, Vector2 initialVelocity, float damage, Texture2D texture) //initialise player data
        {
            Position = position;
            Velocity = initialVelocity;
            Texture = texture;
            isPlayer = true;
            isBoss = false;
            IFrames = 0;
            Health = 15; //make all these values changeable
            MaxHP = Health;
            Size = 1f;
            ShotCooldown = 20f;
            ShotCooldownRemaining = 0f;
            
        }
        #endregion

        #region Input Handling
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
                Health = MaxHP;
                EntityManager.SpawnBoss();
            }

        }

        #endregion
        public override bool ShouldRemoveOnEdgeTouch() => false;
        #region AI
        public override void AI() //cooldowns and iframes and stuff are handled here
        {
            var mouseState = Mouse.GetState();

            HandleMovement();

            //I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU
            Hitbox = new((int)Position.X - (Texture.Width / 2), (int)Position.Y - (Texture.Height / 2),Texture.Width, Texture.Height);

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
                        TakeDamage(projectile);
                    }

                }

                foreach (NPC npc in Main.activeNPCs)
                {
                    if (npc.isCollidingWithPlayer() && IFrames == 0f && npc.ContactDamage == true)
                        TakeDamage(npc);

                }

                if (mouseState.LeftButton == ButtonState.Pressed && ShotCooldownRemaining == 0)
                {
                    ShotCooldownRemaining = ShotCooldown;
                    Shoot();
                }
            }
            else
            {
                Health = MaxHP;
                Position = new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2);
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

            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            if (ActiveWeapon == Weapons.Sharpshooter)
            {
                ShotCooldown = 20f;

                PlayerSharpShooterProjectile playerProjectile = new PlayerSharpShooterProjectile();

                

                playerProjectile.Spawn(Position, 30f * Utilities.Normalise(mousePosition - Position), 2f, Main.player.Texture, 0);
            }

            else if (ActiveWeapon == Weapons.MachineGun)
            {
                ShotCooldown = 3f;

                PlayerProjectile playerProjectile = new PlayerProjectile();

                Random rnd = new Random();

                playerProjectile.Spawn(Position, 20f * Utilities.RotateVectorClockwise(Utilities.Normalise(mousePosition - Position), Utilities.ToRadians(rnd.Next(-10, 10))), 0.25f, Main.player.Texture, 0);
            }

            else if (ActiveWeapon == Weapons.Homing)
            {
                ShotCooldown = 10f;
                float initialVelocity = 7f;
                PlayerHomingProjectile projectile = new PlayerHomingProjectile();

                

                projectile.Spawn(Position, initialVelocity * Utilities.Normalise(mousePosition - Position), 0.4f, Main.player.Texture, 0);


            }
        }
        #endregion
        public void TakeDamage(Entity entity) //take damage from an entity
        {
            Health = Health - entity.Damage;
            IFrames = 20f;
        }
    }
}

