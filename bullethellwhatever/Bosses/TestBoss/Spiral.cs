using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Base;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class Spiral : BossAttack
    {
        public int ProjectilesInSpiral;
        public float RotationSpeed;

        public Spiral(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    ProjectilesInSpiral = 4;
                    RotationSpeed = 70f;
                    break;
                case GameState.Difficulties.Normal:
                    ProjectilesInSpiral = 6;
                    RotationSpeed = 60f;
                    break;
                case GameState.Difficulties.Hard:
                    ProjectilesInSpiral = 8;
                    RotationSpeed = 60f;
                    break;
                case GameState.Difficulties.Insane:
                    ProjectilesInSpiral = 10;
                    RotationSpeed = 50f;
                    break;
            }
        }

        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            List<BasicProjectile> projectilesToShoot = new List<BasicProjectile>();

            int timeToStartAt = Owner.BarDuration * 2;
            int endTime = timeToStartAt + Owner.BarDuration * 10;

            if (AITimer <= timeToStartAt)
            {
                MoveToCentre(AITimer, timeToStartAt);
                SpinUpCounterClockwise(ref Owner.Rotation, 60f);
            }

            if (AITimer == timeToStartAt)
            {
                Owner.Velocity = Vector2.Zero;
            }
            if (AITimer % 2 == 0 && AITimer > timeToStartAt && AITimer < endTime - Owner.BarDuration)
            {

                float acceleration = 0.52f * MathF.Cos(AITimer / 250f + MathF.PI);
                float rotation = AITimer / 15f * MathF.PI / RotationSpeed * acceleration;

                Owner.Rotation = rotation;

                for (int i = 0; i < ProjectilesInSpiral; i++)
                {
                    projectilesToShoot.Add(new BasicProjectile()); //add a projectile

                    // shoot projectiles in a ring and rotate it based on time
                    Vector2 velocity = 7f * Utilities.SafeNormalise(Utilities.RotateVectorClockwise(new Vector2(0, -1), Utilities.ToRadians(i * 360 / ProjectilesInSpiral) + rotation), Vector2.Zero);

                    projectilesToShoot[i].Spawn(Owner.Position, velocity, 1f, 1, "box", 1, Vector2.One, Owner, true, Color.Red, true, false);


                }
            }
        }
    }
}
