﻿using bullethellwhatever.BaseClasses;
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
using System.Diagnostics;

namespace bullethellwhatever.Bosses.CrabBoss
{
    
    public class CrabBoss : Boss
    {
        public CrabArm[] Arms;
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
            ArmPositionsOnBody = new Vector2[2];
            ArmRestingEnds = new Vector2[2];
        }

        public override void Spawn(Vector2 position, Vector2 velocity, float damage, string texture, Vector2 size, float MaxHealth, int pierceToTake, Color colour, bool shouldRemoveOnEdgeTouch, bool harmfulToPlayer, bool harmfulToEnemy)
        {
            base.Spawn(position, velocity, damage, texture, size, MaxHealth, pierceToTake, colour, shouldRemoveOnEdgeTouch, harmfulToPlayer, harmfulToEnemy);

            for (int i = 0; i < 2; i++)
            {
                int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                Vector2 pos = CalculateArmPostions(expandedi);

                Arms[i] = new CrabArm(pos, this, i, ScaleFactor / BodyToArmSizeRatio);

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

            CurrentAttack = new CrabGrab(this);
            ContactDamage = true;
            HealthBar hpBar = new HealthBar("box", new Vector2(900f, 30f), this, new Vector2(GameWidth / 2, GameHeight / 20 * 19));
            hpBar.DisplayPercentage = true;
            hpBar.Display();
        }
        public Vector2 CalculateArmPostionsRelativeToCentre(int expandedi)
        {
            return Utilities.RotateVectorClockwise(new Vector2(expandedi * Texture.Width * GetSize().X / 2.1f, Texture.Height * GetSize().Y / 3.6f), Rotation);
        }
        public Vector2 CalculateArmPostions(int expandedi)
        {
            return Position + CalculateArmPostionsRelativeToCentre(expandedi);
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
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            for (int i = 0; i < 2; i++)
            {
                //Drawing.DrawBox(Arms[i].RestPositionEnd(), Color.Red, 1f);
                //Drawing.DrawBox(Arms[i].LowerArm.CalculateEnd(), Color.Green, 1f);
            }

            //Utilities.drawTextInDrawMethod(Arms[0].LowerArm.RotationFromV().ToString(), Position + new Vector2(0f, 200f), spriteBatch, font, Color.White);
        }

        public override void AI()
        {
            base.AI();
            //Velocity = Vector2.Zero;
            //Rotation = 0f;
        }

        public void ResetArmRotations()
        {
            for (int i = 0; i < 2; i++)
            {
                //int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                Arms[i].ResetRotations();
            }
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
    }
}
