using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.TelegraphLines;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bullethellwhatever.Projectiles;
using bullethellwhatever.DrawCode;


namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBoss : Boss
    {
        public CrabLeg[] Legs;
        public Vector2[] BoosterPositions;
        public float idivisior;
        public float c;
        public float ydivisor;
        public bool BoostersActive;
        public bool LockArmPositions;
        public CrabBoss()
        {
            Texture = Assets["CrabBody"];
            Size = Vector2.One * 1.5f;
            Position = Utilities.CentreOfScreen();
            MaxHP = 2500;
            Health = MaxHP;
            AttackNumber = 1;
            Colour = Color.White;

            BoostersActive = false;
            LockArmPositions = true;

            BarDuration = 60; //to be changed

            Legs = new CrabLeg[2];
            BoosterPositions = new Vector2[2];

            for (int i = 0; i < 2; i++)
            {
                int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                Legs[i] = new CrabLeg(Position + new Vector2(expandedi * Texture.Width / 1.4f, Texture.Height / 2.54f), this, i);

                if (i == 0)
                {
                    Legs[i].HorizontalFlip = true;
                }

                Legs[i].UpperArm.RotationConstant = -expandedi * PI / 12;
                Legs[i].LowerArm.RotationConstant = expandedi * PI / 12;
                //Legs[0].UpperClaw.RotationConstant = expandedi * PI;
                //Legs[0].LowerClaw.RotationConstant = expandedi * PI; // fix later

            }

            BossAttacks = new CrabBossAttack[]
            {
                new TestAttack(BarDuration * 30),
                new TestAttack(BarDuration * 15),
                new CrabCharge(BarDuration * 18),
                new Minefield(1550),
                new BackgroundPunches(390 * 7),
                new ExpandingOrb(1700),
            };

            for (int i = 0; i < BossAttacks.Length; i++)
            {
                BossAttacks[i].Owner = this;
                BossAttacks[i].InitialiseAttackValues();
            }

            //TelegraphLine t = new TelegraphLine(PI, 0, 0, 20, 2000, 9999, new Vector2(ScreenWidth / 2, 0), Color.White, "box", this, false);
            //TelegraphLine really = new TelegraphLine(PI / 2, 0, 0, 20, 2000, 9999, new Vector2(0 , ScreenHeight / 2), Color.White, "box", this, false);
        }

        public void ResetArmRotations()
        {
            for (int i = 0; i < 2; i++)
            {
                //int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                Legs[i].ResetRotations();
            }
        }

        public void SetArmDepth(int armIndex, float depth) // 0 or 1 for left and right arms
        {
            Legs[armIndex].UpperArm.Depth = depth;
            Legs[armIndex].LowerArm.Depth = depth;
            Legs[armIndex].UpperClaw.Depth = depth;
            Legs[armIndex].LowerClaw.Depth = depth;
        }
        public void SetBoosters(bool on)
        {
            BoostersActive = on;
        }
        public void FacePlayer()
        {
            Rotation = Utilities.VectorToAngle(Position - player.Position);
        }
        public override void AI()
        {
            //Rotation = Rotation + PI / 90f;

            BossAttacks[AttackNumber].TryEndAttack(ref AITimer, ref AttackNumber);

            BossAttacks[AttackNumber].Execute(ref AITimer, ref AttackNumber);

            for (int i = 0; i < 2; i++)
            {
                int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                if (Legs[i] is not null)
                {
                    float factorToMoveArms = MathHelper.Lerp(1f, 0.1f, Depth);
                    if (LockArmPositions)
                    {
                        Legs[i].Position = Position + Utilities.RotateVectorClockwise(new Vector2(expandedi * Texture.Width / 1.4f * factorToMoveArms, Texture.Height / 2.54f * factorToMoveArms), Rotation);
                    }
                    BoosterPositions[i] = Position + Utilities.RotateVectorClockwise(new Vector2(expandedi * Texture.Width / 2.1f, -Texture.Height / 4f), Rotation);
                    Legs[i].Update();
                }
            }

            if (Health <= 0)
            {
                IsDesperationOver = true;
                Die();
            }
        }
        public override void Die()
        {
            IsDesperationOver = true; //remove
            base.Die();

            Array.Clear(Legs);

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (BoostersActive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate);

                Shader = Shaders["CrabRocketBoosterShader"];
                Shader.CurrentTechnique.Passes[0].Apply();

                for (int i = 0; i < BoosterPositions.Length; i++)
                {
                    float width = MathHelper.Clamp(Abs(Velocity.Y) / 1f, 0f, 2f);
                    float height = Velocity.Length();
                    Texture2D texture = Assets["box"];

                    Vector2 size = new Vector2(width, height);

                    Vector2 drawPos = BoosterPositions[i];
                    drawPos = drawPos - Utilities.RotateVectorClockwise(new Vector2(0f, texture.Height * (0.5f * height - 0.5f)), Rotation);

                    //drawPos.X = BoosterPositions[i].X + size.X;

                    Vector2 originOffset = new Vector2(0f, 0f);
                 
                    //spriteBatch.Draw(Assets["box"], drawPos, null, Colour, Rotation + PI, originOffset, size, SpriteEffects.None, 1);
                    Drawing.BetterDraw(texture, drawPos, null, Colour, Rotation + PI, size, SpriteEffects.None, 1);
                }

                spriteBatch.End();

                if (this is IDrawsShader) //if we were already drawing stuff that had shaders
                    spriteBatch.Begin(SpriteSortMode.Immediate);
                else spriteBatch.Begin(SpriteSortMode.Deferred);

                //for (int i = 0; i < BoosterPositions.Length;i++) //remove later
                //{
                //    Vector2 drawPos = BoosterPositions[i];
                //    drawPos = drawPos - Utilities.RotateVectorClockwise(new Vector2(0f, Assets["box"].Height * (0.5f * Velocity.Length() - 0.5f)), Rotation);
                //    Drawing.BetterDraw(Assets["box"], drawPos, null, Color.White, Rotation + PI, Vector2.One, SpriteEffects.None, 1);
                //}
            }

            base.Draw(spriteBatch);

            //Hitbox.DrawHitbox();

            foreach (CrabLeg leg in Legs)
            {
                if (leg is not null)
                {
                    leg.DrawHitboxes();
                }
            }

            Drawing.BetterDraw(Assets["box"], Legs[0].LowerArm.CalculateEnd(), null, Color.Red, 0, Vector2.One, SpriteEffects.None, 1);
            Utilities.drawTextInDrawMethod(Legs[0].LowerArm.CalculateFinalRotation().ToDegrees().ToString(), new Vector2(ScreenWidth / 20 * 19, ScreenHeight / 20 * 19), spriteBatch, font, Color.White);
        }
    }
}
