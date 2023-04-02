﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

namespace bullethellwhatever
{
    public class TestBoss : Boss
    {
        public bool HasChosenChargeDirection;
        public float SpiralStartTime;
        public int AttackNumber; //position in pattern
        public float ShotgunFrequency;

        public TestBoss(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
            isBoss = true;
            isPlayer = false;
            Health = 150;
            AITimer = 0f;
            IFrames = 5f;
            HasChosenChargeDirection = false;
            AttackNumber = 1;
            ShotgunFrequency = 15f;
        }

        public override bool ShouldRemoveOnEdgeTouch() => false;

        public override void Spawn(Vector2 position, Vector2 initialVelocity, float damage, Texture2D texture, float size, float MaxHealth)
        {
            base.Spawn(position, initialVelocity, damage, texture, size, MaxHealth);

        }

        public override void HandleMovement()
        {

        }

        public override void AI()
        {
            if (Health < 0)
                DeleteNextFrame = true;



            //Update the boss position based on its velocity.
            Position = Position + Velocity;


            if (IFrames > 0)
            {
                IFrames--;
            }
            //If the timer reaches 10, execute the attack and reset timer.

            switch (AttackNumber)
            {
                case 1:
                    BasicShotgunBlast(ref AITimer, ref AttackNumber, Position, 5f, 24);
                    break;
                case 2:
                    Charge(ref AITimer, ref AttackNumber);
                    break;
                case 3:
                    Spiral(ref AITimer, ref AttackNumber);
                    break;
                case 4:
                    ObnoxiouslyDenseBulletHell(ref AITimer, ref AttackNumber);
                    break;
                case 5:
                    MoveTowardsAndShotgun(ref AITimer, ref AttackNumber);
                    break;
                case 6:
                    MutantBulletHell(ref AITimer, ref AttackNumber);
                    break;
                case 7:
                    if (AITimer % 2 == 0)
                        HorizontalChargesWithProjectiles(ref AITimer, ref AttackNumber);
                    break;
                default:
                    AttackNumber = 1;
                    break;

            }



            //Every frame, add 1 to the timer.
            AITimer++;

            foreach (FriendlyProjectile projectile in Main.activeFriendlyProjectiles)
            {
                if (isCollidingWithPlayerProjectile(projectile) && IFrames == 0)
                {
                    if (IFrames == 0)
                    {
                        IFrames = 5f;
                        Health = Health - projectile.Damage;
                        projectile.DeleteNextFrame = true;
                    }
                }
            }
        }

        public void BasicShotgunBlast(ref float AITimer, ref int AttackNumber, Vector2 bossPosition, float projectileSpeed, int numberOfProjectiles)
        {

            if (AITimer % 40 == 0 && AITimer < 601) //1 added so the 15th burst goes off
            {
                BasicProjectile singleShot = new BasicProjectile();
                singleShot.Spawn(bossPosition, projectileSpeed * Utilities.Normalise(Main.player.Position - Position), 1f, Texture, 0);

                for (int i = 1; i < (numberOfProjectiles / 2) + 0.5f; i++) // loop for each pair of projectiles an angle away from the middle
                {
                    BasicProjectile shotgunBlast = new BasicProjectile();
                    BasicProjectile shotgunBlast2 = new BasicProjectile(); //one for each side of middle
                    shotgunBlast.Spawn(bossPosition, projectileSpeed * Utilities.Normalise(Utilities.RotateVectorClockwise(Main.player.Position - bossPosition, i * MathF.PI / 12)), 1f, Texture, 0);
                    shotgunBlast2.Spawn(bossPosition, projectileSpeed * Utilities.Normalise(Utilities.RotateVectorCounterClockwise(Main.player.Position - bossPosition, i * MathF.PI / 12)), 1f, Texture, 0);

                }
            }

            if (AITimer == 750)
            {
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }

            HandleBounces();

        }

        public void Charge(ref float AITimer, ref int AttackNumber)
        {
            if (AITimer == 0)
            {
                ContactDamage = true; //turn on contact damage
            }

            if (!HasChosenChargeDirection)
            {
                Velocity = 9f * Utilities.Normalise(Main.player.Position - Position);
                HasChosenChargeDirection = true; //charge
            }

            Velocity = 7.5f * Utilities.SafeNormalise(Velocity * (MathF.Sin(AITimer) + 1f), Vector2.Zero); //smoother charging


            SpinUpClockwise(ref Rotation, 20f);

            if (AITimer % 150 == 0)
            {
                HasChosenChargeDirection = false; //enable the next charge to start
            }

            if (AITimer % 150 > 0 && AITimer % 150 < 76 && (AITimer % 30) % 2 == 0)// check if aitimer is between 1 and 15 and if its even
            {
                BasicProjectile projectile = new BasicProjectile();
                projectile.Spawn(Position, 5f * Utilities.Normalise(Main.player.Position - Position), 1f, Texture, 1.02f);
            }

            if (AITimer == 900)
            {
                EndAttack(ref AITimer, ref AttackNumber);
                ContactDamage = false;
                return;
            }


            HandleBounces();
        }

