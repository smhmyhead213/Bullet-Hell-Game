using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;

namespace bullethellwhatever.Bosses
{
    public class SecondBoss : Boss
    {
        public bool HasChosenChargeDirection;        
        public int AttackNumber; //position in pattern
       


        public SecondBoss(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
            isBoss = true;
            isPlayer = false;
            Health = 150;
            AITimer = 0f;
            IFrames = 5f;
            HasChosenChargeDirection = false;
            AttackNumber = 1;
            IsDesperationOver = false;
        }

        public override void AI()
        {

            CheckForAndTakeDamage();

            if (Health < 0 & IsDesperationOver)
                DeleteNextFrame = true;



            //Update the boss position based on its velocity.
            Position = Position + Velocity;


            if (IFrames > 0)
            {
                IFrames--;
            }

            AITimer++;

            ExecuteEasyAttackPattern();
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
                    Explosions(ref AITimer, ref AttackNumber, 4);
                    break;                
                default:
                    AttackNumber = 1;
                    break;

            }
        }

        public void Explosions(ref float AITimer, ref int AttackNumber, int numberOfProjectiles) // this needs fixed
        {
            float offset = MathF.PI / 500f * AITimer;

            if (AITimer % 2 == 0)
            {
                for (int i = 0; i < numberOfProjectiles; i++)
                {
                    ExplodingProjectile explodingProjectile = new ExplodingProjectile(4, 120, -0.5f * offset, false, false, true);

                    float angle = (2 * MathF.PI / numberOfProjectiles) * i;

                    explodingProjectile.Spawn(Position, 3f * Utilities.SafeNormalise(Utilities.RotateVectorClockwise(Vector2.UnitY,  angle + offset), Vector2.Zero), 1f, Texture, 1f, Vector2.One);
                }
            }
        }

        
        public void EndAttack(ref float AITimer, ref int AttackNumber)
        {
            AITimer = -1; //to prevent jank with EndAttack taking a frame, allows attacks to start on 0
            Rotation = 0;
            AttackNumber++;
        }
    }
}
