using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Player;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;

namespace bullethellwhatever.BaseClasses
{
    public class Player : Entity
    {
        #region Fields 
        public float IFrames;
        public float ShotCooldown;
        public float ShotCooldownRemaining;
        public float ScrollCooldown;
        public float MoveSpeed;
        private Deathray PlayerDeathray = new Deathray();

        public enum Weapons
        {
            Laser,
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
            Size = Vector2.One;
            ShotCooldown = 20f;
            ShotCooldownRemaining = 0f;
            ActiveWeapon = Weapons.Homing;
            MoveSpeed = 5.5f;
            ScrollCooldown = 0f;
        }
        #endregion

        #region Input Handling

        #endregion
        public override bool ShouldRemoveOnEdgeTouch() => false;
        #region AI
        public override void AI() //cooldowns and iframes and stuff are handled here
        {
            var kstate = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            PlayerDeathray.Origin = Position;

            if (ScrollCooldown > 0)
            {
                ScrollCooldown--;
            }

            Velocity = Vector2.Zero;

            if (kstate.IsKeyDown(Keys.W) && !touchingTop(this)) //fix the movement so you dont move faster diagonally, future you's problem
            {
                Velocity.Y = Velocity.Y - 1f;
            }

            if (kstate.IsKeyDown(Keys.S) && !touchingBottom(this, Main._graphics.PreferredBackBufferHeight))
            {
                Velocity.Y = Velocity.Y + 1f;
            }

            if (kstate.IsKeyDown(Keys.A) && !touchingLeft(this))
            {
                Velocity.X = Velocity.X - 1f;
            }

            if (kstate.IsKeyDown(Keys.D) && !touchingRight(this, Main._graphics.PreferredBackBufferWidth))
            {
                Velocity.X = Velocity.X + 1f;
            }

            Position = Position + MoveSpeed * Utilities.SafeNormalise(Velocity, Vector2.Zero);
            if (GameState.WeaponSwitchControl) //if scroll wheel controls
            {
                if (mouseState.ScrollWheelValue / 120 % 3 == 0 && ScrollCooldown == 0)  //are you happy now gemma??????????????????????
                {
                    ActiveWeapon = Weapons.Laser;
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
                if (kstate.IsKeyDown(Keys.D1))  //are you happy now gemma??????????????????????
                {
                    ActiveWeapon = Weapons.Laser;
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
                Main.activeButtons.Clear();
            }



            //I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU
            Hitbox = new((int)Position.X - Texture.Width / 2, (int)Position.Y - Texture.Height / 2, Texture.Width, Texture.Height);

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
                    if (projectile.IsCollidingWithEntity(projectile, this) && IFrames == 0f)
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

                if (!(mouseState.LeftButton == ButtonState.Pressed))
                {
                    PlayerDeathray.IsActive = false;
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

            if (ActiveWeapon == Weapons.Laser)
            {
                ShotCooldown = 1f;

                float initialRotation = MathF.PI / 2 + Utilities.VectorToAngle(mousePosition - Position);

                PlayerDeathray.SpawnDeathray(Position, initialRotation, 0.13f, Texture, 5f, 2000f, 0f, 0f, this, false, Color.Yellow);
                
            }

            else if (ActiveWeapon == Weapons.MachineGun)
            {
                ShotCooldown = 3f;

                PlayerProjectile playerProjectile = new PlayerProjectile();

                Random rnd = new Random();

                playerProjectile.Spawn(Position, 20f * Utilities.RotateVectorClockwise(Utilities.Normalise(mousePosition - Position), Utilities.ToRadians(rnd.Next(-10, 10))),
                    0.15f, Main.player.Texture, 0, Vector2.One, this, false, Color.LightBlue);
            }

            else if (ActiveWeapon == Weapons.Homing)
            {
                ShotCooldown = 10f;
                float initialVelocity = 7f;
                PlayerHomingProjectile projectile = new PlayerHomingProjectile();



                projectile.Spawn(Position, initialVelocity * Utilities.Normalise(mousePosition - Position), 0.3f, Main.player.Texture, 0, Vector2.One, this, false, Color.LimeGreen);


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

