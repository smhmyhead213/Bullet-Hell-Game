using bullethellwhatever.BaseClasses;
using bullethellwhatever.Enemies;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class EnemySpam : BossAttack
    {
        public int NumberOfEnemies;
        public int NumberOfProjectiles;
        public EnemySpam(int endTime) : base(endTime)
        {
            EndTime = endTime;           
        }

        public override void InitialiseAttackValues()
        {
            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    NumberOfProjectiles = 4;
                    break;
                case GameState.Difficulties.Normal:
                    NumberOfProjectiles = 6;
                    break;
                case GameState.Difficulties.Hard:
                    NumberOfProjectiles = 8;
                    break;
                case GameState.Difficulties.Insane:
                    NumberOfProjectiles = 10;
                    break;
            }
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            if (AITimer == 0)
            {
                NumberOfEnemies = EndTime / (Owner.BarDuration / 4);
                Owner.Velocity = Vector2.Zero;
                Owner.Position = Utilities.CentreOfScreen();
            }

            if (AITimer % (Owner.BarDuration / 4) == 0 && AITimer < EndTime - (Owner.BarDuration * 8))
            {
                ChargingEnemy enemy = new ChargingEnemy(60, 120);

                Vector2 toPlayer = Utilities.SafeNormalise(Main.player.Position - Owner.Position, Vector2.Zero);

                enemy.Spawn(Owner.Position, 5f * Utilities.RotateVectorClockwise(toPlayer, MathHelper.TwoPi / NumberOfEnemies * AITimer / (Owner.BarDuration / NumberOfEnemies)), 1f, "box", Vector2.One, 1f, 1, Color.White,
                    false, true);
            }

            if (AITimer == EndTime - (Owner.BarDuration * 3))
            {
                foreach (NPC npc in Main.activeNPCs) 
                {
                    if (npc is not Boss)
                    {
                        ExponentialAcceleratingProjectile proj = new ExponentialAcceleratingProjectile(45, 4);

                        TelegraphLine t = new TelegraphLine(Utilities.VectorToAngle(Main.player.Position - npc.Position), 0f, 0f, 10, 2000, 40, npc.Position, Color.White, "box", Owner, false);

                        proj.Spawn(npc.Position, Main.player.Position - npc.Position,
                            1f, 1, "box", 1f, Vector2.One, Owner, true, Color.Red, true, false);
                    }
                }

                foreach (NPC npc in Main.activeNPCs)
                {
                    npc.DeleteNextFrame = true;
                }
            }
        }
    }
}
