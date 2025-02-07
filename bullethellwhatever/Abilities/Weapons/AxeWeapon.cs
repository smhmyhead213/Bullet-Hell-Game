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
    public class AxeWeapon : Weapon
    {
        public List<Entity> HitEnemies; // hold enemies hit to prevent multihits

        public float WeaponRotation;
        public float SwingAngle => 2 * PI / 3;

        public float SwingDuration => 30;
        public float WindUpDuration => 15;

        public float SwingDirection;

        public bool Swinging;
        public AxeWeapon(Player player, string iconTexture) : base(player, iconTexture)
        {

        }
        public override void WeaponInitialise()
        {
            WeaponRotation = 0;
            SwingDirection = 1;
            AITimer = 0;
            Swinging = false;
        }
        public override bool CanSwitchWeapon()
        {
            return !Swinging;
        }
        public override void AI()
        {
            if (LeftClickReleased() && !Swinging)
            {
                Swinging = true;
                AITimer = 0;
            }

            if (Swinging)
            {
                // swing through
                if (SwingDirection == 1)
                {
                    float interpolant = EasingFunctions.Linear(MathHelper.Clamp(AITimer, 0, WindUpDuration) / (float)WindUpDuration);

                    WeaponRotation = interpolant * SwingAngle / 2;

                    if (AITimer == WindUpDuration)
                    {
                        SwingDirection = -1;
                        AITimer = 0;
                    }
                }
                else if (SwingDirection == -1)
                {
                    float interpolant = EasingFunctions.EaseOutExpo(MathHelper.Clamp(AITimer - WindUpDuration, 0, SwingDuration) / (float)SwingDuration);

                    WeaponRotation = MathHelper.Lerp(SwingAngle / 2, -SwingAngle / 2, interpolant);

                    if (AITimer == SwingDuration + WindUpDuration)
                    {
                        SwingDirection = 1;
                        Swinging = false;
                    }
                }
            }
        }

        public override void PrimaryFire()
        {

        }

        public override void SecondaryFire()
        {

        }

        public override void Draw(SpriteBatch s)
        {
            Drawing.BetterDraw("box", Owner.Position, null, Color.Orange, WeaponRotation + PI, new Vector2(1f, 3f), SpriteEffects.None, 0f, new Vector2(5f, 0f));
        }
    }
}
