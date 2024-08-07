﻿using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses.Hitboxes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode.UI.Player
{
    public class PlayerHUD : UIElement
    {
        public static float WeaponIconsRotationToAdd;
        public static float PermanentIconRotation;

        public PlayerHUD(string texture, Vector2 size, Vector2 position) : base(texture, size, position)
        {

        }

        public void ResetHUD()
        {
            PermanentIconRotation = 0;
            WeaponIconsRotationToAdd = 0;
        }
        public override void Draw(SpriteBatch s)
        {
            Drawing.RestartSpriteBatchForShaders(s);

            RotatedRectangle hudBox = new RotatedRectangle(0, Texture.Width, Texture.Height, Position, player);

            hudBox.UpdateVertices();

            float opacity = player.Hitbox.Intersects(hudBox).Collided ? 0.2f : 1f;

            AssetRegistry.GetShader("PlayerHealthBarShader").Parameters["hpRatio"]?.SetValue(player.Health / player.MaxHP);

            AssetRegistry.GetShader("PlayerHealthBarShader").CurrentTechnique.Passes[0].Apply();

            //UI.DrawHealthBar(_spriteBatch, player, new Vector2(ScreenWidth / 7.6f, ScreenHeight / 8.8f), 12.6f, 0.7f);

            Drawing.BetterDraw(AssetRegistry.GetTexture2D("box"), new Vector2(IdealScreenWidth / 7.6f, IdealScreenHeight / 8.8f), null, Color.White * opacity, 0, new Vector2(12.6f, 0.7f), SpriteEffects.None, 0);

            Drawing.RestartSpriteBatchForNotShaders(s);

            Drawing.BetterDraw(Texture, new Vector2(IdealScreenWidth / 10f, IdealScreenHeight / 10f), null, Color.White * opacity, 0, Vector2.One, SpriteEffects.None, 1);

            //---------------------- handle rotating weapon icons --------------------------

            Vector2 iconRotationAxis = new Vector2(IdealScreenWidth / 15.174f, IdealScreenHeight / 10f);

            Vector2 drawDistanceFromCentre = new Vector2(0, -30f);

            //Drawing.BetterDraw(Assets["box"], iconRotationAxis, null, Color.Red, 0, Vector2.One, SpriteEffects.None, 1);

            float numberOfWeapons = 3;

            Vector2 iconSize = Vector2.One * 0.6f;

            if (MainInstance.IsActive)
            {
                WeaponIconsRotationToAdd = (((int)player.ActiveWeapon - (int)player.PreviousWeapon) * Tau / numberOfWeapons / player.WeaponSwitchCooldown) * player.WeaponSwitchCooldownTimer;

                PermanentIconRotation = PermanentIconRotation + WeaponIconsRotationToAdd;

                while (PermanentIconRotation > Tau)
                {
                    PermanentIconRotation = PermanentIconRotation - Tau; // keep within one full turn so we dont go crazy
                }

                while (PermanentIconRotation < -Tau)
                {
                    PermanentIconRotation = PermanentIconRotation + Tau; // keep within one full turn so we dont go crazy
                }
            }

            Drawing.BetterDraw(AssetRegistry.GetTexture2D("HomingWeaponIcon"), iconRotationAxis + Utilities.RotateVectorClockwise(drawDistanceFromCentre, 0 * Tau / numberOfWeapons + PermanentIconRotation), null, Color.White * opacity, 0, iconSize, SpriteEffects.None, 1);
            Drawing.BetterDraw(AssetRegistry.GetTexture2D("MachineWeaponIcon"), iconRotationAxis + Utilities.RotateVectorClockwise(drawDistanceFromCentre, 1 * Tau / numberOfWeapons + PermanentIconRotation), null, Color.White * opacity, 0, iconSize, SpriteEffects.None, 1);
            Drawing.BetterDraw(AssetRegistry.GetTexture2D("LaserWeaponIcon"), iconRotationAxis + Utilities.RotateVectorClockwise(drawDistanceFromCentre, 2 * Tau / numberOfWeapons + PermanentIconRotation), null, Color.White * opacity, 0, iconSize, SpriteEffects.None, 1);
        }
    }
}
