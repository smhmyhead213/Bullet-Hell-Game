using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.UtilitySystems.Dialogue;

namespace bullethellwhatever.Bosses
{
    public class TestBoss : Boss
    {
        public bool HasChosenChargeDirection;
        public float SpiralStartTime;
        public int AttackNumber; //position in pattern
        public float ShotgunFrequency;
        public float DespBombFrequencyInitially;
        public float DespBombFrequency;
        public bool HasDesperationStarted;
        public float DespBombTimer;
        public int DespBombCounter;



        public TestBoss(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
            isBoss = true;
            isPlayer = false;
            Health = 1;
            AITimer = 0f;
            IFrames = 5f;
            HasChosenChargeDirection = false;
            AttackNumber = 1;
            ShotgunFrequency = 15f;
            DespBombFrequencyInitially = 30f;
            DespBombFrequency = DespBombFrequencyInitially;
            DespBombTimer = 0f;
            DespBombCounter = 0;
            HasDesperationStarted = false;
            IsDesperationOver = false;
            dialogueSystem = new DialogueSystem(this);
        }

        public override bool ShouldRemoveOnEdgeTouch() => false;


        public override void Spawn(Vector2 position, Vector2 initialVelocity, float damage, Texture2D texture, Vector2 size, float MaxHealth)
        {
            base.Spawn(position, initialVelocity, damage, texture, size, MaxHealth);

        }

        public override void AI()
        {

            CheckForAndTakeDamage();

            if (Health < 0 && IsDesperationOver)
            {
                DeleteNextFrame = true;
                foreach (Projectile projectile in Main.activeProjectiles)
                {
                    if (projectile.Owner == this)
                    {
                        projectile.DeleteNextFrame = true;
                    }
                }
            }



                //Update the boss position based on its velocity.
                Position = Position + Velocity;


                if (IFrames > 0)
                {
                    IFrames--;
                }

                //If the timer reaches 10, execute the attack and reset timer.
                switch (GameState.Difficulty)
                {
                    case GameState.Difficulties.Easy:
                        ExecuteEasyAttackPattern();
                        break;
                    case GameState.Difficulties.Normal:
                        ExecuteNormalAttackPattern();
                        break;
                    case GameState.Difficulties.Hard:
                        ExecuteHardAttackPattern();
                        break;
                    case GameState.Difficulties.Insane:
                        ExecuteInsaneAttackPattern();
                        break;

                }


                //Every frame, add 1 to the timer.
                AITimer++;


            }

