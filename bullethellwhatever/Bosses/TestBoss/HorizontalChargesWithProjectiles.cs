using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Base;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class HorizontalChargesWithProjectiles : BossAttack
    {
        public float MoveSpeed;

        public HorizontalChargesWithProjectiles(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    MoveSpeed = 3f;
                    break;
                case GameState.Difficulties.Normal:
                    MoveSpeed = 5f;
                    break;
                case GameState.Difficulties.Hard:
                    MoveSpeed = 6.5f;
                    break;
                case GameState.Difficulties.Insane:
                    MoveSpeed = 8f;
                    break;
            }
        }

        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            float screenFraction = 8f;


            if (AITimer % (Owner.BarDuration * 4) == 0)
            {
                Owner.Position = new Vector2(Main._graphics.PreferredBackBufferWidth / screenFraction, Main._graphics.PreferredBackBufferHeight / screenFraction);
                Owner.Velocity = MoveSpeed * Utilities.Normalise(new Vector2(Main._graphics.PreferredBackBufferWidth / screenFraction * 1.33f, Main._graphics.PreferredBackBufferHeight / screenFraction) - Owner.Position); // go to target position from current
            }

            if (AITimer % (Owner.BarDuration * 4) == Owner.BarDuration * 2)
            {
                Owner.Position = new Vector2(Main._graphics.PreferredBackBufferWidth / screenFraction * 7f, Main._graphics.PreferredBackBufferHeight / screenFraction * 7f);
                Owner.Velocity = MoveSpeed * Utilities.Normalise(new Vector2(Main._graphics.PreferredBackBufferWidth / screenFraction, Main._graphics.PreferredBackBufferHeight / screenFraction * 7f) - Owner.Position);
            }
        }
    }
}
