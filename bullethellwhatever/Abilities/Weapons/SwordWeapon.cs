using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using bullethellwhatever.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses.Entities;
using bullethellwhatever.DrawCode;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.NPCs;
using System.Windows.Forms;
using bullethellwhatever.AssetManagement;

namespace bullethellwhatever.Abilities.Weapons
{
    public enum SwordSwingStages
    {
        Prepare,
        Swing,
    }
    public class SwordWeapon : Weapon
    {
        public float WeaponRotation;

        public float PullBackAngle => -4 * PI / 5;
        public float SwingAngle => -PullBackAngle * 2;

        public int SwingDuration => 10;
        public int ChargeDuration => 30;
        public int ChargeTimer = 0;

        public SwordSwingStages SwingStage;

        public float SwingDirection; // direction to mouse
        public float Width => 22.5f;
        public float Length => 90f;

        public bool Swinging;

        public PrimitiveTrail Trail;
        public SwordWeapon(Player player, string iconTexture) : base(player, iconTexture)
        {
            Trail = new PrimitiveTrail(50, Owner.Width(), Color.Orange);
        }
        public override void WeaponInitialise()
        {
            WeaponRotation = 0f;
            SwingStage = SwordSwingStages.Prepare;
            AITimer = 0;
            Swinging = false;
        }
        public override bool CanSwitchWeapon()
        {
            return !Swinging;
        }

        public Vector2 CalculateEnd()
        {
            return CalculateEnd(WeaponRotation);
        }
        public Vector2 CalculateEnd(float angle)
        {
            return Owner.Position - 0.92f * Utilities.RotateVectorClockwise(new Vector2(0f, Length), angle);
        }

        public void Reset()
        {
            AITimer = 0;
            WeaponRotation = 0f;
            ChargeTimer = 0;
            SwingDirection = (MousePositionWithCamera() - Owner.Position).ToAngle(); // lock in swing direction at start of swing
            HitEnemies.Clear();
            Swinging = false;
            SwingStage = SwordSwingStages.Prepare;
        }
        public override void AI()
        {
            Trail.PreUpdate(3f, CalculateEnd(), Color.Orange);

            if (SwingStage == SwordSwingStages.Prepare)
            {
                if (IsLeftClickDown() || true)
                {
                    ChargeTimer++;
                    Swinging = true;
                }
                else
                {
                    Swinging = false;
                }

                //if (ChargeTimer >= 0 && !IsLeftClickDown())
                //{
                //    ChargeTimer = 0;
                //    Swinging = false;
                //}

                if (Charged())
                {
                    SwingStage = SwordSwingStages.Swing;
                    ChargeTimer = 0;
                    AITimer = 0;
                }

                float interpolant = EasingFunctions.EaseOutCubic((float)ChargeTimer / ChargeDuration);
                WeaponRotation = MathHelper.Lerp(0f, PullBackAngle, interpolant);
            }

            else if (SwingStage == SwordSwingStages.Swing)
            {
                WeaponRotation = MathHelper.Lerp(PullBackAngle, SwingAngle, EasingFunctions.EaseOutExpo((float)AITimer / SwingDuration));

                if (AITimer == SwingDuration + 1)
                {
                    Reset();
                }
            }

            WeaponRotation += SwingDirection; // face mouse

            Trail.PostUpdate(CalculateEnd());
        }

        public override void PrimaryFire()
        {

        }

        public override void SecondaryFire()
        {

        }

        public bool Charged()
        {
            return ChargeTimer == ChargeDuration;
        }
        public override void OnHit(NPC npc)
        {
            // to do: centralised damage system

            DealDamage(npc, 5f);
            HitEnemies.Add(npc);

            Drawing.ScreenShake(10, 3);
        }
        public override void UpdateHitbox()
        {
            if (Swinging)
            {
                Hitbox = Utilities.FillRectWithCircles(Owner.Position + 0.5f * (CalculateEnd() - Owner.Position), (int)Width, (int)Length, WeaponRotation);
            }
        }

        public override void Draw(SpriteBatch s)
        {
            if (Swinging)
            {
                Texture2D texture = AssetRegistry.GetTexture2D("SwordWeapon");
                Trail.Draw(s);
                //swap height and width when scaling because sprite is sideways
                float xscale = Width / texture.Width;
                float yscale = Length / texture.Height;
                //xscale = 1;
                //yscale = 1;
                Drawing.BetterDraw(texture, Owner.Position, null, Color.Orange, WeaponRotation, new Vector2(xscale, yscale), SpriteEffects.None, 0f, new Vector2(Width / 2 / xscale, Length / yscale));
                //Drawing.DrawBox(CalculateEnd(), Color.Green, 1f);
                //DrawHitbox();
            }
        }
    }
}
