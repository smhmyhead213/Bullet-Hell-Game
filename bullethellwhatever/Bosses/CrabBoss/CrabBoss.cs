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
        public CrabArm[] Arms;
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

            float scaleFactor = 1.8f;
            float bodyToArmSizeRatio = 1.5f; // adjust to change body/arm proportions

            Size = Vector2.One * scaleFactor;
            Position = Utilities.CentreWithCamera() - new Vector2(0f, GameHeight / 4f);
            MaxHP = 400f;

            Colour = Color.White;
            
            Phase = 1;

            StartedPhaseTwoTransition = false;

            BoostersActive = false;
            LockArmPositions = true;
            PlayerSaidOpeningDialogue = false;
            
            Arms = new CrabArm[2];
            ArmPositionsOnBody = new Vector2[2];
            BoosterPositions = new Vector2[2];

            StartedDeathAnim = false;

            for (int i = 0; i < 2; i++)
            {
                int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                Vector2 pos = CalculateArmPostions(expandedi);

                Arms[i] = new CrabArm(pos, this, i, scaleFactor / bodyToArmSizeRatio);

                ArmPositionsOnBody[i] = pos;

                if (i == 0)
                {
                    Arms[i].HorizontalFlip = true;
                }

                Arms[i].UpperArm.RotationConstant = -expandedi * PI / 12;
                Arms[i].LowerArm.RotationConstant = expandedi * PI / 12;
                //Legs[0].UpperClaw.RotationConstant = expandedi * PI;
                //Legs[0].LowerClaw.RotationConstant = expandedi * PI; // fix later

            }

            CurrentAttack = new CrabIKTest(this);

            HealthBar hpBar = new HealthBar("box", new Vector2(900f, 30f), this, new Vector2(GameWidth / 2, GameHeight / 20 * 19));
            hpBar.Display();

            //TelegraphLine t = new TelegraphLine(PI, 0, 0, 20, 2000, 9999, new Vector2(ScreenWidth / 2, 0), Color.White, "box", this, false);
            //TelegraphLine really = new TelegraphLine(PI / 2, 0, 0, 20, 2000, 9999, new Vector2(0 , ScreenHeight / 2), Color.White, "box", this, false);
        }

        public Vector2 CalculateArmPostionsRelativeToCentre(int expandedi)
        {
            return Utilities.RotateVectorClockwise(new Vector2(expandedi * Texture.Width * GetSize().X / 2.1f, Texture.Height * GetSize().Y / 3.6f), Rotation);
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

                Arms[i].ResetRotations();
            }
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

            if (Arms[0].Dead && Arms[1].Dead && !StartedPhaseTwoTransition)
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

        public override void PostUpdate()
        {
            base.PostUpdate();

            for (int i = 0; i < 2; i++)
            {
                Arms[i].Update();

                int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                if (Arms[i] is not null)
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

            foreach (CrabArm leg in Arms)
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
