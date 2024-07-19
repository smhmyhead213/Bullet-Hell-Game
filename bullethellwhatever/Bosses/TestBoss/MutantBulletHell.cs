using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class MutantBulletHell : BossAttack
    {
        public int ProjectilesInSpiral;

        public MutantBulletHell(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    ProjectilesInSpiral = 6;
                    break;
                case GameState.Difficulties.Normal:
                    ProjectilesInSpiral = 8;
                    break;
                case GameState.Difficulties.Hard:
                    ProjectilesInSpiral = 10;
                    break;
                case GameState.Difficulties.Insane:
                    ProjectilesInSpiral = 12;
                    break;
            }
        }

        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            if (AITimer <= Owner.BarDuration * 3)
            {
                MoveToCentre(AITimer, Owner.BarDuration * 3);
                SpinUpClockwise(ref Owner.Rotation, 80);
            }

            List<Projectile> projectilesToShoot = new List<Projectile>();

            if (AITimer % 5 == 0 && AITimer > Owner.BarDuration * 3 && AITimer < Owner.BarDuration * 10)
            {
                float rotation = AITimer / 10f * MathF.PI / 40f * (AITimer / 100f);

                Owner.Rotation = rotation;

                for (int i = 0; i < ProjectilesInSpiral; i++)
                {
                    projectilesToShoot.Add(new Projectile()); //add a projectile

                    // shoot projectiles in a ring and rotate it based on time
                    Vector2 velocity = 5.5f * Utilities.SafeNormalise(Utilities.RotateVectorClockwise(new Vector2(0, -1), Utilities.ToRadians(i * 360 / ProjectilesInSpiral) + rotation), Vector2.Zero);

                    projectilesToShoot[i].SpawnProjectile(Owner.Position, velocity, 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                }


            }

            if (AITimer == Owner.BarDuration * 10)
            {
                Owner.Rotation = 0;
            }
        }
    }
}
