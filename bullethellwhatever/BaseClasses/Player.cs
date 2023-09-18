using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Player;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Abilities;
using SharpDX.MediaFoundation;

namespace bullethellwhatever.BaseClasses
{
    public class Player : Entity
    {
        public float DefaultMoveSpeed => 5.5f;
        public int DashCooldown;
        public int DashDuration => 10;

        public int TimeAlive;

        public Dash DashAbility;
        
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
            Texture = Assets[texture];
            isPlayer = true;
            isBoss = false;
            IFrames = 0;
            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    Health = 15;
                    break;
                case GameState.Difficulties.Normal:
                    Health = 12;
                    break;
                case GameState.Difficulties.Hard:
                    Health = 10;
                    break;
                case GameState.Difficulties.Insane:
                    Health = 8;
                    break;
            }
            MaxHP = Health;
            Size = DefaultHitbox;
            ShotCooldown = 20f;
            ShotCooldownRemaining = 0f;
            ActiveWeapon = Weapons.Homing;
            MoveSpeed = 5.5f;
            ScrollCooldown = 0f;
            ShouldRemoveOnEdgeTouch = false;
            afterimagesPositions = new Vector2[DashDuration];
            Colour = Color.White;

            Hitbox = new RotatedRectangle(Rotation, Texture.Width * Size.X, Texture.Height * Size.Y, Position, this);
            SetHitbox();

            DashAbility = new Dash(DashDuration, 40, Keys.Space, this);

            TimeAlive = 0;
        }
        #endregion

        #region Input Handling

        #endregion
        #region AI
        public void HandleKeyPresses()
        {
            var mouseState = Mouse.GetState();

            bool upPressed = IsKeyPressed(Keys.W);
            bool downPressed = IsKeyPressed(Keys.S);
            bool rightPressed = IsKeyPressed(Keys.D);
            bool leftPressed = IsKeyPressed(Keys.A);

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

            if (IsKeyPressed(Keys.LeftShift))
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

                if (IsKeyPressed(Keys.Q) && Main.activeNPCs.Count == 0)
                {
                    Health = MaxHP;
                    EntityManager.SpawnBoss();
                }
            }

            else
            {
                if (IsKeyPressed(Keys.D1))
                {
                    ActiveWeapon = Weapons.Laser;
                    ScrollCooldown = 3f;
                }

                if (IsKeyPressed(Keys.D2))
                {
                    ActiveWeapon = Weapons.MachineGun;
                    ScrollCooldown = 3f;
                }

                if (IsKeyPressed(Keys.D3))
                {
                    ActiveWeapon = Weapons.Homing;
                    ScrollCooldown = 3f;
                }
            }

            if (IsKeyPressed(Keys.Q) && Main.activeNPCs.Count == 0)
            {
                Health = MaxHP;               
                EntityManager.SpawnBoss();
                Main.activeButtons.Clear();
            }

            if (IsKeyPressed(Keys.R))
            {
                Utilities.InitialiseGame();
                Main.musicSystem.StopMusic();
            }

            if (DashCooldown > 0)
                DashCooldown--;

            if (DashTimer > 0)
                DashTimer--;

            DashAbility.Execute();


            //if (DashAbility.Timer > 0)
            //{
            //    //float multiplier = 1f + (dashSpeed - 1f) * (DashTimer - 1f) / (DashDuration - 1f);
            //    float multiplier = 3f;
            //    MoveSpeed = MoveSpeed * multiplier;
            //}
        }
        public override void AI() //cooldowns and iframes and stuff are handled here
        {
            TimeAlive++;

            var mouseState = Mouse.GetState();

            if (ScrollCooldown > 0)
            {
                ScrollCooldown--;
            }

            Velocity = Vector2.Zero; //this will change if anything is pressed

            HandleKeyPresses();

            if (IsKeyPressed(Keys.J))
            {
                foreach (Projectile p in Main.activeFriendlyProjectiles)
                {
                    p.DeleteNextFrame = true;
                }
            }

            Position = Position + MoveSpeed * Utilities.SafeNormalise(Velocity, Vector2.Zero);

            Utilities.moveVectorArrayElementsUpAndAddToStart(ref afterimagesPositions, Position);

            if (ActiveWeapon != Weapons.Laser)
            {
                PlayerDeathray.IsSpawned = false;
                Main.activeFriendlyProjectiles.Remove(PlayerDeathray); //i need to fix this crap one day
            }

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

                if ((mouseState.LeftButton == ButtonState.Pressed || IsKeyPressed(Keys.Enter)) && ShotCooldownRemaining == 0 && TimeAlive > 10)
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

                Main.musicSystem.StopMusic();

                Utilities.InitialiseGame();
            }
        }
        #endregion

        public void Dash()
        {

        }
        #region Shooting
        public void Shoot()
        {
            if (ActiveWeapon == Weapons.Laser)
            {
                ShotCooldown = 1f;

                float initialRotation = Utilities.VectorToAngle(MousePosition - Position); // Add an offset so it works I have no idea why

                PlayerDeathray.SpawnDeathray(Position, initialRotation, 0.1f, 2, "box", 50f, 2000f, 0f, 0f, false, Color.Yellow, "PlayerDeathrayShader", this);               
            }

            else if (ActiveWeapon == Weapons.MachineGun)
            {
                PlayerDeathray.IsActive = false;

                ShotCooldown = 3f;

                Projectile playerProjectile = new Projectile();

                Random rnd = new Random();

                playerProjectile.Spawn(Position, 20f * Utilities.RotateVectorClockwise(Utilities.Normalise(MousePosition - Position), Utilities.ToRadians(rnd.Next(-10, 10))),
                    0.15f, 1, "box", 0, Vector2.One, this, false, Color.LightBlue, true, true);
            }

            else if (ActiveWeapon == Weapons.Homing)
            {
                PlayerDeathray.IsActive = false;
                ShotCooldown = 10f;
                float initialVelocity = 7f;
                PlayerHomingProjectile projectile = new PlayerHomingProjectile();

                float damage = IsKeyPressed(Keys.N) ? 28f : 0.28f; // debug

                projectile.Spawn(Position, initialVelocity * Utilities.Normalise(MousePosition - Position), damage, 1, "box", 0, Vector2.One, this, false, Color.LimeGreen, true, true);


            }
        }
        #endregion

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsKeyPressed(Keys.K))
            {
                for (int i = 0; i < Main.activeProjectiles.Count; i++)
                    Utilities.drawTextInDrawMethod(Main.activeProjectiles[i].ToString() + " " + activeProjectiles[i].ShouldRemoveOnEdgeTouch.ToString() + " " + activeProjectiles[i].TimeOutsidePlayArea.ToString(), new Vector2(Main.ScreenWidth / 3, Main.ScreenHeight / 3 + 10 * i), spriteBatch, Main.font, Colour); ;
            }

            if (DashAbility.IsExecuting)
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
                DrawAfterimages = true;
            }
            else DrawAfterimages = false;

            Main.player.Opacity = 4f * (1f / (IFrames + 1f)); //to indicate iframes

            //Draw the player, accounting for immunity frame transparency.

            Drawing.BetterDraw(Main.player.Texture, Main.player.Position, null, Color.White * Main.player.Opacity, Main.player.Rotation, Main.player.Size, SpriteEffects.None, 0f);
        }
    }
}

