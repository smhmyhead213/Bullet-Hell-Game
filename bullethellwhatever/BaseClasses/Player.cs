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
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Abilities.Weapons;
using bullethellwhatever.BaseClasses.Entities;

namespace bullethellwhatever.BaseClasses
{
    public class Player : NPC
    {
        public float DefaultMoveSpeed => 5.5f;
        public int DashCooldown;
        public int DashDuration => 10;

        public Dash DashAbility;
        
        public int DashTimer;
        public Vector2 DefaultHitbox => new Vector2(1f, 5f);

        public int WeaponSwitchCooldownTimer;
        public int WeaponSwitchCooldown;
        public float MoveSpeed;

        public bool Restarted;
        public bool InputLocked;

        public enum Weapons
        {
            Homing,
            MachineGun,        
        }

        public Weapons ActiveWeapon;

        #region Spawning
        public Player(string texture)
        {
            DashAbility = new Dash(DashDuration, 40, Keys.Space, this);
            Texture = AssetRegistry.GetTexture2D(texture);
            InputLocked = false;
        }
        public void Spawn(Vector2 position, Vector2 initialVelocity, float damage, string texture) //initialise player data
        {
            Position = position;
            Velocity = initialVelocity;

            Participating = true;

            IFrames = 0;
            MaxIFrames = 30;

            MaxHP = 1;

            Health = MaxHP; // put this back to normal
            Scale = DefaultHitbox;
            ActiveWeapon = Weapons.Homing;

            MoveSpeed = DefaultMoveSpeed;

            WeaponSwitchCooldown = 15;

            WeaponSwitchCooldownTimer = 0;

            ShouldRemoveOnEdgeTouch = false;

            PrimitiveTrail trail = new PrimitiveTrail(this, 10);
            trail.SetName("PlayerTrail");
            trail.Opacity = EasingFunctions.EaseParabolic(0.9f); // a bug lead to me finding this nice number

            AdditionalComponents.Add(trail);

            Colour = Color.White;
           
            SetHitbox();

            Restarted = false;

            PlayerWeaponManager.Initialise(this);

            AITimer = 0;
        }
        #endregion

        #region AI
        public PrimitiveTrail? GetTrail()
        {
            foreach (Component comp in AdditionalComponents)
            {
                if (comp.Name == "PlayerTrail")
                {
                    return (PrimitiveTrail)comp;
                }
            }

            return null; // if this is hit we're in trouble
        }
        public void SwitchWeapon(Weapons weapon)
        {
            ActiveWeapon = weapon;
            WeaponSwitchCooldownTimer = WeaponSwitchCooldown;
        }

        public void RotateBasedOnDirection()
        {
            if (Velocity == Vector2.Zero)
            {
                Rotation = 0;
                return;
            }

            float angleTo = Utilities.SmallestAngleTo(Rotation, Utilities.VectorToAngle(Velocity));

            Rotation += angleTo;
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
                Velocity.Y = Velocity.Y - MoveSpeed;
            }

            if (upPressed && TouchingTop())
            {
                Position.Y = MainCamera.VisibleArea.Top + Texture.Height / 2f * GetSize().Y;
            }

            if (downPressed && !TouchingBottom())
            {
                Velocity.Y = Velocity.Y + MoveSpeed;
            }

            if (downPressed && TouchingBottom())
            {
                Position.Y = MainCamera.VisibleArea.Bottom + GameHeight - (Texture.Height / 2f * GetSize().Y);
            }

            if (leftPressed && !TouchingLeft())
            {
                Velocity.X = Velocity.X - MoveSpeed;
            }

            if (leftPressed && TouchingLeft())
            {
                Position.X = MainCamera.VisibleArea.Left + Texture.Width / 2f * GetSize().X;
            }

            if (rightPressed && !TouchingRight())
            {
                Velocity.X = Velocity.X + MoveSpeed;
            }

            if (rightPressed && TouchingRight())
            {
                Position.X = MainCamera.VisibleArea.Right + GameWidth - (Texture.Width / 2f * GetSize().X);
            }

            Velocity = MoveSpeed * Utilities.SafeNormalise(Velocity);

            if (IsKeyPressed(Keys.LeftShift))
            {
                MoveSpeed = DefaultMoveSpeed / 2;
                Scale = DefaultHitbox / 2;

            }
            else
            {
                MoveSpeed = DefaultMoveSpeed;
                Scale = DefaultHitbox;
            }

            ControlCamera();

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

            Velocity = Vector2.Zero;

            if (!InputLocked)
            {
                HandleKeyPresses();
            }

            PlayerWeaponManager.Update();

            Position = Position + Velocity;

            RotateBasedOnDirection();

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

        public void LockMovement()
        {
            InputLocked = true;
        }
        public void UnlockMovement()
        {
            InputLocked = false;
        }
        public void ControlCamera()
        {
            if (!MainCamera.Locked)
            {
                MainCamera.Position = Vector2.LerpPrecise(MainCamera.Position, player.Position, 0.03f);

                float minZoom = 1f;
                float maxZoom = 2f;

                NPC furthest = FurthestEnemyFromPlayer();

                if (furthest is not null)
                {
                    float distance = Utilities.DistanceBetweenVectors(Position, furthest.Position);

                    float scaleFactor = Min(minZoom, (GameHeight / 2) / distance);

                    if (scaleFactor < 1f / maxZoom)
                    {
                        scaleFactor = 1f / maxZoom;
                    }

                    MainCamera.CameraScale = MathHelper.Lerp(MainCamera.CameraScale, scaleFactor, 0.1f);
                }
                else
                {
                    MainCamera.CameraScale = MathHelper.Lerp(MainCamera.CameraScale, minZoom, 0.1f);
                }
            }
        }

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

            PlayerWeaponManager.ActiveWeapon.Draw(spriteBatch);

            Opacity = 4f * (1f / (IFrames + 1f)); //to indicate iframes

            //Draw the player, accounting for immunity frame transparency.

            Drawing.BetterDraw(Texture, Position, null, Color.White * Opacity, Rotation, GetSize(), SpriteEffects.None, 0f);

            foreach (Component component in AdditionalComponents)
            {
                component.Draw(spriteBatch);
            }

            DrawHitbox();
        }
    }
}

