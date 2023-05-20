using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.UtilitySystems.Dialogue;

using bullethellwhatever.DrawCode;
using bullethellwhatever.Projectiles.Enemy;

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
        public int DespBeamRotation;
        public int FramesPerMusicBeat;
        public int BeatsPerBar;
        public int BarDuration;
        public float FirstAttackTelegraphLineRotation;
        public int CurrentBeat;
        public bool JustStartedBeat;
        public TestBoss(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
            isBoss = true;
            isPlayer = false;
            Health = 200;
            AITimer = 0;
            IFrames = 5f;
            HasChosenChargeDirection = false;
            AttackNumber = 1;
            ShotgunFrequency = 15f;
            DespBombFrequencyInitially = 40f;
            DespBombFrequency = DespBombFrequencyInitially;
            DespBombTimer = 0f;
            DespBombCounter = 0;
            HasDesperationStarted = false;
            IsDesperationOver = false;
            IsHarmful = true;
            dialogueSystem = new DialogueSystem(this);
            dialogueSystem.dialogueObject = new DialogueObject(position, string.Empty, this, 1, 1);

            Main.musicSystem.SetMusic("TestBossMusic", true, 0.15f);

            FramesPerMusicBeat = 24;
            BeatsPerBar = 4;
            BarDuration = FramesPerMusicBeat * BeatsPerBar;
            // 24 frames per beat
        }
        public override void Spawn(Vector2 position, Vector2 initialVelocity, float damage, string texture, Vector2 size, float MaxHealth, Color colour, bool shouldRemoveOnEdgeTouch)
        {
            base.Spawn(position, initialVelocity, damage, texture, size, MaxHealth, colour, shouldRemoveOnEdgeTouch);

        }

        public override void AI()
        {
            CurrentBeat = (int)((((float)AITimer / BarDuration) - MathF.Floor(AITimer / BarDuration)) * 4) + 1;

            JustStartedBeat = AITimer % FramesPerMusicBeat == 0 ? true : false;

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
                    Charge(ref AITimer, ref AttackNumber, 5f, FramesPerMusicBeat * BeatsPerBar * 2, 1f, 6f);
                    break;
                case 3:
                    Spiral(ref AITimer, ref AttackNumber, 4, 70f);
                    break;
                case 4:
                    LaserBarrages(ref AITimer, ref AttackNumber, MathF.PI / 6.9f, 11, 8);
                    break;
                case 5: //ObnoxiouslyDenseBulletHell(ref AITimer, ref AttackNumber, 25);
                    MoveTowardsAndShotgun(ref AITimer, ref AttackNumber, 1, 9f);
                    break;
                case 6:
                    ExplodingProjectiles(ref AITimer, ref AttackNumber, 10);
                    break;
                case 7:
                    MutantBulletHell(ref AITimer, ref AttackNumber, 6);
                    break;
                case 8:
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
                    Charge(ref AITimer, ref AttackNumber, 7f, BarDuration * 2, 1.01f, 9f);
                    break;
                case 3:
                    Spiral(ref AITimer, ref AttackNumber, 6, 60f);
                    break;
                case 4:
                    LaserBarrages(ref AITimer, ref AttackNumber, MathF.PI / 7.9f, 9, 7);
                    break;
                case 5: //ObnoxiouslyDenseBulletHell(ref AITimer, ref AttackNumber, 25);
                    MoveTowardsAndShotgun(ref AITimer, ref AttackNumber, 3, 11f);
                    break;
                case 6:
                    ExplodingProjectiles(ref AITimer, ref AttackNumber, 15);
                    break;
                case 7:
                    MutantBulletHell(ref AITimer, ref AttackNumber, 8);
                    break;
                case 8:
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
                AITimer = 0;
                HasDesperationStarted = true;
            }
            switch (AttackNumber)
            {
                case 1:
                    //BasicShotgunBlast(ref AITimer, ref AttackNumber, Position, 5f, 24);
                    if (AITimer % 2 == 0)
                        HorizontalChargesWithProjectiles(ref AITimer, ref AttackNumber, 6.5f);
                    break;
                case 2:
                    Charge(ref AITimer, ref AttackNumber, 9f, BarDuration * 2, 1.035f, 13f);
                    break;
                case 3:
                    Spiral(ref AITimer, ref AttackNumber, 8, 60f);
                    break;
                case 4:
                    LaserBarrages(ref AITimer, ref AttackNumber, MathF.PI / 9.1f, 7, 7);
                    break;
                case 5: //ObnoxiouslyDenseBulletHell(ref AITimer, ref AttackNumber, 25);
                    MoveTowardsAndShotgun(ref AITimer, ref AttackNumber, 3, 13f);
                    break;
                case 6:
                    ExplodingProjectiles(ref AITimer, ref AttackNumber, 20);
                    break;
                case 7:
                    MutantBulletHell(ref AITimer, ref AttackNumber, 10);
                    break;
                case 8:
                    if (AITimer % 2 == 0)
                        HorizontalChargesWithProjectiles(ref AITimer, ref AttackNumber, 6.5f);
                    break;
                case 0:
                    Desperation(ref AITimer, ref DespBombTimer, ref DespBombFrequency, DespBombFrequencyInitially, 5, 2);
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
                AITimer = 0;
                HasDesperationStarted = true;
            }

            switch (AttackNumber)
            {
                case 1:
                    BasicShotgunBlast(ref AITimer, ref AttackNumber, Position, 5f, 30);
                    break;
                case 2:
                    Charge(ref AITimer, ref AttackNumber, 11f, BarDuration * 2, 1.04f, 15f);
                    break;
                case 3:
                    Spiral(ref AITimer, ref AttackNumber, 10, 45f);
                    break;
                case 4:
                    LaserBarrages(ref AITimer, ref AttackNumber, MathF.PI / 11.1f, 5, 8);
                    break;
                case 5: //ObnoxiouslyDenseBulletHell(ref AITimer, ref AttackNumber, 25);
                    MoveTowardsAndShotgun(ref AITimer, ref AttackNumber, 5, 15f);
                    break;
                case 6:
                    ExplodingProjectiles(ref AITimer, ref AttackNumber, 25);
                    break;
                case 7:
                    MutantBulletHell(ref AITimer, ref AttackNumber, 12);
                    break;
                case 8:
                    HorizontalChargesWithProjectiles(ref AITimer, ref AttackNumber, 8f);
                    break;
                case 0:
                    Desperation(ref AITimer, ref DespBombTimer, ref DespBombFrequency, DespBombFrequencyInitially, 6, 3);
                    break;
                default:
                    AttackNumber = 1;
                    break;

            }
        }
        public void BasicShotgunBlast(ref int AITimer, ref int AttackNumber, Vector2 bossPosition, float projectileSpeed, int numberOfProjectiles)
        {
            if (AITimer > 0 && AITimer < 5)
            {
                Velocity = new Vector2(2f, 0f);
                FirstAttackTelegraphLineRotation = -1f;
            }

            float angleBetweenShots = MathF.PI / numberOfProjectiles;

            float projectileOscillationFrequency = 10f;

            if (CurrentBeat == 2 && JustStartedBeat)
            {
                if (AITimer > BarDuration * 4)
                {
                    angleBetweenShots = angleBetweenShots * 2;
                }

                OscillatingSpeedProjectile singleShot = new OscillatingSpeedProjectile(projectileOscillationFrequency, projectileSpeed);

                singleShot.Spawn(bossPosition, projectileSpeed * Utilities.Normalise(Main.player.Position - Position), 1f, "box", 0, Vector2.One, this, true, Color.Red, true, false);

                for (int i = 1; i < numberOfProjectiles / 2 + 0.5f; i++) // loop for each pair of projectiles an angle away from the middle
                {
                    OscillatingSpeedProjectile oscillatingSpeedProjectile = new OscillatingSpeedProjectile(projectileOscillationFrequency, projectileSpeed);
                    OscillatingSpeedProjectile oscillatingSpeedProjectile2 = new OscillatingSpeedProjectile(projectileOscillationFrequency, projectileSpeed); //one for each side of middle

                    oscillatingSpeedProjectile.Spawn(bossPosition, Utilities.Normalise(Utilities.RotateVectorClockwise(Main.player.Position - bossPosition, i * angleBetweenShots)),
                        1f, "box", 1.01f, Vector2.One, this, true, Color.Red, true, false);
                    oscillatingSpeedProjectile2.Spawn(bossPosition, Utilities.Normalise(Utilities.RotateVectorCounterClockwise(Main.player.Position - bossPosition, i * angleBetweenShots)),
                        1f, "box", 1.01f, Vector2.One, this, true, Color.Red, true, false);
                }
            }

            

            if (AITimer >= BarDuration * 11 && AITimer <= BarDuration * 12)
            {
                float angle = Utilities.VectorToAngle(-Vector2.UnitY);

                if (AITimer == BarDuration * 11)
                {
                    TelegraphLine t = new TelegraphLine(angle, MathF.PI / 3360, -MathF.PI / (336 * 96), 30f, 2000f, BarDuration, Position, Colour, "box", this);
                }

                if (AITimer == BarDuration * 12)
                {
                    Deathray ray = new Deathray();

                    ray.SpawnDeathray(Position, angle, 1f, BarDuration * 3, "box", 30f, 2000f,
                        150f, 0f, true, Colour, "DeathrayShader2", this);
                }
            }

            if (AITimer > 300)
            {              
                if (CurrentBeat == 1 && JustStartedBeat)
                {
                    TelegraphLine telegraph = new TelegraphLine(FirstAttackTelegraphLineRotation, 0f, 0f, 20f, 2000f, FramesPerMusicBeat * 2, Position, Colour, "box", this);
                }

                if (CurrentBeat == 3 && JustStartedBeat)
                {
                    if (FirstAttackTelegraphLineRotation != -1f) //-1 is a flag for if the direction has not yet been decided
                    {
                        Deathray ray = new Deathray();
                        ray.SpawnDeathray(Position, FirstAttackTelegraphLineRotation, 1f, FramesPerMusicBeat * 2, "box", 20f, 2000f, 0f, 0f, true, Colour, "DeathrayShader", this);
                    }

                    //float predictionStrength = 500f;
                    FirstAttackTelegraphLineRotation = Utilities.VectorToAngle(Main.player.Position - Position) - MathHelper.PiOver2;

                }

            }

            if (AITimer == FramesPerMusicBeat * BeatsPerBar * 4)
            {
                Position = new Vector2(Main.ScreenWidth / 2, Main.ScreenHeight / 2);
                Velocity = Vector2.Zero;
            }

            if (AITimer == FramesPerMusicBeat * BeatsPerBar * 18)
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

        public void Charge(ref int AITimer, ref int AttackNumber, float chargeSpeed, float chargeFrequency, float chargeProjectileAcceleration, float chargeProjSpeed)
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

            if (!(AITimer % chargeFrequency > 0 && AITimer % chargeFrequency < chargeFrequency / 2 + 30f) && AITimer % 30 % 2 == 0 && AITimer > 0)// check if aitimer is between 1 and 15 and if its even
            {
                WeakHomingProjectile projectile = new WeakHomingProjectile(chargeProjSpeed, 120);
                projectile.Spawn(Position, Utilities.Normalise(Main.player.Position - Position), 1f, "box", chargeProjectileAcceleration, Vector2.One, this, true, Color.Red, true, false);

                //BasicProjectile projectile = new BasicProjectile();
                //projectile.Spawn(Position, 5f * Utilities.Normalise(Main.player.Position - Position), 1f, Texture, chargeProjectileAcceleration, Vector2.One, this, true, Color.Red, true, false);
            }

            if (AITimer == chargeFrequency * 8)
            {
                EndAttack(ref AITimer, ref AttackNumber);
                ContactDamage = false;
                return;
            }


            HandleBounces();
        }

        public void MoveToCentre(int AITImer, int duration)
        {
            Vector2 vectorToCentre = Utilities.CentreOfScreen() - Position;
            float distanceToTravel = vectorToCentre.Length();
            //Velocity = Utilities.SafeNormalise(vectorToCentre, Vector2.Zero) * distanceToTravel / (timeToStartAt - AITimer);

            // top 5 integration moments
            Velocity = Utilities.SafeNormalise(vectorToCentre, Vector2.Zero) * (2f * MathF.PI * distanceToTravel / duration) * MathF.Sin(MathF.PI * AITimer / duration);
        }
        public void Spiral(ref int AITimer, ref int AttackNumber, int projectilesInSpiral, float rotationSpeed) //rotation speed 40 by default, increase to make easier
        {
            List<BasicProjectile> projectilesToShoot = new List<BasicProjectile>();

            int timeToStartAt = BarDuration * 2;
            int endTime = timeToStartAt + BarDuration * 10;

            if (AITimer <= timeToStartAt)
            {
                MoveToCentre(AITimer, timeToStartAt);
                SpinUpCounterClockwise(ref Rotation, 60f);
            }

            if (AITimer == timeToStartAt)
            {
                Velocity = Vector2.Zero;
            }
            if (AITimer % 2 == 0 && AITimer > timeToStartAt && AITimer < endTime - BarDuration)
            {

                float acceleration = 0.52f * MathF.Cos(AITimer / 250f + MathF.PI);
                float rotation = AITimer / 15f * MathF.PI / rotationSpeed * acceleration;

                Rotation = rotation;

                for (int i = 0; i < projectilesInSpiral; i++)
                {
                    projectilesToShoot.Add(new BasicProjectile()); //add a projectile

                    // shoot projectiles in a ring and rotate it based on time
                    Vector2 velocity = 7f * Utilities.SafeNormalise(Utilities.RotateVectorClockwise(new Vector2(0, -1), Utilities.ToRadians(i * 360 / projectilesInSpiral) + rotation), Vector2.Zero);

                    projectilesToShoot[i].Spawn(Position, velocity, 1f, "box", 1, Vector2.One, this, true, Color.Red, true, false);


                }


            }

            if (AITimer == endTime)
            {
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }

        }

        public void ObnoxiouslyDenseBulletHell(ref int AITimer, ref int AttackNumber, int projectilesPerWave)
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
                    projectilesToShoot[i].Spawn(Position, 5f * Vector2.Normalize(Utilities.RotateVectorCounterClockwise(projectileDirection, i * MathF.PI / projectilesPerWave)),
                        1f, "box", 0, Vector2.One, this, true, Color.Red, true, false);


                }
            }

            if (Main.player.Position.Y < Position.Y) //have fun cheesing this one sean
            {
                BasicProjectile projectile = new BasicProjectile();
                projectile.Spawn(Position, 5f * Utilities.SafeNormalise(Main.player.Position - Position, Vector2.Zero), 1f, "box", 1.03f, Vector2.One, this, true, Color.Red, true, false);
                Velocity = Velocity * 1.01f;


            }

            if (AITimer == 750)
            {
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }

            HandleBounces();
        }

        public void MoveTowardsAndShotgun(ref int AITimer, ref int AttackNumber, float numberOfProjectiles, float projectileSpeed)
        {
            float angleBetweenProjectiles = MathF.PI / 12;

            if (AITimer == 0) //this could be optimised by giving every entity a rotation from vertical field
            {
                dialogueSystem.Dialogue(Position, "Test dialogue", 4, 800);
            }

            if (AITimer == 510)
            {
                ShotgunFrequency = 10f;
                dialogueSystem.ClearDialogue();
            }

            if (AITimer == 60)
            {
                foreach (TelegraphLine telegraphLine in activeTelegraphs)
                {
                    telegraphLine.RotationalVelocity = 0;
                }
            }

            if (AITimer % ShotgunFrequency == 0 && (AITimer < BarDuration * 4 || AITimer > BarDuration * 5) && AITimer > BarDuration)
            {

                BasicProjectile singleShot = new BasicProjectile();


                Velocity = 1.1f * Utilities.Normalise(Position - Main.player.Position);

                singleShot.Spawn(Position, projectileSpeed * Utilities.Normalise(Main.player.Position - Position), 1f, "box", 1.01f, Vector2.One, this, true, Color.Red, true, false);

                for (int i = 1; i < numberOfProjectiles / 2 + 0.5f; i++) // loop for each pair of projectiles an angle away from the middle
                {
                    BasicProjectile shotgunBlast = new BasicProjectile();
                    BasicProjectile shotgunBlast2 = new BasicProjectile(); //one for each side of middle
                    shotgunBlast.Spawn(Position, projectileSpeed * Utilities.Normalise(Utilities.RotateVectorClockwise(Main.player.Position - Position, i * angleBetweenProjectiles)),
                        1f, "box", 1.01f, Vector2.One, this, true, Color.Red, true, false);
                    shotgunBlast2.Spawn(Position, projectileSpeed * Utilities.Normalise(Utilities.RotateVectorCounterClockwise(Main.player.Position - Position, i * angleBetweenProjectiles)),
                        1f, "box", 1.01f, Vector2.One, this, true, Color.Red, true, false);

                }


            }

            if (AITimer > BarDuration * 4 && AITimer < BarDuration * 5)
            {
                Velocity = MathHelper.Lerp(1f, 5f, Utilities.DistanceBetweenEntities(Main.player, this) / 1000f) * Utilities.Normalise(Main.player.Position - Position);
            }

            if (AITimer == BarDuration * 7)
            {
                ShotgunFrequency = 15f;
                dialogueSystem.ClearDialogue();
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }

            HandleBounces();
        }

        public void LaserBarrages(ref int AITimer, ref int AttackNumber, float angleBetween, int timeBetweenRays, int numberOfRaysBetweenTelegraphAndBeam)
        {
            float endTime = BarDuration * 6;

            if (AITimer == 0)
            {
                Position = new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2);
                Velocity = Vector2.Zero;
            }

            if (AITimer % timeBetweenRays == 0)
            {
                if (AITimer < endTime - (timeBetweenRays * numberOfRaysBetweenTelegraphAndBeam))
                {
                    TelegraphLine TeleLine = new TelegraphLine(angleBetween * AITimer / timeBetweenRays, 0f, 0f,20, 2000, timeBetweenRays * numberOfRaysBetweenTelegraphAndBeam, Position, Color.White, "box", this);
                }

                if (AITimer > timeBetweenRays * numberOfRaysBetweenTelegraphAndBeam)
                {
                    Deathray ray = new Deathray();
                    ray.SpawnDeathray(Position, angleBetween * (AITimer - timeBetweenRays * numberOfRaysBetweenTelegraphAndBeam) / timeBetweenRays, 1f, 50, "box", 30, 2000, 0, 0, true, Color.Red, "DeathrayShader", this);
                }
            }

            if (AITimer == endTime)
            {
                dialogueSystem.ClearDialogue();
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }
        }
        public void HorizontalChargesWithProjectiles(ref int AITimer, ref int AttackNumber, float moveSpeed)
        {
            float screenFraction = 8f;


            if (AITimer % (BarDuration * 4) == 0)
            {
                Position = new Vector2(Main._graphics.PreferredBackBufferWidth / screenFraction, Main._graphics.PreferredBackBufferHeight / screenFraction);
                Velocity = moveSpeed * Utilities.Normalise(new Vector2(Main._graphics.PreferredBackBufferWidth / screenFraction * 1.33f, Main._graphics.PreferredBackBufferHeight / screenFraction) - Position); // go to target position from current
            }

            if (AITimer % (BarDuration * 4) == BarDuration * 2)
            {
                Position = new Vector2(Main._graphics.PreferredBackBufferWidth / screenFraction * 7f, Main._graphics.PreferredBackBufferHeight / screenFraction * 7f);
                Velocity = moveSpeed * Utilities.Normalise(new Vector2(Main._graphics.PreferredBackBufferWidth / screenFraction, Main._graphics.PreferredBackBufferHeight / screenFraction * 7f) - Position);
            }

            if (AITimer == BarDuration * 5)
            {
                ShotgunFrequency = 15f;
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }

            SizeChangingProjectile projectile = new SizeChangingProjectile(0.011f, 0.014f);
            projectile.Spawn(Position, 2f * Utilities.Normalise(Main.player.Position - Position), 1f, "box", 1.03f, new Vector2(0.6f, 0.6f), this, true, Color.Red, true, false);

        }

        public void MutantBulletHell(ref int AITimer, ref int AttackNumber, int projectilesInSpiral) //this is literqally spiral but with 100f instead of 1250f
        {
            if (AITimer <= BarDuration * 3)
            {
                MoveToCentre(AITimer, BarDuration * 3);
                SpinUpClockwise(ref Rotation, 80);
            }

            List<BasicProjectile> projectilesToShoot = new List<BasicProjectile>();

            if (AITimer % 5 == 0 && AITimer > BarDuration * 3 && AITimer < BarDuration * 10)
            {
                float rotation = AITimer / 10f * MathF.PI / 40f * (AITimer / 100f);

                Rotation = rotation;

                for (int i = 0; i < projectilesInSpiral; i++)
                {
                    projectilesToShoot.Add(new BasicProjectile()); //add a projectile

                    // shoot projectiles in a ring and rotate it based on time
                    Vector2 velocity = 5.5f * Utilities.SafeNormalise(Utilities.RotateVectorClockwise(new Vector2(0, -1), Utilities.ToRadians(i * 360 / projectilesInSpiral) + rotation), Vector2.Zero);

                    projectilesToShoot[i].Spawn(Position, velocity, 1f, "box", 1, Vector2.One, this, true, Color.Red, true, false);
                }
            }

            if (AITimer == BarDuration * 10)
            {
                Rotation = 0;
            }

            if (AITimer == BarDuration * 12)
            {
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }
        }

        public void ExplodingProjectiles(ref int AITimer, ref int AttackNumber, int numberOfProjectiles)
        {
            Velocity = 0.55f * Utilities.Normalise(Main.player.Position - Position);

            int startTime = 270;
            int endTime = 720;
            int timeBetween = BarDuration / 2;

            int explosionDelay = (int)(endTime - AITimer);

            if (AITimer % timeBetween == 0 && AITimer <= endTime && AITimer >= startTime)
            {
                ExplodingDeathrayProjectile explodingProjectile = new ExplodingDeathrayProjectile(numberOfProjectiles, 180, 0, true, true, false);

                Vector2 direction = Utilities.RotateVectorClockwise(Main.player.Position - Position, Utilities.ToRadians(AITimer - 250f));

                explodingProjectile.Spawn(Position, 15f * Utilities.SafeNormalise(direction, Vector2.Zero), 1f, "box", 0, new Vector2(2, 2), this, true, Color.Red, false, false);
            }

            Rotation = Utilities.ToRadians(AITimer - 250f);

            if (AITimer == BarDuration * 10)
            {
                EndAttack(ref AITimer, ref AttackNumber);
                return;
            }
        }

        public void Desperation(ref int AITimer, ref float despBombTimer, ref float despBombFrequency, float despBombFrequencyInitially, int projectilesPerBomb, int blenderBeams)
        {
            Random random = new Random();

            int despStartTime = 400;
            int despEndTime = 3000;

            int despTime = despEndTime - despStartTime;
            int despTimePassed = despTime - AITimer;
            
            if (AITimer == 0)
            {
                Velocity = Vector2.Zero; //amke it sit in the middle
                Rotation = 0;
                dialogueSystem.Dialogue(Position, "It's not over yet!", 4, despStartTime);
                Drawing.ScreenShake(4, despEndTime);

                for (int i = 0; i < blenderBeams; i++)
                {
                    TelegraphLine telegraphLine = new TelegraphLine(MathHelper.TwoPi / blenderBeams * i, 0, 0 , 15, 1500f, despStartTime, Position, Color.White, "box", this);
                }
            }

            if (AITimer < despStartTime)
            {
                MoveToCentre(AITimer, despStartTime);
            }

            if (AITimer == despStartTime)
            {
                DespBeamRotation = Position.X > Main.player.Position.X ? 1 : -1;
                Main.activeProjectiles.Clear();

                for (int i = 0; i < blenderBeams; i++)
                {
                    Deathray ray = new Deathray();
                    ray.SpawnDeathray(Position, MathHelper.TwoPi / blenderBeams * i, 1f, 2600, "box", 40f, 1500f, DespBeamRotation * 40f, 0f, true, Color.Red, "DeathrayShader2", this);
                }
                
                dialogueSystem.ClearDialogue();
            }

            if (AITimer > despStartTime)
            {
                Drawing.ScreenShake(3, despEndTime);

                float bombRotation = -DespBeamRotation * MathF.PI / 9 * (despBombFrequencyInitially - DespBombCounter);

                despBombTimer++;

                if (despBombTimer >= despBombFrequency)
                {
                    ExplodingProjectile explodingProjectile1 = new ExplodingProjectile(projectilesPerBomb, 60, MathF.PI / (((float)random.NextDouble() + 0.5f) * 30f), false, false, false);
                    ExplodingProjectile explodingProjectile2 = new ExplodingProjectile(projectilesPerBomb, 60, MathF.PI / (((float)random.NextDouble() + 0.5f) * 30f), false, false, false);
                    TelegraphLine telegraphLine1 = new TelegraphLine(bombRotation, 0, 0, 10, 1000, 60, Position, Color.White, "box", this);
                    TelegraphLine telegraphLine2 = new TelegraphLine(MathF.PI + bombRotation, 0, 0, 10, 1000, 60, Position, Color.White, "box", this);

                    explodingProjectile1.Spawn(Position, 3f * Utilities.RotateVectorClockwise(Vector2.UnitY, bombRotation), 1f, "box", 1.02f, new Vector2(1.3f, 1.3f), this, true, Color.Red, false, false);
                    explodingProjectile2.Spawn(Position, 10f * Utilities.RotateVectorClockwise(Vector2.UnitY, MathF.PI + bombRotation), 1f, "box", 1.02f, new Vector2(1.3f, 1.3f), this, true, Color.Red, false, false);



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

                        oscillatingSpeedProjectile.Spawn(Position, 2f * Utilities.SafeNormalise(Main.player.Position - Position, Vector2.Zero), 1f, "box", 1.01f, new Vector2(0.9f, 0.9f), this, true, Color.Red, true, false);
                    }
                }
            }

            if (AITimer == despEndTime)
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

            if (touchingRight(this))
            {
                if (Velocity.X > 0)
                    Velocity.X = Velocity.X * -1;
            }

            if (touchingTop(this))
            {
                if (Velocity.Y < 0)
                    Velocity.Y = Velocity.Y * -1f;

            }

            if (touchingBottom(this))
            {
                if (Velocity.Y > 0)
                    Velocity.Y = Velocity.Y * -1f;
            }
        }

        public void EndAttack(ref int AITimer, ref int AttackNumber)
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            Drawing.BetterDraw(Texture, Position, null, Colour, Rotation, Size, SpriteEffects.None, 0f);

            Utilities.drawTextInDrawMethod(CurrentBeat.ToString(), new Vector2(Main.ScreenWidth / 5, Main.ScreenHeight / 5), spriteBatch, Main.font, Colour);
        }
    }
}

