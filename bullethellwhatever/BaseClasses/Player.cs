using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Player;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;

namespace bullethellwhatever.BaseClasses
{
    public class Player : Entity
    {
        public float DefaultMoveSpeed => 5.5f;
        public int DashCooldown;
        public int DashDuration => 10;
        public int DashTimer;
        public Vector2 DefaultHitbox => new Vector2(1f, 1f);

        public float IFrames;
        public float ShotCooldown;
        public float ShotCooldownRemaining;
        public float ScrollCooldown;
        public float MoveSpeed;
        public Deathray PlayerDeathray = new Deathray();

        public enum Weapons
        {
            Laser,
            MachineGun,
            Homing,

        }

        public Weapons ActiveWeapon;

        #region Spawning
        public void Spawn(Vector2 position, Vector2 initialVelocity, float damage, string texture) //initialise player data
        {
            Position = position;
            Velocity = initialVelocity;
            Texture = Main.Assets[texture];
            isPlayer = true;
            isBoss = false;
            IFrames = 0;
            Health = 15; //make all these values changeable
            MaxHP = Health;
            Size = Vector2.One;
            ShotCooldown = 20f;
            ShotCooldownRemaining = 0f;
            ActiveWeapon = Weapons.Laser;
            MoveSpeed = 5.5f;
            ScrollCooldown = 0f;
            ShouldRemoveOnEdgeTouch = false;
            afterimagesPositions = new Vector2[DashDuration];
            Colour = Color.White;
        }
        #endregion

        #region Input Handling

        #endregion
        #region AI
        public void HandleKeyPresses()
        {
            var kstate = Keyboard.GetState();
            var mouseState = Mouse.GetState();

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
                //float multiplier = 1f + (dashSpeed - 1f) * (DashTimer - 1f) / (DashDuration - 1f);
                float multiplier = 3f;
                MoveSpeed = MoveSpeed * multiplier;
            }
        }
        public override void AI() //cooldowns and iframes and stuff are handled here
        {

            var mouseState = Mouse.GetState();
            var kstate = Keyboard.GetState();

            if (ScrollCooldown > 0)
            {
                ScrollCooldown--;
            }

            Velocity = Vector2.Zero; //this will change if anything is pressed

            HandleKeyPresses();

            //I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU I HATE YOU
            SetHitbox(this);

            Position = Position + MoveSpeed * Utilities.SafeNormalise(Velocity, Vector2.Zero);

            Utilities.moveVectorArrayElementsUpAndAddToStart(ref afterimagesPositions, Position);

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

                PlayerDeathray.SpawnDeathray(Position, initialRotation, 0.13f, 2, "box", 50f, 2000f, 0f, 0f, false, Color.Yellow, "PlayerDeathrayShader", this);               
            }

            else if (ActiveWeapon == Weapons.MachineGun)
            {
                PlayerDeathray.IsActive = false;

                ShotCooldown = 3f;

                PlayerProjectile playerProjectile = new PlayerProjectile();

                Random rnd = new Random();

                playerProjectile.Spawn(Position, 20f * Utilities.RotateVectorClockwise(Utilities.Normalise(mousePosition - Position), Utilities.ToRadians(rnd.Next(-10, 10))),
                    0.15f, "box", 0, Vector2.One, this, false, Color.LightBlue, true, true);
            }

            else if (ActiveWeapon == Weapons.Homing)
            {
                PlayerDeathray.IsActive = false;
                ShotCooldown = 10f;
                float initialVelocity = 7f;
                PlayerHomingProjectile projectile = new PlayerHomingProjectile();
                
                projectile.Spawn(Position, initialVelocity * Utilities.Normalise(mousePosition - Position), 0.28f, "box", 0, Vector2.One, this, false, Color.LimeGreen, true, true);


            }
        }
        #endregion

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.activeProjectiles.Count > 0)
                Utilities.drawTextInDrawMethod(Main.activeFriendlyProjectiles.Count.ToString(), new Vector2(Main.ScreenWidth / 5, Main.ScreenHeight / 5), spriteBatch, Main.font, Color.White);

            if (DashTimer > 0)
            {
                for (int i = 0; i < afterimagesPositions.Length; i++)
                {
                    if (afterimagesPositions[i] != Vector2.Zero)
                    {
                        float colourMultiplier = (float)(afterimagesPositions.Length - (i + 1)) / (float)(afterimagesPositions.Length + 1) - 0.2f;
                        Drawing.BetterDraw(Main.player.Texture, afterimagesPositions[i], null, Colour * colourMultiplier, Rotation, Size * (afterimagesPositions.Length - 1 - i) / afterimagesPositions.Length, SpriteEffects.None, 0f); //draw afterimages

                        // Draw another afterimage between this one and the last one, for a less choppy trail.

                        if (i > 0)
                        {
                            colourMultiplier = (float)(afterimagesPositions.Length - (i + 1) + 0.5f) / (float)(afterimagesPositions.Length + 1) - 0.2f;

                            Drawing.BetterDraw(Main.player.Texture, Vector2.Lerp(afterimagesPositions[i - 1], afterimagesPositions[i], 0.5f), null, Colour * colourMultiplier,
                                Rotation, Size * (afterimagesPositions.Length - 1 - i + 0.5f) / afterimagesPositions.Length, SpriteEffects.None, 0f); //draw afterimages
                        }

                    }
                }
            }

            Main.player.Opacity = 4f * (1f / (Main.player.IFrames + 1f)); //to indicate iframes

            //Draw the player, accounting for immunity frame transparency.

            Drawing.BetterDraw(Main.player.Texture, Main.player.Position, null, Color.White * Main.player.Opacity, Main.player.Rotation, Main.player.Size, SpriteEffects.None, 0f);
        }
    }
}

