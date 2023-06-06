using bullethellwhatever.Enemies;
using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class SpawnEnemies : BossAttack
    {
        public int NumberOfEnemies;

        public SpawnEnemies(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    NumberOfEnemies = 4;
                    break;
                case GameState.Difficulties.Normal:
                    NumberOfEnemies = 6;
                    break;
                case GameState.Difficulties.Hard:
                    NumberOfEnemies = 7;
                    break;
                case GameState.Difficulties.Insane:
                    NumberOfEnemies = 8;
                    break;
            }
        }

        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            //if (AITimer <= Owner.BarDuration * 1 && AITimer % (Owner.BarDuration / NumberOfEnemies) == 0)
            //{
            //    ChargingEnemy enemy = new ChargingEnemy(60, 120);

            //    Vector2 toPlayer = Utilities.SafeNormalise(Main.player.Position - Owner.Position, Vector2.Zero);

            //    enemy.Spawn(Owner.Position, 5f * Utilities.RotateVectorClockwise(toPlayer, MathHelper.TwoPi / NumberOfEnemies * AITimer / (Owner.BarDuration / NumberOfEnemies)), 1f, "box", Vector2.One, 1f, 1, Color.White,
            //        false, true);
            //}
        }
    }
}
