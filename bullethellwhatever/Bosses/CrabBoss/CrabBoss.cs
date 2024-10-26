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
using bullethellwhatever.DrawCode.UI;

using bullethellwhatever.AssetManagement;
using bullethellwhatever.Bosses.CrabBoss.Attacks;

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

        public float SpinVelOnDeath = PI / 40;

        public CrabBoss()
        {
            Texture = AssetRegistry.GetTexture2D("CrabBody");
            Size = Vector2.One * 1.5f;
            Position = Utilities.CentreOfScreen();
            MaxHP = 400f;

            Colour = Color.White;

            Phase = 1;

            StartedPhaseTwoTransition = false;

            BoostersActive = false;
            LockArmPositions = true;
            PlayerSaidOpeningDialogue = false;

            Legs = new CrabLeg[2];
            ArmPositionsOnBody = new Vector2[2];
            BoosterPositions = new Vector2[2];

            StartedDeathAnim = false;

            for (int i = 0; i < 2; i++)
            {
                int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                Vector2 pos = CalculateArmPostions(expandedi);

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

            CurrentAttack = new NeutralToCrabFlailChargeTransition(this);

            HealthBar hpBar = new HealthBar("box", new Vector2(900f, 30f), this, new Vector2(GameWidth / 2, GameHeight / 20 * 19));
            hpBar.Display();

            //TelegraphLine t = new TelegraphLine(PI, 0, 0, 20, 2000, 9999, new Vector2(ScreenWidth / 2, 0), Color.White, "box", this, false);
            //TelegraphLine really = new TelegraphLine(PI / 2, 0, 0, 20, 2000, 9999, new Vector2(0 , ScreenHeight / 2), Color.White, "box", this, false);
        }

        public Vector2 CalculateArmPostionsRelativeToCentre(int expandedi)
        {
            return Utilities.RotateVectorClockwise(new Vector2(expandedi * Texture.Width / 1.4f, Texture.Height / 2.54f), Rotation) * DepthFactor();
        }
        public Vector2 CalculateArmPostions(int expandedi)
        {
            return Position + CalculateArmPostionsRelativeToCentre(expandedi);
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

            if (Health <= 0 && StartedDeathAnim == false)
            {
                StartedDeathAnim = true;
            }

            base.AI();

            if (Legs[0].Dead && Legs[1].Dead && !StartedPhaseTwoTransition)
            {
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
                Drawing.RestartSpriteBatchForShaders(spriteBatch, true);

                Effect boosterShader = AssetRegistry.GetShader("CrabRocketBoosterShader");
                boosterShader.CurrentTechnique.Passes[0].Apply();

                for (int i = 0; i < BoosterPositions.Length; i++)
                {
                    float width = MathHelper.Clamp(Abs(Velocity.Y) / 1f, 0f, 2.5f);
                    float height = Velocity.Length();
                    Texture2D texture = AssetRegistry.GetTexture2D("box");

                    Vector2 size = new Vector2(width, height);

                    Vector2 drawPos = BoosterPositions[i];
                    drawPos = drawPos - Utilities.RotateVectorClockwise(new Vector2(0f, texture.Height * (0.5f * height - 0.5f)), Rotation);

                    //drawPos.X = BoosterPositions[i].X + size.X;

                    //spriteBatch.Draw(Assets["box"], drawPos, null, Colour, Rotation + PI, originOffset, size, SpriteEffects.None, 1);
                    Drawing.BetterDraw(texture, drawPos, null, Colour, Rotation + PI, size, SpriteEffects.None, 1);
                }

                if (Shader is not null) //if we were already drawing stuff that had shaders
                    Drawing.RestartSpriteBatchForShaders(spriteBatch, true);
                else Drawing.RestartSpriteBatchForNotShaders(spriteBatch, true);
            }

            base.Draw(spriteBatch);


            //Utilities.drawTextInDrawMethod(Main.activeProjectiles.Count.ToString(), new Vector2(ScreenWidth / 4f * 3f, ScreenHeight / 4f * 3f), spriteBatch, font, Color.White);
            //Hitbox.DrawHitbox();

            foreach (CrabLeg leg in Legs)
            {
                if (leg is not null)
                {
                    //leg.DrawHitboxes();

                    //spriteBatch.Draw(AssetRegistry.GetTexture2D("box"), leg.PositionAtDistanceFromWrist(100f), null, Color.Red, Rotation + PI, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
                }
            }
        }

        public bool CanPerformCrabPunch()
        {
            return Utilities.DistanceBetweenVectors(player.Position, Position) > 500;
        }
    }
}
