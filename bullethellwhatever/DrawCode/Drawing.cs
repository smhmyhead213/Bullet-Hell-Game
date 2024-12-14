using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.UtilitySystems;
using System.Diagnostics.Contracts;
using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.MainFiles;
using bullethellwhatever.AssetManagement;
using System.Runtime.CompilerServices;
using bullethellwhatever.BaseClasses.Entities;
namespace bullethellwhatever.DrawCode
{
    public static class Drawing
    {
        public static bool AreButtonsDrawn;

        public static float ScreenShakeMagnitude;
        public static int ScreenShakeTimer;
        public static bool IsScreenShaking => ScreenShakeTimer > 0;
        public static int Timer;

        public static bool DrawingShaders;

        public static SpriteBatch PreviousSpriteBatch;
        public static void Initialise()
        {
            DrawingShaders = false;
        }

        public static void UpdateDrawer()
        {
            if (!Utilities.ImportantMenusPresent() && MainInstance.IsActive)
            {
                ScreenShakeTimer--;
            }
        }
        public static void RestartSpriteBatchForShaders(SpriteBatch s, bool useCamera)
        {
            s.End();
            MainInstance.GraphicsDevice.SetRenderTarget(MainRT);
            System.Numerics.Matrix4x4 transform = useCamera ? MainCamera.Matrix : System.Numerics.Matrix4x4.Identity;
            s.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.LinearWrap, transformMatrix: transform);
            DrawingShaders = true;
        }

        public static void RestartSpriteBatchForNotShaders(SpriteBatch s, bool useCamera)
        {
            s.End();
            MainInstance.GraphicsDevice.SetRenderTarget(MainRT);
            System.Numerics.Matrix4x4 transform = useCamera ? MainCamera.Matrix : System.Numerics.Matrix4x4.Identity;
            s.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointWrap, transformMatrix: transform);
            DrawingShaders = false;
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
            ScreenShakeMagnitude = 0;
            MainCamera.ScreenShakeOffset = 0f;
        }
        public static void HandleScreenShake() //under the hood screen shaking
        {
            if (!Utilities.ImportantMenusPresent() && MainInstance.IsActive && IsScreenShaking) 
            {
                MainCamera.ScreenShakeOffset = Utilities.RandomFloat(-ScreenShakeMagnitude, ScreenShakeMagnitude);
            }
            
            if (!IsScreenShaking)
            {
                StopScreenShake();
            }
        }

        public static void ConfirmControlSettingsChange(SpriteBatch spriteBatch)
        {
            string ControlChangedTo = "";

            if (GameState.WeaponSwitchControl == GameState.WeaponSwitchControls.ScrollWheel)
            {
                ControlChangedTo = "Weapon switch control switched to scroll wheel.";
            }
            else
            {
                ControlChangedTo = "Weapon switch control switched to number keys.";
            }

            DialogueSystem.Dialogue(ControlChangedTo, 1, new Vector2(GameWidth / 2, GameHeight / 10 * 7));
        }
    }
}
