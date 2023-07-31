﻿using Microsoft.Xna.Framework;
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
using bullethellwhatever.Projectiles._3D;

namespace bullethellwhatever.Bosses
{
    public class SecondBoss : Boss
    {
        public bool HasChosenChargeDirection;

        public float DeathrayAngularVelocity;

        public Deathray ray;
        public Deathray ray2;
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
            Rotation = PI / 6f;
            Size = new Vector2(3f, 3f);

            Opacity = 0.1f;

            Colour = Color.White;

            Texture = Assets["box"];
        }

        public override HitboxTypes GetHitboxType()
        {
            return HitboxTypes.RotatableRectangle;
        }

        public override void UpdateHitbox()
        {
            // the position used in collision code should not be the centre.

            float hypotenuse = Texture.Height / 2f * Size.Y;

            Vector2 positionForHitboxPurposes = new Vector2(Position.X - hypotenuse * Sin(Rotation), Position.Y + hypotenuse * Cos(Rotation));

            Hitbox.RotatableHitbox.UpdateRectangle(Rotation, Texture.Width * Size.X, Texture.Height * Size.Y, positionForHitboxPurposes, Position);

            Hitbox.RotatableHitbox.UpdateVertices();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            for (int i = 0; i < Hitbox.RotatableHitbox.Vertices.Length; i++)
            {
                Drawing.BetterDraw(Texture, Hitbox.RotatableHitbox.Vertices[i], null, Color.Red, 0, Vector2.One, SpriteEffects.None, 0);
                Utilities.drawTextInDrawMethod(i.ToString(), Hitbox.RotatableHitbox.Vertices[i] + new Vector2(30f, 0f), spriteBatch, font, Colour);
            }
        }
        public override void AI()
        {

            if (Health < 0 && IsDesperationOver)
                Die();


            Rotation = Rotation + PI / 90f;
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

                //TelegraphLine t = new(0f, MathF.PI / 600f, 0f, 20f, 500f, 10000, Position, Color.Yellow, "box", this, true);
                
                //Cuboid cuboid = new Cuboid(500, 500, 500, this);

                //ray = new Deathray();
                //ray2 = new Deathray();

                //ray.SpawnDeathray(new Vector2(ScreenWidth / 4 + 50f, ScreenHeight / 2), PI / 2f, 0f, 9999, "box", 30, 3000, 0, 0, true, Colour, "DeathrayShader", this);
                //ray2.SpawnDeathray(new Vector2(ScreenWidth / 4 * 3 + 50f, ScreenHeight / 2), 0f, 0f, 9999, "box", 30, 3000, PI / 240f, 0, true, Colour, "DeathrayShader", this);
            }

            if (AITimer == 120)
            {
                //ChargingEnemy enemy = new ChargingEnemy(60, 120);

                //enemy.Spawn(Position, 7f * Vector2.UnitY, 1f, "box", Vector2.One, 3f, 1, Color.White, false, true);
            }


        }

        public void EndAttack(ref float AITimer, ref int AttackNumber)
        {
            AITimer = 0; //to prevent jank with EndAttack taking a frame, allows attacks to start on 0
            Rotation = 0;
            AttackNumber++;
        }
    }
}