        public void Spiral(ref float AITimer, ref int AttackNumber)
        {
            if (AITimer == 0)
            {
                Position = new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2);
                Velocity = Vector2.Zero; //amke it sit in the middle
            }

            List<BasicProjectile> projectilesToShoot = new List<BasicProjectile>();

            if (AITimer <= 240)
            {
                SpinUpCounterClockwise(ref Rotation, 60f);
            }

            if (AITimer % 2 == 0 && AITimer > 240 && AITimer < 1700)
            {
                int projectilesInSpiral = 10;
                float acceleration = 0.52f * MathF.Cos(AITimer / 250 + MathF.PI / 3);
                float rotation = (AITimer / 15 * MathF.PI / 40f) * acceleration;

                Rotation = rotation;

                for (int i = 0; i < projectilesInSpiral; i++)
                {
                    projectilesToShoot.Add(new BasicProjectile()); //add a projectile

                    // shoot projectiles in a ring and rotate it based on time
                    Vector2 velocity = 7f * Utilities.SafeNormalise(Utilities.RotateVectorCounterClockwise(new Vector2(0, -1), Utilities.ToRadians(i * 45) + rotation), Vector2.Zero);

                    projectilesToShoot[i].Spawn(Position, velocity, 1f, Texture, 1);


                }


            }

            if (AITimer == 1800)
            {
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }

        }

        public void ObnoxiouslyDenseBulletHell(ref float AITimer, ref int AttackNumber)
        {
            if (AITimer == 0)
            {
                Position = new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 18);
                Velocity = new Vector2(-1.5f, 0);
            }

            if (AITimer % 30 == 0 && AITimer < 500)
            {
                int projectilesPerWave = 25;

                List<BasicProjectile> projectilesToShoot = new List<BasicProjectile>();

                Random rnd = new Random();

                Vector2 projectileDirection = new Vector2(1, 0);
                for (int i = 0; i < projectilesPerWave; i++)
                {
                    projectilesToShoot.Add(new BasicProjectile());

                    //Shoot projectiles based on an offset.
                    projectilesToShoot[i].Spawn(Position, 5f * Vector2.Normalize(Utilities.RotateVectorCounterClockwise(projectileDirection, i * MathF.PI / projectilesPerWave)), 1f, Texture, 0);


                }
            }

            if (Main.player.Position.Y < Position.Y) //have fun cheesing this one sean
            {
                BasicProjectile projectile = new BasicProjectile();
                projectile.Spawn(Position, 5f * Utilities.SafeNormalise(Main.player.Position - Position, Vector2.Zero), 1f, Texture, 1.03f);
                Velocity = Velocity * 1.01f;


            }

            if (AITimer == 750)
            {
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }

