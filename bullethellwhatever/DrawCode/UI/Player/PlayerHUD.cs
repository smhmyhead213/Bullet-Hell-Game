using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses.Hitboxes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using bullethellwhatever.UtilitySystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.Abilities.Weapons;

namespace bullethellwhatever.DrawCode.UI.Player
{
    public class PlayerHUD : UIElement
    {
        public static float WeaponHUDRotation;
        public static float TotalAngleToRotate;

        public PlayerHUD(string texture, Vector2 size, Vector2 position) : base(texture, size, position)
        {
            ResetHUD();
            Interactable = false;
        }

        public void ResetHUD()
        {
            WeaponHUDRotation = 0;
        }
        public void BeginRotation()
        {
            // this code is useless at the moment 

            int activeIndex = PlayerWeaponManager.ActiveWeaponIndex;
            int lastIndex = PlayerWeaponManager.LastWeaponIndex;
            int numberOfWeapons = PlayerWeaponManager.AvailableWeapons.Length;
            // calculate the angle the weapon HUD should be at so that the currently selected weapon is at the top.

            float targetAngle = activeIndex * Tau / 3f;

            // calculate the signed "difference" between the current and previous weapons.
            int differenceLeft = (activeIndex - lastIndex) % numberOfWeapons;
            int differenceRight = (lastIndex - activeIndex) % numberOfWeapons;

            // % in c# isn't the modulo operation, its the remainder. adding the number of weapons brings the difference into the range we want it.

            if (differenceLeft < 0)
            {
                differenceLeft += numberOfWeapons;
            }

            if (differenceRight < 0)
            {
                differenceRight += numberOfWeapons;
            }

            int indexDifference = (int)Min(differenceLeft, differenceRight);

            int rotationDirection = 0;

            if (differenceLeft > differenceRight)
            {
                rotationDirection = -1;
            }
            else
            {
                rotationDirection = 1;
            }

            TotalAngleToRotate = rotationDirection * indexDifference * Tau / numberOfWeapons;
        }

        public override void Update()
        {
            base.Update();

            //if (PlayerWeaponManager.WeaponSwitchCooldownTimer > 0)
            //{
            //    WeaponHUDRotation += TotalAngleToRotate / PlayerWeaponManager.WeaponSwitchCooldown;
            //}

            int numberOfWeapons = PlayerWeaponManager.AvailableWeapons.Length;
            float lastWeaponAngle = Tau / numberOfWeapons * PlayerWeaponManager.LastWeaponIndex;
            float activeWeaponAngle = Tau / numberOfWeapons * PlayerWeaponManager.ActiveWeaponIndex;

            float interpolant = 1f - (float)PlayerWeaponManager.WeaponSwitchCooldownTimer / PlayerWeaponManager.WeaponSwitchCooldown;

            // decide on the smallest angle that will reach the target (to do: minimise rotation)

            float smallestAngle = Utilities.SmallestAngleTo(lastWeaponAngle, activeWeaponAngle);

            if (smallestAngle == activeWeaponAngle - lastWeaponAngle)
            {
                WeaponHUDRotation = MathHelper.LerpPrecise(lastWeaponAngle, activeWeaponAngle, EasingFunctions.EaseOutQuad(interpolant));
            }
            else
            {
                // if the difference is not the optimal angle, adjust the value to lerp towards
                if (activeWeaponAngle < lastWeaponAngle)
                {
                    WeaponHUDRotation = MathHelper.LerpPrecise(lastWeaponAngle, activeWeaponAngle + Tau, EasingFunctions.EaseOutQuad(interpolant));
                }
                else
                {
                    WeaponHUDRotation = MathHelper.LerpPrecise(lastWeaponAngle + Tau, activeWeaponAngle, EasingFunctions.EaseOutQuad(interpolant));
                }
            }
        }

        public override void Draw(SpriteBatch s)
        {
            Drawing.RestartSpriteBatchForShaders(s, false);

            float opacity = 1f;

            Effect hpBarShader = AssetRegistry.GetEffect("PlayerHealthBarShader");

            hpBarShader.Parameters["hpRatio"]?.SetValue(player.Health / player.MaxHP);

            hpBarShader.CurrentTechnique.Passes[0].Apply();

            Drawing.BetterDraw(AssetRegistry.GetTexture2D("box"), new Vector2(GameWidth / 7.6f, GameHeight / 8.8f), null, Color.White * opacity, 0, new Vector2(12.6f, 0.7f), SpriteEffects.None, 0);

            Drawing.RestartSpriteBatchForShaders(s, false);

            Drawing.BetterDraw(Texture, new Vector2(GameWidth / 10f, GameHeight / 10f), null, Color.White * opacity, 0, Vector2.One, SpriteEffects.None, 1);

            //---------------------- handle rotating weapon icons --------------------------

            Vector2 iconRotationAxis = new Vector2(GameWidth / 15.174f, GameHeight / 10f);

            Vector2 drawDistanceFromCentre = new Vector2(0, -30f);

            //Drawing.BetterDraw(Assets["box"], iconRotationAxis, null, Color.Red, 0, Vector2.One, SpriteEffects.None, 1);

            Vector2 iconSize = Vector2.One * 0.6f;

            int numberOfWeapons = PlayerWeaponManager.AvailableWeapons.Length;

            for (int i = 0; i < numberOfWeapons; i++)
            {
                Drawing.BetterDraw(PlayerWeaponManager.AvailableWeapons[i].IconHUD, iconRotationAxis + Utilities.RotateVectorClockwise(drawDistanceFromCentre, -i * Tau / numberOfWeapons + WeaponHUDRotation), null, Color.White * opacity, 0, iconSize, SpriteEffects.None, 1);
            }
        }
    }
}