            public void ExecuteEasyAttackPattern()
            {
                if (Health <= 0)
                {
                    IsDesperationOver = true;
                }


                switch (AttackNumber)
                {
                    case 1:
                        BasicShotgunBlast(ref AITimer, ref AttackNumber, Position, 5f, 14);
                        break;
                    case 2:
                        Charge(ref AITimer, ref AttackNumber, 3f, 180f, 1f);
                        break;
                    case 3:
                        Spiral(ref AITimer, ref AttackNumber, 4, 70f);
                        break;
                    case 4: //ObnoxiouslyDenseBulletHell(ref AITimer, ref AttackNumber, 25);
                        MoveTowardsAndShotgun(ref AITimer, ref AttackNumber, 1, 9f);
                        break;
                    case 5:
                        ExplodingProjectiles(ref AITimer, ref AttackNumber, 10f);
                        break;
                    case 6:
                        MutantBulletHell(ref AITimer, ref AttackNumber, 6);
                        break;
                    case 7:
                        if (AITimer % 2 == 0)
                            HorizontalChargesWithProjectiles(ref AITimer, ref AttackNumber, 3f);
                        break;
                    default:
                        AttackNumber = 1;
                        break;

                }
            }
            public void ExecuteNormalAttackPattern()
            {
                if (Health <= 0)
                {
                    IsDesperationOver = true;
                }

                switch (AttackNumber)
                {
                    case 1:
                        BasicShotgunBlast(ref AITimer, ref AttackNumber, Position, 5f, 20);
                        break;
                    case 2:
                        Charge(ref AITimer, ref AttackNumber, 5f, 165f, 1.01f);
                        break;
                    case 3:
                        Spiral(ref AITimer, ref AttackNumber, 6, 60f);
                        break;
                    case 4: //ObnoxiouslyDenseBulletHell(ref AITimer, ref AttackNumber, 25);
                        MoveTowardsAndShotgun(ref AITimer, ref AttackNumber, 3, 11f);
                        break;
                    case 5:
                        ExplodingProjectiles(ref AITimer, ref AttackNumber, 15f);
                        break;
                    case 6:
                        MutantBulletHell(ref AITimer, ref AttackNumber, 8);
                        break;
                    case 7:
                        if (AITimer % 2 == 0)
                            HorizontalChargesWithProjectiles(ref AITimer, ref AttackNumber, 5f);
                        break;
                    default:
                        AttackNumber = 1;
                        break;

                }
            }
            public void ExecuteHardAttackPattern()
            {
                if (Health <= 0 && !HasDesperationStarted)
                {
                    AttackNumber = 0; //desperation
                    AITimer = 0f;
                    HasDesperationStarted = true;
                }
                switch (AttackNumber)
                {
                    case 1:
                        BasicShotgunBlast(ref AITimer, ref AttackNumber, Position, 5f, 24);
                        break;
                    case 2:
                        Charge(ref AITimer, ref AttackNumber, 7f, 150f, 1.035f);
                        break;
                    case 3:
                        Spiral(ref AITimer, ref AttackNumber, 8, 60f);
                        break;
                    case 4: //ObnoxiouslyDenseBulletHell(ref AITimer, ref AttackNumber, 25);
                        MoveTowardsAndShotgun(ref AITimer, ref AttackNumber, 3, 13f);
                        break;
                    case 5:
                        ExplodingProjectiles(ref AITimer, ref AttackNumber, 20f);
                        break;
                    case 6:
                        MutantBulletHell(ref AITimer, ref AttackNumber, 10);
                        break;
                    case 7:
                        if (AITimer % 2 == 0)
                            HorizontalChargesWithProjectiles(ref AITimer, ref AttackNumber, 6.5f);
                        break;
                    case 0:
                        Desperation(ref AITimer, ref DespBombTimer, ref DespBombFrequency, DespBombFrequencyInitially, 9);
                        break;
                    default:
                        AttackNumber = 1;
                        break;

                }
            }

            public void ExecuteInsaneAttackPattern()
            {
                if (Health <= 0 && !HasDesperationStarted)
                {
                    AttackNumber = 0; //desperation
                    AITimer = 0f;
                    HasDesperationStarted = true;
                }

                switch (AttackNumber)
                {
                    case 1:
                        BasicShotgunBlast(ref AITimer, ref AttackNumber, Position, 5f, 30);
                        break;
                    case 2:
                        Charge(ref AITimer, ref AttackNumber, 9f, 140f, 1.04f);
                        break;
                    case 3:
                        Spiral(ref AITimer, ref AttackNumber, 10, 45f);
                        break;
                    case 4: //ObnoxiouslyDenseBulletHell(ref AITimer, ref AttackNumber, 25);
                        MoveTowardsAndShotgun(ref AITimer, ref AttackNumber, 5, 15f);
                        break;
                    case 5:
                        ExplodingProjectiles(ref AITimer, ref AttackNumber, 25f);
                        break;
                    case 6:
                        MutantBulletHell(ref AITimer, ref AttackNumber, 12);
                        break;
                    case 7:
                        HorizontalChargesWithProjectiles(ref AITimer, ref AttackNumber, 8f);
                        break;
                    case 0:
                        Desperation(ref AITimer, ref DespBombTimer, ref DespBombFrequency, DespBombFrequencyInitially, 12);
                        break;
                    default:
                        AttackNumber = 1;
                        break;

                }
            }
            public void BasicShotgunBlast(ref float AITimer, ref int AttackNumber, Vector2 bossPosition, float projectileSpeed, int numberOfProjectiles)
            {
                float projectileOscillationFrequency = 10f;

                if (AITimer % 40 == 0 && AITimer < 600)
                {
                    OscillatingSpeedProjectile singleShot = new OscillatingSpeedProjectile(projectileOscillationFrequency, projectileSpeed);

                    singleShot.Spawn(bossPosition, projectileSpeed * Utilities.Normalise(Main.player.Position - Position), 1f, Texture, 0, Vector2.One, this);

                    for (int i = 1; i < numberOfProjectiles / 2 + 0.5f; i++) // loop for each pair of projectiles an angle away from the middle
                    {
                        OscillatingSpeedProjectile oscillatingSpeedProjectile = new OscillatingSpeedProjectile(projectileOscillationFrequency, projectileSpeed);
                        OscillatingSpeedProjectile oscillatingSpeedProjectile2 = new OscillatingSpeedProjectile(projectileOscillationFrequency, projectileSpeed); //one for each side of middle

                        oscillatingSpeedProjectile.Spawn(bossPosition, Utilities.Normalise(Utilities.RotateVectorClockwise(Main.player.Position - bossPosition, i * MathF.PI / 12)), 1f, Texture, 1.01f, Vector2.One, this);
                        oscillatingSpeedProjectile2.Spawn(bossPosition, Utilities.Normalise(Utilities.RotateVectorCounterClockwise(Main.player.Position - bossPosition, i * MathF.PI / 12)), 1f, Texture, 1.01f, Vector2.One, this);

                    }
                }

                if (AITimer == 800)
                {
                    foreach (Projectile projectile in Main.activeProjectiles)
                    {
                        projectile.Velocity = projectile.Velocity * 2;
                    }

                    EndAttack(ref AITimer, ref AttackNumber);
                    return;
                }

                HandleBounces();

            }

