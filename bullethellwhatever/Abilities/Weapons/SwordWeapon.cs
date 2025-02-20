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
        public float SpinDuration => 200;

        public SwordSwingStages SwingStage;

        public float SwingDirection; // direction to mouse
        public float Width => 15;
        public float Length => 60;

        public bool Swinging;

        public PrimitiveTrail Trail;
        public SwordWeapon(Player player, string iconTexture) : base(player, iconTexture)
        {
            Trail = new PrimitiveTrail(50, Owner.Width(), Owner.Position, Color.Orange);
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
            return Owner.Position - 0.9f * Utilities.RotateVectorClockwise(new Vector2(0f, Length), WeaponRotation);
        }
        public Vector2 CalculateEnd(float rotation)
        {
            return Owner.Position - 0.9f * Utilities.RotateVectorClockwise(new Vector2(0f, Length), rotation);
        }
        public override void AI()
        {
            Trail.PreUpdate(10f, CalculateEnd(), Color.Orange);

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
                    //PastEnds.Add(CalculateEnd());

                    float interpolant = EasingFunctions.EaseOutExpo(MathHelper.Clamp(AITimer, 0, SpinDuration) / (float)SpinDuration);

                    float rotAtStartOfFrame = WeaponRotation;
                    int additionalTrailPoints = 3;

                    WeaponRotation = MathHelper.Lerp(-SwingAngle / 2, Tau, interpolant);

                    for (int i = 1; i < additionalTrailPoints + 1; i++)
                    {
                        float lerpedAngle = MathHelper.Lerp(rotAtStartOfFrame.WithinTau(), WeaponRotation.WithinTau(), i / (float)additionalTrailPoints);
                        //Trail.AddPoint(CalculateEnd(lerpedAngle));
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
                Trail.Draw(s);
                Drawing.BetterDraw("box", Owner.Position, null, Color.Orange, WeaponRotation + PI, new Vector2(Width / 10f, Length / 10f), SpriteEffects.None, 0f, new Vector2(5f, 0f));
                //Drawing.DrawBox(CalculateEnd(), Color.Green, 1f);
                //DrawHitbox();
            }
        }
    }
}
