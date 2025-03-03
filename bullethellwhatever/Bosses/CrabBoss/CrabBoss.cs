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
        public Vector2[] ArmPositionsOnBody;
        public Vector2[] ArmRestingEnds;

        public int Phase; // flag for if arms are detached yet

        public const float ScaleFactor = 1.5f;
        public const float BodyToArmSizeRatio = 1.5f; // adjust to change body/arm proportions
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

                Vector2 wristRestingOffset = new Vector2(0f, Arms[i].WristLength() * 0.95f);
                Arms[i].TouchPoint(pos + wristRestingOffset);

                // store a vector from the arm start position to its end
                ArmRestingEnds[i] = Arms[i].LowerArm.CalculateEnd() - pos;
            }

            CurrentAttack = new CrabIntro(this);

            HealthBar hpBar = new HealthBar("box", new Vector2(900f, 30f), this, new Vector2(GameWidth / 2, GameHeight / 20 * 19));
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

            DrawHitbox();
        }
        public void ResetArmRotations()
        {
            for (int i = 0; i < 2; i++)
            {
                //int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                Arms[i].ResetRotations();
            }
        }

        public void FacePlayer()
        {
            Rotation = Utilities.VectorToAngle(Position - player.Position);
        }

        public override void PostUpdate()
        {
            for (int i = 0; i < 2; i++)
            {
                Arms[i].Update();
            }

            base.PostUpdate();
        }
        public override void Die()
        {
            base.Die();
        }

        public bool CanPerformCrabPunch()
        {
            return Utilities.DistanceBetweenVectors(player.Position, Position) > 500;
        }
    }
}
