﻿using Microsoft.Xna.Framework.Graphics;
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
        Spin
    }
    public class SwordWeapon : Weapon
    {
        public float WeaponRotation;
        public float SwingAngle => 2 * PI / 3;
        public float SwingDuration => 40;
        public float SpinDuration => 20;

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
            WeaponRotation = SwingAngle / 2;
            SwingStage = SwordSwingStages.Swing;
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
        public override void AI()
        {
            Trail.PreUpdate(3f, CalculateEnd(), Color.Orange);
            
            if (LeftClickReleased() && !Swinging)
            {
                Swinging = true;
                AITimer = 0;
                WeaponRotation = SwingAngle / 2f;
                SwingStage = SwordSwingStages.Swing;
                SwingDirection = (MousePositionWithCamera() - Owner.Position).ToAngle(); // lock in swing direction at start of swing
                HitEnemies.Clear();
            }

            if (Swinging)
            {
                if (SwingStage == SwordSwingStages.Swing)
                {
                    float interpolant = EasingFunctions.EaseOutExpo(MathHelper.Clamp(AITimer, 0, SwingDuration) / (float)SwingDuration);

                    WeaponRotation = MathHelper.Lerp(SwingAngle / 2, -SwingAngle / 2, interpolant);

                    if (AITimer == SwingDuration)
                    {
                        SwingStage = SwordSwingStages.Spin;
                        AITimer = 0;
                        HitEnemies.Clear();
                    }
                }
                else if (SwingStage == SwordSwingStages.Spin)
                {
                    int additionalTrailPoints = 9;

                    for (int i = 0; i < additionalTrailPoints; i++)
                    {
                        float interpolant = EasingFunctions.EaseOutExpo(MathHelper.Clamp(AITimer, 0, SpinDuration) / (float)SpinDuration);
                        float prevInterpolant = EasingFunctions.EaseOutExpo(MathHelper.Clamp(AITimer - 1, 0, SpinDuration) / (float)SpinDuration);
                        
                        // how bro feel after lerping lerps

                        float toUse = MathHelper.Lerp(prevInterpolant, interpolant, (float)i / additionalTrailPoints);

                        WeaponRotation = MathHelper.Lerp(-SwingAngle / 2, Tau, toUse);

                        Trail.AddPoint(CalculateEnd(WeaponRotation + SwingDirection));
                    }

                    // since the expo easing does a large leap rotation and kinda messes up the trail, add additional trail points
                    if (AITimer == SpinDuration)
                    {
                        AITimer = 0;
                        Swinging = false;
                        HitEnemies.Clear();
                        Hitbox.Clear();
                    }
                }

                WeaponRotation += SwingDirection; // face mouse
            }

            Trail.PostUpdate(CalculateEnd());
        }

        public override void PrimaryFire()
        {

        }

        public override void SecondaryFire()
        {

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