            HandleBounces();
        }

        public void MoveTowardsAndShotgun(ref float AITimer, ref int AttackNumber)
        {


            if (AITimer == 510)
            {
                ShotgunFrequency = 10f;
            }

            if (AITimer % ShotgunFrequency == 0 && (AITimer < 410 || AITimer > 509) && AITimer > 60)
            {

                BasicProjectile singleShot = new BasicProjectile();
                float numberOfProjectiles = 3;
                float projectileSpeed = 13f;

                Velocity = 1.1f * Utilities.Normalise(Position - Main.player.Position);

                singleShot.Spawn(Position, projectileSpeed * Utilities.Normalise(Main.player.Position - Position), 1f, Texture, 0);

                for (int i = 1; i < (numberOfProjectiles / 2) + 0.5f; i++) // loop for each pair of projectiles an angle away from the middle
                {
                    BasicProjectile shotgunBlast = new BasicProjectile();
                    BasicProjectile shotgunBlast2 = new BasicProjectile(); //one for each side of middle
                    shotgunBlast.Spawn(Position, projectileSpeed * Utilities.Normalise(Utilities.RotateVectorClockwise(Main.player.Position - Position, i * MathF.PI / 13)), 1f, Texture, 0);
                    shotgunBlast2.Spawn(Position, projectileSpeed * Utilities.Normalise(Utilities.RotateVectorCounterClockwise(Main.player.Position - Position, i * MathF.PI / 12)), 1f, Texture, 0);

                }


            }

            if ((AITimer > 410 && AITimer < 509) || AITimer < 60)
            {
                Velocity = 5f * Utilities.Normalise(Main.player.Position - Position);
            }

            if (AITimer == 800)
            {
                ShotgunFrequency = 15f;
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }

            HandleBounces();
        }

        public void HorizontalChargesWithProjectiles(ref float AITimer, ref int AttackNumber)
        {
            float screenFraction = 8f;
            float moveSpeed = 6f;

            if (AITimer % 400 == 0)
            {
                Position = new Vector2(Main._graphics.PreferredBackBufferWidth / screenFraction, Main._graphics.PreferredBackBufferHeight / screenFraction);
                Velocity = moveSpeed * Utilities.Normalise(new Vector2(Main._graphics.PreferredBackBufferWidth / screenFraction * 1.33f, Main._graphics.PreferredBackBufferHeight / screenFraction) - Position); // go to target position from current
            }

            if (AITimer % 400 == 200)
            {
                Position = new Vector2(Main._graphics.PreferredBackBufferWidth / screenFraction * 7f, Main._graphics.PreferredBackBufferHeight / screenFraction * 7f);
                Velocity = moveSpeed * Utilities.Normalise(new Vector2(Main._graphics.PreferredBackBufferWidth / screenFraction, Main._graphics.PreferredBackBufferHeight / screenFraction * 7f) - Position);
            }

            if (AITimer == 480)
            {
                ShotgunFrequency = 15f;
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }

            BasicProjectile projectile = new BasicProjectile();
            projectile.Spawn(Position, 2f * Utilities.Normalise(Main.player.Position - Position), 1f, Texture, 1.03f);

        }

        public void MutantBulletHell(ref float AITimer, ref int AttackNumber) //this is literqally spiral but with 100f instead of 1250f
        {
            if (AITimer == 0)
            {
                Position = new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2);
                Velocity = Vector2.Zero; //amke it sit in the middle
            }

            if (AITimer <= 240)
            {
                SpinUpClockwise(ref Rotation, 80);
            }

            List<BasicProjectile> projectilesToShoot = new List<BasicProjectile>();

            if (AITimer % 5 == 0 && AITimer > 240 && AITimer < 950)
            {
                int projectilesInSpiral = 10;
                float rotation = (AITimer / 10 * MathF.PI / 40f) * (AITimer / 100f);

                Rotation = rotation;

                for (int i = 0; i < projectilesInSpiral; i++)
                {
                    projectilesToShoot.Add(new BasicProjectile()); //add a projectile

                    // shoot projectiles in a ring and rotate it based on time
                    Vector2 velocity = 5.5f * Utilities.SafeNormalise(Utilities.RotateVectorCounterClockwise(new Vector2(0, -1), Utilities.ToRadians(i * 45) + rotation), Vector2.Zero);

                    projectilesToShoot[i].Spawn(Position, velocity, 1f, Texture, 1);
                }
            }

            if (AITimer == 951)
            {
                Rotation = 0;
            }

            if (AITimer == 1150)
            {
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }
        }
        public void HandleBounces()
        {
            if (touchingLeft(this))
            {
                if (Velocity.X < 0)
                    Velocity.X = Velocity.X * -1;
            }

            if (touchingRight(this, Main._graphics.PreferredBackBufferWidth))
            {
                if (Velocity.X > 0)
                    Velocity.X = Velocity.X * -1;
            }

            if (touchingTop(this))
            {
                if (Velocity.Y < 0)
                    Velocity.Y = Velocity.Y * -1f;

            }

            if (touchingBottom(this, Main._graphics.PreferredBackBufferHeight))
            {
                if (Velocity.Y > 0)
                    Velocity.Y = Velocity.Y * -1f;
            }
        }

        public void EndAttack(ref float AITimer, ref int AttackNumber)
        {
            AITimer = -1; //to prevent jank with EndAttack taking a frame, allows attacks to start on 0
            Rotation = 0;
            AttackNumber++;
        }

        public void SpinUpClockwise(ref float rotation, float accel) //as accel parameter increases, the actual accel decreases
        {
            rotation = Rotation + (MathF.PI / 90) * AITimer / 80f; //spin up
        }

        public void SpinUpCounterClockwise(ref float rotation, float accel) //as accel parameter increases, the actual accel decreases
        {
            rotation = Rotation - (MathF.PI / 90) * AITimer / 80f; //spin up
        }
    }
}