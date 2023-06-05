using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses;
using static bullethellwhatever.Utilities;
using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class Circle : BossAttack
    {
        public float AngularVelocity;

        public Circle(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    AngularVelocity = MathF.PI / 210; 
                    break;
                case GameState.Difficulties.Normal:
                    AngularVelocity = MathF.PI / 180;
                    break;
                case GameState.Difficulties.Hard:
                    AngularVelocity = MathF.PI / 150;
                    break;
                case GameState.Difficulties.Insane:
                    AngularVelocity = MathF.PI / 120;
                    break;
            }

            AngularVelocity = MathF.PI / 150;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            float distance = DistanceBetweenVectors(Main.player.Position, CentreOfScreen());

            // Constrict the circle depending on player closeness to edge, to prevent cheese.

            Owner.Position = Main.player.Position + ((distance * -4f / 11f + 600f)* AngleToVector(AngularVelocity * AITimer));

            if (AITimer % 2 == 0)
            {
                Projectile projectile = new Projectile();

                projectile.Spawn(Owner.Position, 10f * SafeNormalise(Main.player.Position - Owner.Position), 1f, 1, "box", 0f, Vector2.One, Owner, true, Color.Red, true, false);
            }

        }
    }
}