            public void Charge(ref float AITimer, ref int AttackNumber, float chargeSpeed, float chargeFrequency, float chargeProjectileAcceleration)
            {
                if (AITimer == 0)
                {
                    ContactDamage = true; //turn on contact damage
                }

                if (!HasChosenChargeDirection)
                {
                    Velocity = chargeSpeed * Utilities.Normalise(Main.player.Position - Position);
                    HasChosenChargeDirection = true; //charge
                }

                //maths really is beautiful
                chargeSpeed = chargeSpeed * (MathF.Cos(AITimer % chargeFrequency / (chargeFrequency / 2)) + 0.5f); //the velocity follows a sine curve, so the acceleration follows its derived graph, cos x

                Velocity = chargeSpeed * Utilities.SafeNormalise(Velocity, Vector2.Zero);


                SpinUpClockwise(ref Rotation, 20f);

                if (AITimer % chargeFrequency == 0)
                {
                    HasChosenChargeDirection = false; //enable the next charge to start
                }

                if (!(AITimer % chargeFrequency > 0 && AITimer % chargeFrequency < chargeFrequency / 2 + 30f) && AITimer % 30 % 2 == 0)// check if aitimer is between 1 and 15 and if its even
                {
                    BasicProjectile projectile = new BasicProjectile();
                    projectile.Spawn(Position, 5f * Utilities.Normalise(Main.player.Position - Position), 1f, Texture, chargeProjectileAcceleration, Vector2.One, this);
                }

                if (AITimer == chargeFrequency * 6)
                {
                    EndAttack(ref AITimer, ref AttackNumber);
                    ContactDamage = false;
                    return;
                }


                HandleBounces();
            }

            public void Spiral(ref float AITimer, ref int AttackNumber, int projectilesInSpiral, float rotationSpeed) //rotation speed 40 by default, increase to make easier
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

                    float acceleration = 0.52f * MathF.Cos(AITimer / 250 + MathF.PI / 3);
                    float rotation = AITimer / 15 * MathF.PI / rotationSpeed * acceleration;

                    Rotation = rotation;

                    for (int i = 0; i < projectilesInSpiral; i++)
                    {
                        projectilesToShoot.Add(new BasicProjectile()); //add a projectile

                        // shoot projectiles in a ring and rotate it based on time
                        Vector2 velocity = 7f * Utilities.SafeNormalise(Utilities.RotateVectorCounterClockwise(new Vector2(0, -1), Utilities.ToRadians(i * 360 / projectilesInSpiral) + rotation), Vector2.Zero);

                        projectilesToShoot[i].Spawn(Position, velocity, 1f, Texture, 1, Vector2.One, this);


                    }


                }

