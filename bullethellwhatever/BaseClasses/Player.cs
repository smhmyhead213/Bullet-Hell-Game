﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;
using bullethellwhatever.DrawCode.UI.Buttons;
using bullethellwhatever.Abilities;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.DrawCode.UI;
using bullethellwhatever.Projectiles;
using bullethellwhatever.NPCs;
using bullethellwhatever.AssetManagement;

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
        public int WeaponSwitchCooldownTimer;
        public int WeaponSwitchCooldown;
        public float MoveSpeed;

        public Deathray PlayerDeathray;
        public bool Restarted;

        public DialogueSystem dialogueSystem;
        public enum Weapons
        {
            Homing,
            MachineGun,
            Laser,           
        }

        public Weapons ActiveWeapon;
        public Weapons PreviousWeapon;

        #region Spawning
        public Player(string texture)
        {
            dialogueSystem = new DialogueSystem(this);
            DashAbility = new Dash(DashDuration, 40, Keys.Space, this);
            Texture = AssetRegistry.GetTexture2D(texture);
        }
        public void Spawn(Vector2 position, Vector2 initialVelocity, float damage, string texture) //initialise player data
        {
            Position = position;
            Velocity = initialVelocity;

            Participating = true;

            IFrames = 0;
            Depth = 0;

            MaxHP = 15;

            Health = MaxHP; // put this back to normal
            Size = DefaultHitbox;
            ShotCooldown = 20f;
            ShotCooldownRemaining = 0f;
            ActiveWeapon = Weapons.Homing;

            MoveSpeed = 5.5f;

            WeaponSwitchCooldown = 15;

            WeaponSwitchCooldownTimer = 0;

            ShouldRemoveOnEdgeTouch = false;

            SetDrawAfterimages(DashDuration, 3);

            Colour = Color.White;

            Hitbox = new RotatedRectangle(Rotation, Texture.Width * GetSize().X, Texture.Height * GetSize().Y, Position, this);
            SetHitbox();

            Restarted = false;

            TimeAlive = 0;

            PlayerDeathray = SpawnDeathray(Position, 0f, 0.03f, 60, "box", 50f, 2000f, 0f, false, Color.Red, "PlayerDeathrayShader", this);

            PlayerDeathray.SetStayWithOwner(true);
            PlayerDeathray.SetDieAfterDuration(false);

            PlayerDeathray.SetExtraAI(new Action(() =>
            {
                PlayerDeathray.IsActive = IsLeftClickDown() && ActiveWeapon == Weapons.Laser;
            }));
        }
        #endregion

        #region Input Handling

        #endregion
        #region AI
        public void SwitchWeapon(Weapons weapon)
        {
            PreviousWeapon = ActiveWeapon;
            ActiveWeapon = weapon;
            WeaponSwitchCooldownTimer = WeaponSwitchCooldown;
        }
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

            if (upPressed && touchingTop(this))
            {
                Position.Y = Texture.Height / 2f * GetSize().Y;
            }

            if (downPressed && !touchingBottom(this))
            {
                Velocity.Y = Velocity.Y + 1f;
            }

            if (downPressed && touchingBottom(this))
            {
                Position.Y = IdealScreenHeight - (Texture.Height / 2f * GetSize().Y);
            }

            if (leftPressed && !touchingLeft(this))
            {
                Velocity.X = Velocity.X - 1f;
            }

            if (leftPressed && touchingLeft(this))
            {
                Position.X = Texture.Width / 2f * GetSize().X;
            }

            if (rightPressed && !touchingRight(this))
            {
                Velocity.X = Velocity.X + 1f;
            }

            if (rightPressed && touchingRight(this))
            {
                Position.X = IdealScreenWidth - (Texture.Width / 2f * GetSize().X);
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
                if (mouseState.ScrollWheelValue / 120 % 3 == 0 && WeaponSwitchCooldownTimer == 0)  //are you happy now Gemma??????????????????????
                {
                    SwitchWeapon(Weapons.Homing);
                }

                if (mouseState.ScrollWheelValue / 120 % 3 == 1 && WeaponSwitchCooldownTimer == 0)
                {
                    SwitchWeapon(Weapons.Laser);
                }

                if (mouseState.ScrollWheelValue / 120 % 3 == 2 && WeaponSwitchCooldownTimer == 0)
                {
                    SwitchWeapon(Weapons.MachineGun);
                }

                if (IsKeyPressed(Keys.Q) && activeNPCs.Count == 0)
                {
                    Health = MaxHP;
                    EntityManager.SpawnBoss();
                }
            }

            else
            {
                if (IsKeyPressed(Keys.D1) && WeaponSwitchCooldownTimer == 0)
                {
                    SwitchWeapon(Weapons.Homing);
                }

                if (IsKeyPressed(Keys.D2) && WeaponSwitchCooldownTimer == 0)
                {
                    SwitchWeapon(Weapons.Laser);
                }

                if (IsKeyPressed(Keys.D3) && WeaponSwitchCooldownTimer == 0)
                {
                    SwitchWeapon(Weapons.MachineGun);
                }
            }

            if (IsKeyPressed(Keys.Q) && activeNPCs.Count == 0 && !Utilities.ImportantMenusPresent() && !IsKeyPressed(Keys.R)) // haha suck it
            {
                Health = MaxHP;
                EntityManager.SpawnBoss();
                Restarted = false;

                foreach (Menu m in UIManager.ActiveUIElements) // none of the active menus are important so they can go bye bye
                {
                    m.Hide();
                }
            }

            if (IsKeyPressed(Keys.R) && Restarted == false)
            {
                Utilities.InitialiseGame();
                Restarted = true;

                BackButton start = new BackButton("StartButton", Vector2.One * 3, new Vector2(IdealScreenWidth / 4, IdealScreenHeight / 4));

                start.SetClickEvent(new Action(() =>
                {
                    GameState.SetGameState(GameState.GameStates.TitleScreen);
                }));

                start.AddToActiveUIElements();

                musicSystem.StopMusic();
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

            if (WeaponSwitchCooldownTimer > 0)
            {
                WeaponSwitchCooldownTimer--;
            }

            Velocity = Vector2.Zero; //this will change if anything is pressed

            HandleKeyPresses();

            if (IsKeyPressed(Keys.J))
            {
                foreach (Projectile p in activeFriendlyProjectiles)
                {
                    p.DeleteNextFrame = true;
                }
            }

            Position = Position + MoveSpeed * Utilities.SafeNormalise(Velocity, Vector2.Zero);

            Utilities.moveArrayElementsUpAndAddToStart(ref afterimagesPositions, Position);
            Utilities.moveArrayElementsUpAndAddToStart(ref afterimagesRotations, Rotation);

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
            }
            else
            {
                Health = MaxHP;
                Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

                musicSystem.StopMusic();

                Utilities.InitialiseGame();
            }
        }
        #endregion

        #region Shooting

        public void Shoot()
        {
            if (ActiveWeapon == Weapons.Laser)
            {
                ShotCooldown = 1f;

                float initialRotation = Utilities.VectorToAngle(MousePosition - Position);

                PlayerDeathray.Rotation = initialRotation;
            }

            else if (ActiveWeapon == Weapons.MachineGun)
            {
                ShotCooldown = 3f;

                Random rnd = new Random();

                Projectile playerProjectile = SpawnProjectile(Position, 20f * Utilities.RotateVectorClockwise(Utilities.Normalise(MousePosition - Position), Utilities.ToRadians(rnd.Next(-10, 10))),
                    0.4f, 1, "MachineGunProjectile", Vector2.One, this, false, Color.LightBlue, true, true);

                playerProjectile.Rotation = Utilities.VectorToAngle(Utilities.RotateVectorClockwise(Utilities.Normalise(MousePosition - Position), Utilities.ToRadians(rnd.Next(-10, 10))));
            }

            else if (ActiveWeapon == Weapons.Homing)
            {
                ShotCooldown = 10f;
                float initialVelocity = 7f;
                float damage = 0.28f * 100f;

                Projectile projectile = SpawnProjectile(Position, initialVelocity * Utilities.Normalise(MousePosition - Position), damage, 1, "box", Vector2.One, this, false, Color.LimeGreen, true, true);

                projectile.SetExtraData(0, 0); // extra data 0 represents how long the projectile has gone without a target

                projectile.SetUpdates(2);

                projectile.SetDrawAfterimages(22, 3);

                projectile.SetOnHit(new Action(() =>
                {
                    if (projectile.Owner == player)
                    {
                        int numberOfParticles = Utilities.RandomInt(1, 4);

                        for (int i = 0; i < numberOfParticles; i++)
                        {
                            float rotation = Utilities.RandomFloat(0, Tau);

                            Particle p = new Particle();

                            Vector2 velocity = 10f * Utilities.RotateVectorClockwise(-Vector2.UnitY, rotation);
                            int lifetime = 20;

                            p.Spawn("box", projectile.Position, velocity, -velocity / 2f / lifetime, Vector2.One * 0.45f, rotation, projectile.Colour, 1f, 20);
                        }
                    }
                }));
                projectile.SetExtraAI(new Action(() =>
                {
                    ref float TimeWithNoTarget = ref projectile.ExtraData[0];

                    int homingTime = 30 * projectile.Updates;

                    NPC closestNPC = new NPC(); //the target
                    float minDistance = float.MaxValue;

                    if (projectile.AITimer > homingTime)
                    {
                        bool validTarget = false;

                        if (activeNPCs.Count > 0)
                        {
                            projectile.Opacity = 1f; // come back to full opacity if a target is found while fading out

                            foreach (NPC npc in activeNPCs)
                            {
                                if (npc.TargetableByHoming && npc.Participating)
                                {
                                    float distance = Utilities.DistanceBetweenEntities(projectile, npc);
                                    if (distance < minDistance)
                                    {
                                        minDistance = distance;
                                        closestNPC = npc;
                                    }

                                    validTarget = true;
                                }
                            }

                            if (validTarget)
                                projectile.Velocity = 0.4f / projectile.Updates * Vector2.Normalize(closestNPC.Position - projectile.Position) * (projectile.AITimer - homingTime);
                        }

                        else
                        {
                            TimeWithNoTarget++;
                            projectile.Velocity = projectile.Velocity * 0.98f; // slow down if no target
                        }

                        if (!validTarget)
                        {
                            TimeWithNoTarget++;
                            projectile.Velocity = projectile.Velocity * 0.98f; // slow down if no target
                        }
                    }

                    int beginFading = 50;

                    if (TimeWithNoTarget >= beginFading) //after a second of not finding a target
                    {
                        // if almost expired, start fading out

                        int fadeOutTime = 60;

                        projectile.Opacity = MathHelper.Lerp(1f, 0f, ((float)TimeWithNoTarget - beginFading) / fadeOutTime);

                        if (TimeWithNoTarget > fadeOutTime + beginFading)
                        {
                            projectile.DeleteNextFrame = true;

                            projectile.OnHitEffect(projectile.Position);
                        }
                    }

                    projectile.Rotation = Utilities.VectorToAngle(projectile.Velocity);
                }));
            }
        }
        #endregion
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsKeyPressed(Keys.K))
            {
                for (int i = 0; i < activeProjectiles.Count; i++)
                    Utilities.drawTextInDrawMethod(activeProjectiles[i].ToString() + " " + activeProjectiles[i].ShouldRemoveOnEdgeTouch.ToString() + " " + activeProjectiles[i].TimeOutsidePlayArea.ToString(), new Vector2(Main.IdealScreenWidth / 3, Main.IdealScreenHeight / 3 + 10 * i), spriteBatch, font, Colour); ;
            }

            if (DashAbility.IsExecuting)
            {
                int extraImages = 3;

                for (int i = 0; i < afterimagesPositions.Length; i++)
                {
                    if (afterimagesPositions[i] != Vector2.Zero)
                    {
                        float colourMultiplier = (float)(afterimagesPositions.Length - (i + 1)) / (float)(afterimagesPositions.Length + 1) - 0.2f;
                        Drawing.BetterDraw(Main.player.Texture, afterimagesPositions[i], null, Colour * colourMultiplier, Rotation, GetSize() * (afterimagesPositions.Length - 1 - i) / afterimagesPositions.Length, SpriteEffects.None, 0f); //draw afterimages

                        // Draw another afterimage between this one and the last one, for a less choppy trail.

                        if (i > 0)
                        {
                            for (int j = 0; j < extraImages; j++)
                            {
                                float interpolant = (j + 1) * (1f / (extraImages + 1));

                                colourMultiplier = (float)(afterimagesPositions.Length - (i + 1) + interpolant) / (float)(afterimagesPositions.Length + 1) - 0.2f;

                                Drawing.BetterDraw(Main.player.Texture, Vector2.Lerp(afterimagesPositions[i], afterimagesPositions[i - 1], interpolant), null, Colour * colourMultiplier,
                                    afterimagesRotations[i], GetSize() * (afterimagesPositions.Length - 1 - i + interpolant) / afterimagesPositions.Length, SpriteEffects.None, 0f); //draw afterimages
                            }
                        }

                    }
                }

                DrawAfterimages = true;
            }
            else DrawAfterimages = false;

            Opacity = 4f * (1f / (IFrames + 1f)); //to indicate iframes

            //Draw the player, accounting for immunity frame transparency.

            Drawing.BetterDraw(Texture, Position, null, Color.White * Opacity, Rotation, GetSize(), SpriteEffects.None, 0f);
        }
    }
}

