using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

using bullethellwhatever.Buttons;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.UtilitySystems;
using System.Diagnostics.Contracts;
using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.MainFiles;


namespace bullethellwhatever.DrawCode
{
    public static class Drawing
    {
        public static bool AreButtonsDrawn;
        public static ScreenShakeObject screenShakeObject;
        public static Vector2 ScreenShakeMagnitude;
        public static int ScreenShakeDuration;
        public static int ScreenShakeTimer;
        public static bool IsScreenShaking;
        public static int Timer;

        public static void Initialise()
        {
            screenShakeObject = new ScreenShakeObject(0, 0);
        }
        public static void BetterDraw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 scale, SpriteEffects spriteEffects, float layerDepth)
        {
            //This method exists so that one does not have to repeat the same paraemters for stuff like origin offsets and screenshake offset.

            //Draw the item at the position, moved by the amount the screen is shaking.

            if (screenShakeObject.Timer > 0)
            {
                Vector2 positionWithScreenShake = new(position.X + screenShakeObject.Magnitude.X, position.Y + screenShakeObject.Magnitude.Y);

                Main._spriteBatch.Draw(texture, positionWithScreenShake, sourceRectangle, color, rotation, new Vector2(texture.Width / 2, texture.Height / 2), scale, spriteEffects, layerDepth);
            }

            else Main._spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, new Vector2(texture.Width / 2, texture.Height / 2), scale, spriteEffects, layerDepth);
        }

        public static void DrawTelegraphs(Entity entity)
        {
            foreach (TelegraphLine telegraphLine in entity.activeTelegraphs)
            {
                telegraphLine.Draw(Main._spriteBatch);
            }
        }
        public static void ScreenShake(int magnitude, int duration)
        {
            if (magnitude > screenShakeObject.Magnitude.X) //always apply strongest screen shake
                screenShakeObject = new ScreenShakeObject(magnitude, duration);
        }

        public static void HandleScreenShake() //under the hood screen shaking
        {
            if (screenShakeObject is not null)
            {
                screenShakeObject.TickDownDuration();

                Random rng = new Random();

                screenShakeObject.Magnitude = new(rng.Next((int)screenShakeObject.MaxMagnitude.X), rng.Next((int)screenShakeObject.MaxMagnitude.Y));
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
