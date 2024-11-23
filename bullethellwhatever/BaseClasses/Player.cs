using Microsoft.Xna.Framework;
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

            MoveSpeed = DefaultMoveSpeed;

            WeaponSwitchCooldown = 15;

            WeaponSwitchCooldownTimer = 0;

            ShouldRemoveOnEdgeTouch = false;


            Colour = Color.White;

            Hitbox = new RotatedRectangle(Rotation, Texture.Width * GetSize().X, Texture.Height * GetSize().Y, Position, this);
            SetHitbox();

            Restarted = false;

            AITimer = 0;

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

            if (upPressed && !TouchingTop())
            {
                Velocity.Y = Velocity.Y - 1f;
            }

            if (upPressed && TouchingTop())
            {
                Position.Y = MainCamera.VisibleArea.Top + Texture.Height / 2f * GetSize().Y;
            }

            if (downPressed && !TouchingBottom())
            {
                Velocity.Y = Velocity.Y + 1f;
            }

            if (downPressed && TouchingBottom())
            {
                Position.Y = MainCamera.VisibleArea.Bottom + GameHeight - (Texture.Height / 2f * GetSize().Y);
            }

            if (leftPressed && !TouchingLeft())
            {
                Velocity.X = Velocity.X - 1f;
            }

            if (leftPressed && TouchingLeft())
            {
                Position.X = MainCamera.VisibleArea.Left + Texture.Width / 2f * GetSize().X;
            }

            if (rightPressed && !TouchingRight())
            {
                Velocity.X = Velocity.X + 1f;
            }

            if (rightPressed && TouchingRight())
            {
                Position.X = MainCamera.VisibleArea.Right + GameWidth - (Texture.Width / 2f * GetSize().X);
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

            ControlCamera();

            if (GameState.WeaponSwitchControl == GameState.WeaponSwitchControls.ScrollWheel)
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

                if (IsKeyPressed(Keys.Q) && EntityManager.activeNPCs.Count == 0)
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

        public override void Delete()
        {
            // add something if the player will ever be deleted
        }
        public override void AI() //cooldowns and iframes and stuff are handled here
        {
            AITimer++;

            var mouseState = Mouse.GetState();

            if (WeaponSwitchCooldownTimer > 0)
            {
                WeaponSwitchCooldownTimer--;
            }

            if (IsKeyPressedAndWasntLastFrame(Keys.R))
            {
                EntityManager.Clear();
                EntityManager.SpawnBoss();
            }

            Velocity = Vector2.Zero; //this will change if anything is pressed

            HandleKeyPresses();

            Position = Position + MoveSpeed * Utilities.SafeNormalise(Velocity, Vector2.Zero);

            foreach (Component component in AdditionalComponents)
            {
                component.Update();
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

                if (mouseState.LeftButton == ButtonState.Pressed && ShotCooldownRemaining == 0 && AITimer > 10)
                {
                    ShotCooldownRemaining = ShotCooldown;
                    Shoot();
                }              
            }
            else
            {
                Die();
            }
        }
        #endregion

        public override void Die()
        {
            Health = MaxHP;

            Position = Utilities.CentreOfScreen();

            musicSystem.StopMusic();

            Utilities.InitialiseGame();

            UI.CreateAfterBossMenu();
        }
        public void FullHeal()
        {
            Heal(MaxHP - Health);
        }

        public void ControlCamera()
        {
            MainCamera.Position = Vector2.LerpPrecise(MainCamera.Position, player.Position, 0.1f);

            float minZoom = 1f;
            float maxZoom = 2f;

            NPC furthest = FurthestEnemyFromPlayer();

            if (furthest is not null)
            {
                float distance = Utilities.DistanceBetweenVectors(Position, furthest.Position);

                float scaleFactor = Min(minZoom, (GameHeight / 2) / distance);

                if (scaleFactor > maxZoom)
                {
                    scaleFactor = maxZoom;
                }

                MainCamera.CameraScale = MathHelper.Lerp(MainCamera.CameraScale, scaleFactor, 0.1f);
            }
            else
            {
                MainCamera.CameraScale = MathHelper.Lerp(MainCamera.CameraScale, minZoom, 0.1f);
            }

        }

        #region Shooting

        public void Shoot()
        {
            if (ActiveWeapon == Weapons.Laser)
            {
                ShotCooldown = 1f;

                float initialRotation = Utilities.VectorToAngle(MousePositionWithCamera() - Position);

                PlayerDeathray.Rotation = initialRotation;
            }

            else if (ActiveWeapon == Weapons.MachineGun)
            {
                ShotCooldown = 3f;

                Random rnd = new Random();

                Projectile playerProjectile = SpawnProjectile(Position, 20f * Utilities.RotateVectorClockwise(Utilities.Normalise(MousePositionWithCamera() - Position), Utilities.ToRadians(rnd.Next(-10, 10))),
                    0.4f, 1, "MachineGunProjectile", Vector2.One, this, false, Color.LightBlue, true, true);

                playerProjectile.Rotation = Utilities.VectorToAngle(Utilities.RotateVectorClockwise(Utilities.Normalise(MousePositionWithCamera() - Position), Utilities.ToRadians(rnd.Next(-10, 10))));
            }

            else if (ActiveWeapon == Weapons.Homing)
            {
                ShotCooldown = 10f;
                float initialVelocity = 7f;
                float damage = 0.28f * 100f;

                Projectile projectile = SpawnProjectile(Position, initialVelocity * Utilities.Normalise(MousePositionWithCamera() - Position), damage, 1, "box", Vector2.One, this, false, Color.LimeGreen, true, true);

                projectile.SetExtraData(0, 0); // extra data 0 represents how long the projectile has gone without a target

                projectile.SetUpdates(2);

                projectile.AddTrail(22, "PrimitiveTestShader");

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

                        if (EntityManager.activeNPCs.Count > 0)
                        {
                            projectile.Opacity = 1f; // come back to full opacity if a target is found while fading out

                            foreach (NPC npc in EntityManager.activeNPCs)
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

        public NPC? FurthestEnemyFromPlayer()
        {
            if (EntityManager.activeNPCs.Count == 0)
            {
                return null;
            }

            int indexOfFurthest = 0;
            float furthestDistance = 0f;
            bool foundConsideredNPC = false;

            for (int i = 0; i < EntityManager.activeNPCs.Count; i++) 
            {
                if (EntityManager.activeNPCs[i].ConsideredForCameraZoom())
                {
                    foundConsideredNPC = true;

                    float distance = Utilities.DistanceBetweenVectors(EntityManager.activeNPCs[i].Position, player.Position);
                    if (distance > furthestDistance)
                    {
                        indexOfFurthest = i;
                        furthestDistance = distance;
                    }
                }
            }

            if (!foundConsideredNPC)
            {
                return null;
            }

            return EntityManager.activeNPCs[indexOfFurthest];
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsKeyPressed(Keys.K))
            {
                for (int i = 0; i < EntityManager.activeProjectiles.Count; i++)
                    Utilities.drawTextInDrawMethod(EntityManager.activeProjectiles[i].ToString() + " " + EntityManager.activeProjectiles[i].ShouldRemoveOnEdgeTouch.ToString() + " " + EntityManager.activeProjectiles[i].TimeOutsidePlayArea.ToString(), new Vector2(GameWidth / 3, GameHeight / 3 + 10 * i), spriteBatch, font, Colour); ;
            }

            Opacity = 4f * (1f / (IFrames + 1f)); //to indicate iframes

            //Draw the player, accounting for immunity frame transparency.

            Drawing.BetterDraw(Texture, Position, null, Color.White * Opacity, Rotation, GetSize(), SpriteEffects.None, 0f);

            foreach (Component component in AdditionalComponents)
            {
                component.Draw(spriteBatch);
            }
        }
    }
}

