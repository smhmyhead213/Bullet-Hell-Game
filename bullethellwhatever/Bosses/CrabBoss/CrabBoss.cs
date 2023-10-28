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
using System.Windows.Forms;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBoss : Boss
    {
        public CrabLeg[] Legs;
        public Vector2[] BoosterPositions;
        public Vector2[] ArmPositionsOnBody;
        public bool BoostersActive;
        public bool LockArmPositions;
        public bool PlayerSaidOpeningDialogue;
        public bool StartedDeathAnim;

        public int Phase; // flag for if arms are detached yet

        public bool StartedPhaseTwoTransition;

        public CrabBossAttack[] PhaseTwoAttacks;
        public CrabBoss()
        {
            Texture = Assets["CrabBody"];
            Size = Vector2.One * 1.5f;
            Position = Utilities.CentreOfScreen();
            MaxHP = 550f;
            Health = MaxHP;
            AttackNumber = 1;
            Colour = Color.White;

            Phase = 1;

            StartedPhaseTwoTransition = false;

            BoostersActive = false;
            LockArmPositions = true;
            PlayerSaidOpeningDialogue = false;

            BarDuration = 60; //to be changed

            Legs = new CrabLeg[2];
            ArmPositionsOnBody = new Vector2[2];
            BoosterPositions = new Vector2[2];

            StartedDeathAnim = false;

            for (int i = 0; i < 2; i++)
            {
                int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                Vector2 pos = Position + new Vector2(expandedi * Texture.Width / 1.4f, Texture.Height / 2.54f);

                Legs[i] = new CrabLeg(pos, this, i);

                ArmPositionsOnBody[i] = pos;

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
                new CrabCharge(BarDuration * 18 - 60),
                new SideImpacts(1000),
                new BackgroundPunches(390 * 7),
                new ExpandingOrb(1700),
            };

            PhaseTwoAttacks = new CrabBossAttack[]
            {
                new TestAttack(BarDuration * 30), // put desp / death anim here
                new CrabDeathray(1200),
                new SpinningAtPlayer(1200),
                new ChargingArmBulletHell(900),
                new RainingProjectileCharges(900),
                new MovingBlender(1000),
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
            if (!PlayerSaidOpeningDialogue)
            {
                PlayerSaidOpeningDialogue = true;
                //player.dialogueSystem.Dialogue("That body seems to be impervious to damage... Maybe I should try to take out the arms first?", 2, 300);
            }
            //Rotation = Rotation + PI / 90f;

            BossAttacks[AttackNumber].Execute(ref AITimer, ref AttackNumber);

            BossAttacks[AttackNumber].TryEndAttack(ref AITimer, ref AttackNumber);

            if (Legs[0].Dead && Legs[1].Dead && !StartedPhaseTwoTransition)
            {
                CrabBossAttack[] attacks = new CrabBossAttack[]
                {
                    new PhaseTwoTransition(1000), // flush attack pattern and replace
                };

                ReplaceAttackPattern(attacks);
                
                StartedPhaseTwoTransition = true;
            }

            if (StartedPhaseTwoTransition)
            {
                SetDR(0f);
                TargetableByHoming = true;
            }
            else
            {
                SetDR(1f);
                TargetableByHoming = false;
            }

            if (Health <= 0 && StartedDeathAnim == false)
            {
                ReplaceAttackPattern(new BossAttack[] { new CrabDeathAnimation(1000) });
                StartedDeathAnim = true;
            }
        }

        public override void Update()
        {
            base.Update();

            for (int i = 0; i < 2; i++)
            {
                Legs[i].Update();

                int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                if (Legs[i] is not null)
                {
                    BoosterPositions[i] = Position + Utilities.RotateVectorClockwise(new Vector2(expandedi * Texture.Width / 2.1f, -Texture.Height / 4f), Rotation);
                }
            }
        }
        public override void Die()
        {
            base.Die();
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
                    float width = MathHelper.Clamp(Abs(Velocity.Y) / 1f, 0f, 2.5f);
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

                if (this is IDrawsShader) //if we were already drawing stuff that had shaders
                    DrawGame.RestartSpriteBatchForShaders(spriteBatch);
                else DrawGame.RestartSpriteBatchForNotShaders(spriteBatch);

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
                    //leg.DrawHitboxes();
                }
            }
        }
    }
}
