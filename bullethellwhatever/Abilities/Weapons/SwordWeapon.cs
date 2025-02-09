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
        public List<Entity> HitEnemies; // hold enemies hit to prevent multihits

        public float WeaponRotation;
        public float SwingAngle => 2 * PI / 3;

        public float SwingDuration => 40;
        public float WindUpDuration => 20;
        public float SpinDuration => 20;

        public SwordSwingStages SwingStage;

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
        public override void AI()
        {
            Trail.PreUpdate(10f, CalculateEnd(), Color.Orange);

            if (LeftClickReleased() && !Swinging)
            {
                Swinging = true;
                AITimer = 0;
                WeaponRotation = SwingAngle / 2f;
                SwingStage = SwordSwingStages.Swing;
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
                    }
                }
                else if (SwingStage == SwordSwingStages.Spin)
                {
                    float interpolant = EasingFunctions.EaseOutExpo(MathHelper.Clamp(AITimer, 0, SpinDuration) / (float)SpinDuration);

                    WeaponRotation = MathHelper.Lerp(-SwingAngle / 2, Tau, interpolant);

                    if (AITimer == SpinDuration)
                    {
                        AITimer = 0;
                        Swinging = false;
                    }
                }
            }

            Trail.PostUpdate(CalculateEnd());
        }

        public override void PrimaryFire()
        {

        }

        public override void SecondaryFire()
        {

        }

        public override void Draw(SpriteBatch s)
        {
            if (Swinging)
            {
                Trail.Draw(s);
                Drawing.BetterDraw("box", Owner.Position, null, Color.Orange, WeaponRotation + PI, new Vector2(Width / 10f, Length / 10f), SpriteEffects.None, 0f, new Vector2(5f, 0f));
            }
        }
    }
}
