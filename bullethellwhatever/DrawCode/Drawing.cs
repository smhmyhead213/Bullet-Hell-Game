using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.UtilitySystems;
using System.Diagnostics.Contracts;
using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.MainFiles;
using bullethellwhatever.AssetManagement;
using System.Runtime.CompilerServices;

namespace bullethellwhatever.DrawCode
{
    public static class Drawing
    {
        public static bool AreButtonsDrawn;

        public static float ScreenShakeMagnitude;
        public static int ScreenShakeTimer;
        public static bool IsScreenShaking => ScreenShakeTimer > 0;
        public static int Timer;

        public static SpriteBatch PreviousSpriteBatch;
        public static void Initialise()
        {

        }

        public static void UpdateDrawer()
        {
            if (!Utilities.ImportantMenusPresent() && MainInstance.IsActive)
            {
                ScreenShakeTimer--;
            }
        }
        public static void RestartSpriteBatchForUI(SpriteBatch s)
        {
            s.End();
            s.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointWrap);
        }
        public static void RestartSpriteBatchForShaders(SpriteBatch s)
        {
            s.End();
            s.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.LinearWrap, transformMatrix: MainCamera.Matrix);
        }

        public static void RestartSpriteBatchForNotShaders(SpriteBatch s)
        {
            s.End();
            s.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointWrap, transformMatrix: MainCamera.Matrix);
        }

        public static void BetterDraw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 scale, SpriteEffects spriteEffects, float layerDepth, Vector2? origin = null)
        {
            //This method exists so that one does not have to repeat the same paraemters for stuff like origin offsets and screenshake offset.

            Vector2 finalOrigin = origin is null ? new Vector2(texture.Width / 2, texture.Height / 2) : origin.Value;

            _spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, finalOrigin, scale, spriteEffects, layerDepth);
        }
        public static void BetterDraw(ManagedTexture texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 scale, SpriteEffects spriteEffects, float layerDepth, Vector2? origin = null)
        {
            BetterDraw(texture.Asset, position, sourceRectangle, color, rotation, scale, spriteEffects, layerDepth, origin);
        }
        public static void DrawTelegraphs(Entity entity)
        {
            foreach (TelegraphLine telegraphLine in entity.activeTelegraphs)
            {
                telegraphLine.Draw(_spriteBatch);
            }
        }
        public static void ScreenShake(int magnitude, int duration)
        {
            if (magnitude >= ScreenShakeMagnitude) //always apply strongest screen shake
            { 
                ScreenShakeMagnitude = magnitude;
                ScreenShakeTimer = duration;
            }
        }

        public static void StopScreenShake()
        {
            ScreenShakeTimer = 0;
            MainCamera.SetScreenShakeOffset(0f);
        }
        public static void HandleScreenShake() //under the hood screen shaking
        {
            if (!Utilities.ImportantMenusPresent() && MainInstance.IsActive) 
            {
                MainCamera.SetScreenShakeOffset(Utilities.RandomFloat(0f, ScreenShakeMagnitude));
            }
            
            if (!IsScreenShaking)
            {
                MainCamera.SetScreenShakeOffset(0f);
            }
        }

        public static void ConfirmControlSettingsChange(SpriteBatch spriteBatch)
        {
            string ControlChangedTo = "";

            if (GameState.HasASettingBeenChanged)
            {
                if (GameState.WeaponSwitchControl)
                {
                    ControlChangedTo = "Weapon switch control switched to scroll wheel.";
                }

                else
                {
                    ControlChangedTo = "Weapon switch control switched to number keys.";
                }
            }
            Utilities.drawTextInDrawMethod(ControlChangedTo, new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 10 * 7), Main._spriteBatch, Main.font, Color.White);
        }
    }
}
