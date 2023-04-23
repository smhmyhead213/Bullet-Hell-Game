using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base; //bad code
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.TelegraphLines;

namespace bullethellwhatever.Bosses
{
    public class SecondBoss : Boss
    {
        public bool HasChosenChargeDirection;
        public int AttackNumber; //position in pattern
        public float DeathrayAngularVelocity;

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

            AITimer++;
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

                    explodingProjectile.Spawn(Position, 3f * Utilities.SafeNormalise(Utilities.RotateVectorClockwise(Vector2.UnitY, angle + offset), Vector2.Zero), 1f, Texture, 1f, Vector2.One, this, true, Color.Red, false);
                }
            }
        }

        
        public void DialogueTest(ref float AITimer, ref int AttackNumber)
        {
            Deathray deathray = new Deathray();
            Deathray deathray2 = new Deathray();
            if (AITimer == 0)
            {
                dialogueSystem.Dialogue(Position, "This boss is in progress, ignore it.", 4, 400);
                deathray.SpawnDeathray(Position, MathF.PI + MathF.PI / 6, 1f, Texture, 50f, 2000f, 0f, 0f, true, Color.Red, Main.deathrayShader, this);
                deathray2.SpawnDeathray(Position, MathF.PI / 6, 1f, Texture, 50f, 2000f, 0f, 0f, true, Color.Red, Main.deathrayShader2, this);
                activeTelegraphs.Add(new TelegraphLine(0f, MathF.PI / 2708f, 0f, 20f, 500f, 100, Position, Color.Yellow, Texture, this));
                Drawing.ScreenShake(4, 300);
            }            

            if (AITimer > 0)
            {
                //float angle = Utilities.VectorToAngle(Main.player.Position - Position);

                //activeTelegraphs[0].Rotation = angle - MathHelper.PiOver2;
            }

            if (AITimer == 450)
            {
                dialogueSystem.Dialogue(Position, "among us", 4, 400);
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

