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
using bullethellwhatever.Bosses.CrabBoss.Attacks.DoubleArmSlam;
using System.Diagnostics;
using bullethellwhatever.Bosses.CrabBoss.Attacks;
using System.Buffers;
using bullethellwhatever.UtilitySystems;

namespace bullethellwhatever.Bosses.CrabBoss
{
    
    public partial class CrabBoss : Boss
    {
        public CrabArm[] Arms;
        public CrabArmBehaviour[] ArmBehaviours;

        public Vector2[] ArmPositionsOnBody;
        public Vector2[] ArmRestingEnds
        {
            get;
            set;
        }

        public int Phase; // flag for if arms are detached yet

        public const float ScaleFactor = 1.5f;
        public const float BodyToArmSizeRatio = 1.5f; // adjust to change body/arm proportions

        public const int GrabbingArm = 1;
        public const int GrabPunishArm = 0;

        public const int ScuttleLegsOnEachSide = 4;
        public int ScuttleLegCount => ScuttleLegsOnEachSide * 2;
        public Vector2[][] ScuttleLegPositions;
        public Vector2[][] ScuttleLegTargetPositions;
        public int ScuttlerUpperPartLength = 30;
        public int ScuttlerLowerPartLength = 20;
        public int ScuttlerLength => ScuttlerUpperPartLength + ScuttlerLowerPartLength;
        public override float Rotation
        { 
            get => base.Rotation;
            set { 
                base.Rotation = value;
                PerformAdjustments();
            }
        }
        public CrabBoss()
        {
            Texture = AssetRegistry.GetTexture2D("CrabBody");

            Scale = Vector2.One * ScaleFactor;

            Position = Utilities.CentreWithCamera() - new Vector2(0f, GameHeight / 4f);
            MaxHP = 400f;

            Colour = Color.White;
            
            Phase = 1;

            Arms = new CrabArm[2];
            ArmBehaviours = new CrabArmBehaviour[2];
            ArmPositionsOnBody = new Vector2[2];
            ArmRestingEnds = new Vector2[2];

            ScuttleLegPositions = new Vector2[2][];
            ScuttleLegTargetPositions = new Vector2[2][];

            for (int i = 0; i < ScuttleLegPositions.Length; i++)
            {
                ScuttleLegPositions[i] = new Vector2[ScuttleLegsOnEachSide];
                ScuttleLegTargetPositions[i] = new Vector2[ScuttleLegsOnEachSide];
            }
        }

        public override void Spawn(Vector2 position, Vector2 velocity, float damage, string texture, Vector2 size, float MaxHealth, int pierceToTake, Color colour, bool shouldRemoveOnEdgeTouch, bool harmfulToPlayer, bool harmfulToEnemy)
        {
            base.Spawn(position, velocity, damage, texture, size, MaxHealth, pierceToTake, colour, shouldRemoveOnEdgeTouch, harmfulToPlayer, harmfulToEnemy);

            for (int i = 0; i < 2; i++)
            {
                int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                Vector2 pos = CalculateArmPostions(expandedi);

                Arms[i] = new CrabArm(pos, this, i, DefaultArmScale());

                ArmPositionsOnBody[i] = pos;

                if (i == 0)
                {
                    Arms[i].HorizontalFlip = true;
                }

                float wristLength = Arms[i].WristLength();
                Vector2 wristRestingOffset = new Vector2(0f, wristLength * 0.95f);
                Arms[i].TouchPoint(pos + wristRestingOffset, false);

                // store a vector from the arm start position to its end
                Vector2 lowerArmEnd = Arms[i].LowerArm.CalculateEnd();
                ArmRestingEnds[i] = lowerArmEnd - pos;
            }

            CurrentAttack = new CrabDoubleArmSmash(this);
            ContactDamage = true;
            DisplayBossHPBar();
            UpdateLegPositions();
        }
        public Vector2 CalculateArmPostionsRelativeToCentre(int expandedi)
        {
            return Utilities.RotateVectorClockwise(new Vector2(expandedi * Texture.Width * GetScale().X / 2.1f, Texture.Height * GetScale().Y / 3.6f), Rotation);
        }
        public Vector2 CalculateArmPostions(int expandedi)
        {
            return Position + CalculateArmPostionsRelativeToCentre(expandedi);
        }

        public float DefaultArmScale()
        {
            return ScaleFactor / BodyToArmSizeRatio;
        }

        public override void UpdateHitbox()
        {
            base.UpdateHitbox();

            foreach (CrabArm crabArm in Arms)
            {
                if (crabArm == null) return;

                foreach (CrabBossAppendage crabBossAppendage in crabArm.ArmParts)
                {
                    crabBossAppendage.UpdateHitbox();
                    Hitbox.Add(crabBossAppendage.LimbHitbox());
                }
            }
        }

        public override void AI()
        {
            base.AI();
            
            foreach (CrabArmBehaviour armBehaviour in ArmBehaviours)
            {
                //armBehaviour.Execute();
            }
        }

