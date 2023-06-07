using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base; //bad code
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Enemies;

namespace bullethellwhatever.Bosses
{
    public class SecondBoss : Boss
    {
        public bool HasChosenChargeDirection;

        public float DeathrayAngularVelocity;

        public SecondBoss(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
            isBoss = true;
            isPlayer = false;
            Health = 200;
            AITimer = 0;
            IFrames = 5f;
            HasChosenChargeDirection = false;
            AttackNumber = 1;
            IsDesperationOver = false;
            dialogueSystem = new DialogueSystem(this);
            DeathrayAngularVelocity = 180f;
            IsHarmful = true;
        }



        public override void AI()
        {

            if (Health < 0 && IsDesperationOver)
                DeleteNextFrame = true;



            //Update the boss position based on its velocity.
            Position = Position + Velocity;


            if (IFrames > 0)
            {
                IFrames--;
            }



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
                    DialogueTest(ref AITimer, ref AttackNumber);
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

                    explodingProjectile.Spawn(Position, 3f * Utilities.SafeNormalise(Utilities.RotateVectorClockwise(Vector2.UnitY, angle + offset), Vector2.Zero), 1f, 1, "box", 1f, Vector2.One, this, true, Color.Red, false, false);
                }
            }
        }

        
        public void DialogueTest(ref int AITimer, ref int AttackNumber)
        {
            if (AITimer == 0)
            {              
                dialogueSystem.Dialogue(Position, "This boss is in progress, ignore it.", 4, 400);

                TelegraphLine t = new(0f, MathF.PI / 600f, 0f, 20f, 500f, 10000, Position, Color.Yellow, "box", this, true);

                ExponentialAcceleratingProjectile proj = new ExponentialAcceleratingProjectile(120, 4);

                proj.Spawn(Position + new Vector2(200, 0), Main.player.Position - proj.Position, 1f, 1, "box", 0f, Vector2.One, this, true, Color.Red, true, false);
            }            

            if (AITimer == 120)
            {
                ChargingEnemy enemy = new ChargingEnemy(60 , 120);

                enemy.Spawn(Position, 7f * Vector2.UnitY, 1f, "box", Vector2.One, 3f, 1, Color.White, false, true);
            }


        }

        public void EndAttack(ref float AITimer, ref int AttackNumber)
        {
            AITimer = -1; //to prevent jank with EndAttack taking a frame, allows attacks to start on 0
            Rotation = 0;
            AttackNumber++;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (activeTelegraphs.Count > 0)
                Utilities.drawTextInDrawMethod(activeTelegraphs[0].Rotation.ToDegrees().ToString(), new Vector2(Main.ScreenWidth / 3, Main.ScreenHeight / 3), spriteBatch, Main.font, Color.White);
        }
    }
}

