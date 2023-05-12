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
        public float DefaultMoveSpeed => 5.5f;
        public int DashCooldown;
        public int DashDuration => 10;
        public int DashTimer;
        public Vector2 DefaultHitbox => new Vector2(1f, 1f);
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
            ShouldRemoveOnEdgeTouch = false;
        }
        #endregion

        #region Input Handling

        #endregion
        #region AI
        public override void AI() //cooldowns and iframes and stuff are handled here
        {
            var kstate = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            PlayerDeathray.Position = Position; // Make sure the deathray is constantly centred on the player (abstract this away)

            if (ScrollCooldown > 0)
            {
                ScrollCooldown--;
            }

            Velocity = Vector2.Zero; //this will change if anything is pressed

            bool upPressed = kstate.IsKeyDown(Keys.W);
            bool downPressed = kstate.IsKeyDown(Keys.S);
            bool rightPressed = kstate.IsKeyDown(Keys.D);
            bool leftPressed = kstate.IsKeyDown(Keys.A);

            if (upPressed && !touchingTop(this))
            {
                Velocity.Y = Velocity.Y - 1f;
            }

            if (downPressed && !touchingBottom(this))
            {
                Velocity.Y = Velocity.Y + 1f;
            }

            if (leftPressed && !touchingLeft(this))
            {
                Velocity.X = Velocity.X - 1f;
            }

            if (rightPressed && !touchingRight(this))
            {
                Velocity.X = Velocity.X + 1f;
            }

            if (kstate.IsKeyDown(Keys.LeftShift))
            {
                MoveSpeed = DefaultMoveSpeed / 2;
                Size = DefaultHitbox / 2;

            }
            else
            {
                MoveSpeed = DefaultMoveSpeed;
                Size = DefaultHitbox;
            }



            if (GameState.WeaponSwitchControl) //if scroll wheel controls
            {
                if (mouseState.ScrollWheelValue / 120 % 3 == 0 && ScrollCooldown == 0)  //are you happy now Gemma??????????????????????
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
                if (kstate.IsKeyDown(Keys.D1))  
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

            if (kstate.IsKeyDown(Keys.R))
            {
                Utilities.InitialiseGame();
            }

            if (DashCooldown > 0) 
                DashCooldown--;

            if (DashTimer > 0)
                DashTimer--;

            if (kstate.IsKeyDown(Keys.Space) && DashCooldown == 0)
            {
                //MoveSpeed = MoveSpeed * 3;
                DashCooldown = 40;
                DashTimer = DashDuration;
                IFrames = DashDuration;
            }

            if (DashTimer > 0)
            {
                float dashSpeed = 5f;
                
                float multiplier = 1f + (dashSpeed - 1f) * (DashTimer - 1f) / (DashDuration - 1f);
                //multiplier = 3f;
                MoveSpeed = MoveSpeed * multiplier;
            }

            //I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU
            SetHitbox(this);

            Position = Position + MoveSpeed * Utilities.SafeNormalise(Velocity, Vector2.Zero);

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

                if ((mouseState.LeftButton == ButtonState.Pressed || kstate.IsKeyDown(Keys.Enter)) && ShotCooldownRemaining == 0)
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

        public void Dash()
        {

        }
        #region Shooting
        public void Shoot()
        {
            var mouseState = Mouse.GetState();

            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            if (ActiveWeapon == Weapons.Laser)
            {
                ShotCooldown = 1f;

                float initialRotation = Utilities.VectorToAngle(mousePosition - Position) - MathHelper.PiOver2; // Add an offset so it works I have no idea why

                PlayerDeathray.SpawnDeathray(Position, initialRotation, 0.13f, 999, Texture, 10f, 2000f, 0f, 0f, false, Color.Yellow, Main.deathrayShader, this);               
            }

            else if (ActiveWeapon == Weapons.MachineGun)
            {
                PlayerDeathray.IsActive = false;

                ShotCooldown = 3f;

                PlayerProjectile playerProjectile = new PlayerProjectile();

                Random rnd = new Random();

                playerProjectile.Spawn(Position, 20f * Utilities.RotateVectorClockwise(Utilities.Normalise(mousePosition - Position), Utilities.ToRadians(rnd.Next(-10, 10))),
                    0.15f, Main.player.Texture, 0, Vector2.One, this, false, Color.LightBlue, true, true);
            }

            else if (ActiveWeapon == Weapons.Homing)
            {
                PlayerDeathray.IsActive = false;
                ShotCooldown = 10f;
                float initialVelocity = 7f;
                PlayerHomingProjectile projectile = new PlayerHomingProjectile();
                
                projectile.Spawn(Position, initialVelocity * Utilities.Normalise(mousePosition - Position), 0.28f, Main.player.Texture, 0, Vector2.One, this, false, Color.LimeGreen, true, true);


            }
        }
        #endregion
        //public void TakeDamage(Entity entity) //take damage from an entity
        //{
        //    Health = Health - entity.Damage;
        //    IFrames = 20f;
            
        //}
    }
}