        public void ResetArmRotations()
        {
            for (int i = 0; i < 2; i++)
            {
                Arms[i].ResetRotations();
            }
        }

        public void AssumeKeyframe(CrabArmKeyframe keyframe)
        {
            // loop each arm
            for (int arm = 0; arm < keyframe.ArmPartScales.Length; arm++)
            {
                for (int i = 0; i < keyframe.ArmPartScales[arm].Length; i++)
                {
                    // scales and arms have same dimensions so we can do both in one loop
                    Arms[arm].ArmParts[i].Scale = keyframe.ArmPartScales[arm][i];
                    Arms[arm].ArmParts[i].RotationToAdd = keyframe.ArmPartRotations[arm][i];
                }
            }
        }

        public CrabArmKeyframe CurrentStance()
        {
            return new CrabArmKeyframe(this);
        }

        public void LerpToFacePlayer(float rotateFraction = 0.2f)
        {
            Vector2 toPlayer = player.Position - Position;
            float angleToPlayer = Utilities.VectorToAngle(toPlayer);

            float rotationToPlayer = Utilities.SmallestAngleTo(Rotation + PI, Utilities.AngleToPlayerFrom(Position));

            Rotation += rotateFraction * rotationToPlayer;

            //Vector2 toPlayer = Position - player.Position;
            //float angleToPlayer = Utilities.VectorToAngle(toPlayer);
            //Rotation = Utilities.LerpRotation(Rotation, angleToPlayer, interpolant);
        }
        public void FacePlayer()
        {
            Rotation = Utilities.VectorToAngle(Position - player.Position);            
        }

        public void Rotate(float angle)
        {
            Rotation += angle;
            PerformAdjustments(); // update arms immediately
        }

        public void UpdateLegPositions()
        {
            for (int i = 0; i < ScuttleLegPositions.Length; i++)
            {
                int expandedi = Utilities.ExpandedIndex(i);

                for (int j = 0; j < ScuttleLegPositions[i].Length; j++)
                {
                    float startingHeight = -Height() / 2f;
                    float offsetFromCentreY = startingHeight + j * Height() / (ScuttleLegsOnEachSide - 1);

                    // if you wanna flip everything round put a negative on expandedi
                    ScuttleLegPositions[i][j] = Position + new Vector2(expandedi * Width() / 2f, offsetFromCentreY).Rotate(Rotation);
                    ScuttleLegTargetPositions[i][j] = ScuttleLegPositions[i][j] + (ScuttlerLength - 15) * (expandedi * PI / 2 + Rotation).ToVector();
                }
            }
        }

        public override void PerformAdjustments()
        {
            base.PerformAdjustments();

            for (int i = 0; i < 2; i++)
            {
                Arms[i].UpdatePositions();
            }

            for (int i = 0; i < 2; i++)
            {
                Arms[i].UpdateAppendages();
            }
        }
        public override void PostUpdate()
        {
            base.PostUpdate();
            UpdateLegPositions();
        }
        public override void Die()
        {
            base.Die();

            foreach (CrabArm arm in Arms)
            {
                foreach (CrabBossAppendage crabBossAppendage in arm.ArmParts)
                {
                   crabBossAppendage.Die();
                }
            }
        }

        public bool CanPerformCrabPunch()
        {
            return Utilities.DistanceBetweenVectors(player.Position, Position) > 500;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //DrawHitbox();

            for (int i = 0; i < ScuttleLegPositions.Length; i++)
            {
                for (int j = 0; j < ScuttleLegPositions[i].Length; j++)
                {
                    Vector2 elbowPosition;

                    int expandedi = Utilities.ExpandedIndex(i);

                    float[] rotations = MathsUtils.SolveTwoPartIK(ScuttleLegPositions[i][j], ScuttleLegTargetPositions[i][j], ScuttlerUpperPartLength, ScuttlerLowerPartLength, out elbowPosition, -expandedi);

                    Texture2D texture = AssetRegistry.GetTexture2D("box");
                    float scuttlerWidth = 10f;
                    Vector2 upperPartScale = new Vector2(scuttlerWidth / texture.Width, ScuttlerUpperPartLength / texture.Height);
                    Vector2 lowerPartScale = new Vector2(scuttlerWidth / texture.Width, ScuttlerLowerPartLength / texture.Height);

                    Drawing.BetterDraw(texture, ScuttleLegPositions[i][j], null, Color.White, rotations[0] + PI, upperPartScale, SpriteEffects.None, 0f, new Vector2(texture.Width / 2f, 0f));
                    Drawing.BetterDraw(texture, elbowPosition, null, Color.White, rotations[1] + PI, lowerPartScale, SpriteEffects.None, 0f, new Vector2(texture.Width / 2f, 0f));

                    Drawing.DrawBox(ScuttleLegPositions[i][j], Color.Red, 1f);
                    Drawing.DrawBox(ScuttleLegTargetPositions[i][j], Color.Yellow, 1f);
                    Drawing.DrawBox(elbowPosition, Color.Orange, 1f);
                }
            }
        }
    }
}