                if (AITimer == 1800)
                {
                    EndAttack(ref AITimer, ref AttackNumber);
                    return;
                }

            }

            public void ObnoxiouslyDenseBulletHell(ref float AITimer, ref int AttackNumber, int projectilesPerWave)
            {
                if (AITimer == 0)
                {
                    Position = new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 18);
                    Velocity = new Vector2(-1.5f, 0);
                }

                if (AITimer % 30 == 0 && AITimer < 500)
                {
                    List<BasicProjectile> projectilesToShoot = new List<BasicProjectile>();

                    Random rnd = new Random();

                    Vector2 projectileDirection = new Vector2(1, 0);
                    for (int i = 0; i < projectilesPerWave; i++)
                    {
                        projectilesToShoot.Add(new BasicProjectile());

                        //Shoot projectiles based on an offset.
                        projectilesToShoot[i].Spawn(Position, 5f * Vector2.Normalize(Utilities.RotateVectorCounterClockwise(projectileDirection, i * MathF.PI / projectilesPerWave)), 1f, Texture, 0, Vector2.One, this);


                    }
                }

                if (Main.player.Position.Y < Position.Y) //have fun cheesing this one sean
                {
                    BasicProjectile projectile = new BasicProjectile();
                    projectile.Spawn(Position, 5f * Utilities.SafeNormalise(Main.player.Position - Position, Vector2.Zero), 1f, Texture, 1.03f, Vector2.One, this);
                    Velocity = Velocity * 1.01f;


                }

                if (AITimer == 750)
                {
                    EndAttack(ref AITimer, ref AttackNumber);
                    return;
                }

                HandleBounces();
            }

            public void MoveTowardsAndShotgun(ref float AITimer, ref int AttackNumber, float numberOfProjectiles, float projectileSpeed)
            {
                dialogueSystem.Dialogue(Position, "Test dialogue", 4, 800);

                if (AITimer == 510)
                {
                    ShotgunFrequency = 10f;
                }

                if (AITimer % ShotgunFrequency == 0 && (AITimer < 410 || AITimer > 509) && AITimer > 60)
                {


                    BasicProjectile singleShot = new BasicProjectile();


                    Velocity = 1.1f * Utilities.Normalise(Position - Main.player.Position);

                    singleShot.Spawn(Position, projectileSpeed * Utilities.Normalise(Main.player.Position - Position), 1f, Texture, 1.01f, Vector2.One, this);

                    for (int i = 1; i < numberOfProjectiles / 2 + 0.5f; i++) // loop for each pair of projectiles an angle away from the middle
                    {
                        BasicProjectile shotgunBlast = new BasicProjectile();
                        BasicProjectile shotgunBlast2 = new BasicProjectile(); //one for each side of middle
                        shotgunBlast.Spawn(Position, projectileSpeed * Utilities.Normalise(Utilities.RotateVectorClockwise(Main.player.Position - Position, i * MathF.PI / 13)), 1f, Texture, 1.01f, Vector2.One, this);
                        shotgunBlast2.Spawn(Position, projectileSpeed * Utilities.Normalise(Utilities.RotateVectorCounterClockwise(Main.player.Position - Position, i * MathF.PI / 12)), 1f, Texture, 1.01f, Vector2.One, this);

                    }


                }

                if (AITimer > 410 && AITimer < 509)
                {
                    Velocity = 5f * Utilities.Normalise(Main.player.Position - Position);
                }

                if (AITimer == 800)
                {
                    ShotgunFrequency = 15f;
                    dialogueSystem.ClearDialogue();
                    EndAttack(ref AITimer, ref AttackNumber);
                    return;
                }

                HandleBounces();
            }

            public void HorizontalChargesWithProjectiles(ref float AITimer, ref int AttackNumber, float moveSpeed)
            {
                float screenFraction = 8f;


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
                projectile.Spawn(Position, 2f * Utilities.Normalise(Main.player.Position - Position), 1f, Texture, 1.03f, Vector2.One, this);

            }

            public void MutantBulletHell(ref float AITimer, ref int AttackNumber, int projectilesInSpiral) //this is literqally spiral but with 100f instead of 1250f
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
                    float rotation = AITimer / 10 * MathF.PI / 40f * (AITimer / 100f);

                    Rotation = rotation;

                    for (int i = 0; i < projectilesInSpiral; i++)
                    {
                        projectilesToShoot.Add(new BasicProjectile()); //add a projectile

                        // shoot projectiles in a ring and rotate it based on time
                        Vector2 velocity = 5.5f * Utilities.SafeNormalise(Utilities.RotateVectorCounterClockwise(new Vector2(0, -1), Utilities.ToRadians(i * 45) + rotation), Vector2.Zero);

                        projectilesToShoot[i].Spawn(Position, velocity, 1f, Texture, 1, Vector2.One, this);
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

            public void ExplodingProjectiles(ref float AITimer, ref int AttackNumber, float numberOfProjectiles)
            {
                Velocity = 1.1f * Utilities.Normalise(Main.player.Position - Position);

                if (AITimer % 30 == 0 && AITimer < 590 && AITimer > 240)
                {
                    ExplodingProjectile explodingProjectile = new ExplodingProjectile(numberOfProjectiles, 120f, 0, true, true, false);

                    Vector2 direction = Utilities.RotateVectorClockwise(Main.player.Position - Position, Utilities.ToRadians(AITimer - 250f));



                    explodingProjectile.Spawn(Position, 5f * Utilities.SafeNormalise(direction, Vector2.Zero), 1f, Texture, 0, new Vector2(2, 2), this);
                }

                Rotation = Utilities.ToRadians(AITimer - 250f);

                if (AITimer == 990)
                {
                    EndAttack(ref AITimer, ref AttackNumber);
                    return;
                }
            }

            public void Desperation(ref float AITimer, ref float despBombTimer, ref float despBombFrequency, float despBombFrequencyInitially, int projectilesPerBomb)
            {
                Random random = new Random();

                int despStartTime = 400;

                Deathray deathray = new Deathray();

                if (AITimer == 0)
                {
                    Position = new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2);
                    Velocity = Vector2.Zero; //amke it sit in the middle
                    Rotation = 0;

                }

                if (AITimer < despStartTime)
                {


                    dialogueSystem.Dialogue(Position, "It's not over yet!", 4, despStartTime);

                    Drawing.ScreenShake(5, 3000);

                }

                if (AITimer == despStartTime)
                {
                    int directionToRotate = Position.X > Main.player.Position.X ? 1 : -1;

                    deathray.Spawn(Position, MathF.PI, 1f, Texture, 40f, 1500f, directionToRotate * 40f, 0f, this);
                }

                if (AITimer > despStartTime)
                {
                    Drawing.ScreenShake(4, 3000);

                    float bombRotation = MathF.PI / 9 * (despBombFrequencyInitially - DespBombCounter);

                    despBombTimer++;

                    if (despBombTimer >= despBombFrequency)
                    {
                        ExplodingProjectile explodingProjectile1 = new ExplodingProjectile(projectilesPerBomb, 60, MathF.PI / (((float)random.NextDouble() + 0.5f) * 30f), false, false, false);
                        ExplodingProjectile explodingProjectile2 = new ExplodingProjectile(projectilesPerBomb, 60, MathF.PI / (((float)random.NextDouble() + 0.5f) * 30f), false, false, false);

                        explodingProjectile1.Spawn(Position, 3f * Utilities.RotateVectorClockwise(Vector2.UnitY, bombRotation), 1f, Texture, 1f, new Vector2(1.3f, 1.3f), this);
                        explodingProjectile2.Spawn(Position, 3f * Utilities.RotateVectorClockwise(Vector2.UnitY, MathF.PI + bombRotation), 1f, Texture, 1f, new Vector2(1.3f, 1.3f), this);



                        DespBombCounter++;

                        if (despBombFrequency < 5f)
                            despBombFrequency++;



                        despBombTimer = 0;
                    }
                    if (GameState.Difficulty == GameState.Difficulties.Insane)
                    {
                        if (AITimer > despStartTime & AITimer % 100 == 0)
                        {
                            BasicProjectile oscillatingSpeedProjectile = new BasicProjectile();

                            oscillatingSpeedProjectile.Spawn(Position, 2f * Utilities.SafeNormalise(Main.player.Position - Position, Vector2.Zero), 1f, Texture, 1.01f, new Vector2(0.9f, 0.9f), this);
                        }
                    }
                }

                if (AITimer == 3000)
                {
                    IsDesperationOver = true; //die
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
                rotation = Rotation + MathF.PI / 90 * AITimer / 80f; //spin up
            }

            public void SpinUpCounterClockwise(ref float rotation, float accel) //as accel parameter increases, the actual accel decreases
            {
                rotation = Rotation - MathF.PI / 90 * AITimer / 80f; //spin up
            }
        }
    }

