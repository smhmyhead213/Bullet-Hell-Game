﻿using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.TelegraphLines;

using Microsoft.Xna.Framework;
using System;


namespace bullethellwhatever.Bosses.TestBoss
{
    public class BasicShotgunSpread : BossAttack
    {
        public float NumberOfProjectiles;
        public float ProjectileSpeed;
        public float FirstAttackTelegraphLineRotation;
        public int LaserSpinDirection;
        public float MegaRayTeleLineRotation;
        public TelegraphLine MegaRayTeleLine;
        public BasicShotgunSpread(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            ProjectileSpeed = 5f;

            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    NumberOfProjectiles = 14;
                    break;
                case GameState.Difficulties.Normal:
                    NumberOfProjectiles = 20;
                    break;
                case GameState.Difficulties.Hard:
                    NumberOfProjectiles = 24;
                    break;
                case GameState.Difficulties.Insane:
                    NumberOfProjectiles = 30;
                    break;
            }
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            if (AITimer > 0 && AITimer < 5)
            {
                Owner.Velocity = new Vector2(2f, 0f);
                FirstAttackTelegraphLineRotation = -1f;
            }

            float angleBetweenShots = MathF.PI / NumberOfProjectiles;

            float projectileOscillationFrequency = 10f;

            int partTwoStartTime = Owner.BarDuration * 4;

            if (Owner.CurrentBeat == 2 && Owner.JustStartedBeat && AITimer < EndTime - Owner.BarDuration * 3)
            {
                if (AITimer > partTwoStartTime)
                {
                    angleBetweenShots = angleBetweenShots * 2;
                }

                OscillatingSpeedProjectile singleShot = new OscillatingSpeedProjectile(projectileOscillationFrequency, ProjectileSpeed);

                singleShot.Spawn(Owner.Position, ProjectileSpeed * Utilities.Normalise(Main.player.Position - Owner.Position), 1f, 1, "box", 0, Vector2.One, Owner, true, Color.Red, true, false);

                for (int i = 1; i < NumberOfProjectiles / 2 + 0.5f; i++) // loop for each pair of projectiles an angle away from the middle
                {
                    OscillatingSpeedProjectile oscillatingSpeedProjectile = new OscillatingSpeedProjectile(projectileOscillationFrequency, ProjectileSpeed);
                    OscillatingSpeedProjectile oscillatingSpeedProjectile2 = new OscillatingSpeedProjectile(projectileOscillationFrequency, ProjectileSpeed); //one for each side of middle

                    oscillatingSpeedProjectile.Spawn(Owner.Position, Utilities.Normalise(Utilities.RotateVectorClockwise(Main.player.Position - Owner.Position, i * angleBetweenShots)),
                        1f, 1, "box", 1.01f, Vector2.One, Owner, true, Color.Red, true, false);
                    oscillatingSpeedProjectile2.Spawn(Owner.Position, Utilities.Normalise(Utilities.RotateVectorCounterClockwise(Main.player.Position - Owner.Position, i * angleBetweenShots)),
                        1f, 1, "box", 1.01f, Vector2.One, Owner, true, Color.Red, true, false);
                }
            }

            if (AITimer >= Owner.BarDuration * 11 && AITimer <= Owner.BarDuration * 12)
            {
                float angle = Utilities.VectorToAngle(-Vector2.UnitY);

                if (AITimer == Owner.BarDuration * 11)
                {
                    LaserSpinDirection = Main.player.Position.X > Owner.Position.X ? 1 : -1;

                    MegaRayTeleLine = new TelegraphLine(angle, LaserSpinDirection * MathF.PI / 3360, LaserSpinDirection * MathF.PI / (336 * 96), 30f, 2000f, Owner.BarDuration, Owner.Position, Owner.Colour, "box", Owner, true);                 
                }

                MegaRayTeleLineRotation = MegaRayTeleLine.Rotation;

                if (AITimer == Owner.BarDuration * 12)
                {
                    Deathray ray = new Deathray();

                    ray.SpawnDeathray(Owner.Position, MegaRayTeleLineRotation, 1f, Owner.BarDuration * 3 - Owner.FramesPerMusicBeat, "box", 30f, 2000f,
                        LaserSpinDirection * PI / 150f, 0f, true, Owner.Colour, "DeathrayShader", Owner);
                }
            }

            if (AITimer == partTwoStartTime)
            {
                Owner.Position = new Vector2(Main.ScreenWidth / 2, Main.ScreenHeight / 2);
                Owner.Velocity = Vector2.Zero;
            }

            if (AITimer > partTwoStartTime - Owner.BarDuration)
            {
                if (Owner.CurrentBeat == 1 && Owner.JustStartedBeat)
                {
                    FirstAttackTelegraphLineRotation = Utilities.VectorToAngle(Main.player.Position - Owner.Position);
                    TelegraphLine telegraph = new TelegraphLine(FirstAttackTelegraphLineRotation, 0f, 0f, 20f, 2000f, Owner.FramesPerMusicBeat * 2, Owner.Position, Owner.Colour, "box", Owner, true);
                }

                if (Owner.CurrentBeat == 3 && Owner.JustStartedBeat)
                {
                    if (FirstAttackTelegraphLineRotation != -1f) //-1 is a flag for if the direction has not yet been decided
                    {
                        Deathray ray = new Deathray();
                        ray.SpawnDeathray(Owner.Position, FirstAttackTelegraphLineRotation, 1f, Owner.FramesPerMusicBeat * 2, "box", 20f, 2000f, 0f, 0f, true, Owner.Colour, "DeathrayShader", Owner);
                    }

                    //float predictionStrength = 500f;
                    
                }

            }

            HandleBounces();

        }
    }
}
