using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.Projectiles.TelegraphLines;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class LiterallyJustABulletHell : BossAttack
   {
        List<Deathray> pendingDeathrays = new List<Deathray>();
        List<int> deathraySpawnTimes = new List<int>();

        public int ProjectileFrequency;
        public LiterallyJustABulletHell(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    ProjectileFrequency = 30;
                    break;
                case GameState.Difficulties.Normal:
                    ProjectileFrequency = 25;
                    break;
                case GameState.Difficulties.Hard:
                    ProjectileFrequency = 20;
                    break;
                case GameState.Difficulties.Insane:
                    ProjectileFrequency = 15;
                    break;
            }
        }

        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            Owner.DealDamage = false;

            Owner.Velocity = Vector2.Zero;

            if (AITimer > Owner.BarDuration * 2)
            {
                if (AITimer % ProjectileFrequency == 0)
                {
                    Owner.Velocity = Vector2.Zero;

                    Random rng = new Random();

                    Projectile[] possibleProjectiles = { new BasicProjectile(), new ExplodingProjectile(8, 120, 0, false, false, false), new OscillatingSpeedProjectile(10, 5f), new Deathray() };

                    int projIndexToSpawn = rng.Next(possibleProjectiles.Length);

                    int sideDeterminant = rng.Next(2); // 0 is left/bottom, 1 is right/top

                    if (possibleProjectiles[projIndexToSpawn] is not Deathray)
                    {
                        int height = rng.Next(ScreenHeight);

                        int side = sideDeterminant * ScreenWidth;

                        int direction = sideDeterminant == 1 ? -1 : 1; // -1 is right/top  of screen, 1 is left/bottom

                        Color colour = possibleProjectiles[projIndexToSpawn] is OscillatingSpeedProjectile ? Color.White : Color.Red;

                        possibleProjectiles[projIndexToSpawn].Spawn(new Vector2(side, height), 5f * new Vector2(direction, 0), 1f, 1, "box", 0f, Vector2.One, Owner, true, colour, true, false);

                        Projectile projectile = new Projectile();

                        //kill me now

                        switch (projIndexToSpawn) 
                        {
                            case 1:
                                projectile = new ExplodingProjectile(8, 120, 0, false, false, false);
                                break;
                            case 2:
                                projectile = new OscillatingSpeedProjectile(10, 5f);
                                break;
                        }

                        int width = rng.Next(ScreenWidth);

                        side = sideDeterminant * ScreenWidth;

                        projectile.Spawn(new Vector2(width, side), 5f * new Vector2(0, direction), 1f, 1, "box", 0f, Vector2.One, Owner, true, colour, true, false);
                    }
                    else
                    {
                        Vector2 rayOrigin = new Vector2(sideDeterminant * ScreenWidth, rng.Next(ScreenHeight));
                        Vector2 verticalRayOrigin = new Vector2(rng.Next(ScreenWidth), ScreenHeight - (ScreenHeight * sideDeterminant));

                        TelegraphLine telegraphLine = new TelegraphLine(PI * sideDeterminant + PI / 2, 0f, 0f, 20f, ScreenWidth, 90, rayOrigin, Color.White, "box", Owner, false);

                        TelegraphLine verticalTelegraphLine = new TelegraphLine(PI * sideDeterminant, 0f, 0f, 20f, ScreenHeight, 90, verticalRayOrigin, Color.White, "box", Owner, false);

                        Deathray rayToSpawn = new Deathray();

                        rayToSpawn.CreateDeathray(rayOrigin, PI * sideDeterminant + PI / 2, 1f, 90, "box", telegraphLine.Width, telegraphLine.Length, 0f, 0f, true, Color.White, "DeathrayShader", Owner);

                        Deathray verticalRayToSpawn = new Deathray();

                        verticalRayToSpawn.CreateDeathray(verticalRayOrigin, PI * sideDeterminant, rayToSpawn.Damage, rayToSpawn.Duration, "box", rayToSpawn.Width, verticalTelegraphLine.Length,
                            0f, 0f, true, Color.White, "DeathrayShader", Owner);

                        pendingDeathrays.Add(rayToSpawn);
                        pendingDeathrays.Add(verticalRayToSpawn);

                        deathraySpawnTimes.Add(AITimer + telegraphLine.Duration);
                        deathraySpawnTimes.Add(AITimer + telegraphLine.Duration);

                    }
                }

                for (int i = 0; i < pendingDeathrays.Count; i++)
                {
                    if (AITimer == deathraySpawnTimes[i])
                    {
                        pendingDeathrays[i].AddDeathrayToActiveProjectiles();
                    }
                }
            }
        }
    }
}
