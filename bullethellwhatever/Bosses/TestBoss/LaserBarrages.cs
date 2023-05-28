using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.UtilitySystems.Dialogue;
using System;
using Microsoft.Xna.Framework;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class LaserBarrages : BossAttack
    {
        public float AngleBetween;
        public int TimeBetweenRays;
        public int NumberOfRaysBetweenTelegraphAndBeam;

        public LaserBarrages(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            NumberOfRaysBetweenTelegraphAndBeam = 8;

            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    AngleBetween = MathF.PI / 6.9f;
                    TimeBetweenRays = 11;
                    break;
                case GameState.Difficulties.Normal:
                    AngleBetween = MathF.PI / 7.9f;
                    TimeBetweenRays = 9;
                    break;
                case GameState.Difficulties.Hard:
                    AngleBetween = MathF.PI / 9.1f;
                    TimeBetweenRays = 7;
                    break;
                case GameState.Difficulties.Insane:
                    AngleBetween = MathF.PI / 11.1f;
                    TimeBetweenRays = 5;
                    break;
            }
        }

        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            EndTime = Owner.BarDuration * 7;

            if (AITimer == 0)
            {
                Owner.Position = new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2);
                Owner.Velocity = Vector2.Zero;
            }

            if (AITimer % TimeBetweenRays == 0)
            {
                if (AITimer < EndTime - TimeBetweenRays * NumberOfRaysBetweenTelegraphAndBeam)
                {
                    TelegraphLine TeleLine = new TelegraphLine(AngleBetween * AITimer / TimeBetweenRays, 0f, 0f, 20, 2000, TimeBetweenRays * NumberOfRaysBetweenTelegraphAndBeam, Owner.Position, Color.White, "box", Owner);
                }

                if (AITimer > TimeBetweenRays * NumberOfRaysBetweenTelegraphAndBeam)
                {
                    Deathray ray = new Deathray();
                    ray.SpawnDeathray(Owner.Position, AngleBetween * (AITimer - TimeBetweenRays * NumberOfRaysBetweenTelegraphAndBeam) / TimeBetweenRays, 1f, 50, "box", 30, 2000, 0, 0, true, Color.Red, "DeathrayShader", Owner);
                }
            }
        }
    }
}
