using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using System;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using bullethellwhatever.AssetManagement;

namespace bullethellwhatever.DrawCode
{
    public class ChainSwap
    {
        public Texture2D Texture;
        public Vector2 Scale;
        public RenderTarget2D RT1;
        public RenderTarget2D RT2;
        public List<Shader> ShadersToApply;
        public Vector2 Position;
        public ChainSwap(Texture2D texture, List<Shader> shadersToApply, Vector2 scale, Vector2 position)
        {
            Texture = texture;
            Scale = scale;
            ShadersToApply = shadersToApply;
            int width = (int)(Texture.Width * Scale.X);
            int height = (int)(Texture.Height * Scale.Y);
            RT1 = new RenderTarget2D(MainInstance.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            RT2 = new RenderTarget2D(MainInstance.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Position = position;
        }

        public void Draw()
        {
            RenderTarget2D original = RenderTargetManager.CurrentRT;
            Texture2D currentTextureStep = Texture;
            RenderTarget2D rtToUse = RT1;
            SpriteBatchSettings originalSpriteBatchSettings = Drawing.SBSettings;

            for (int i = 0; i < ShadersToApply.Count; i++)
            {
                _spriteBatch.End();

                RenderTargetManager.SetRenderTarget(rtToUse);

                // dont return to main RT automatically - we are gonna do this manually
                // start SB in immediate mode

                Drawing.StartSB(_spriteBatch, true, false, false);

                ShadersToApply[i].Apply();

                currentTextureStep.SaveAsImage($"testbefore{i + 1}.jpg");

                MainInstance.GraphicsDevice.Textures[0] = currentTextureStep;

                _spriteBatch.Draw(currentTextureStep, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

                currentTextureStep = rtToUse;

                currentTextureStep.SaveAsImage($"test{i + 1}.jpg");

                if (i != ShadersToApply.Count - 1)
                {
                    rtToUse = rtToUse == RT1 ? RT2 : RT1;
                }
            }

            // restore to original state
            Drawing.SBSettings = originalSpriteBatchSettings;
            Drawing.RestartSB(_spriteBatch, originalSpriteBatchSettings);

            RenderTargetManager.SetRenderTarget(original);
            rtToUse.SaveAsImage($"final.jpg");
            _spriteBatch.Draw(rtToUse, Position, null, Color.White);
        }

        public static void Test()
        {
            Shader outline = AssetRegistry.GetShader("MenuOutline");
            outline.SetColour(Color.Blue);
            outline.SetParameter("backgroundColour", Color.Yellow);
            outline.SetParameter("xThickness", 0.25f);
            outline.SetParameter("yThickness", 0.25f);

            Shader blackLine = AssetRegistry.GetShader("DrawVerticalBlackLine");

            List<Shader> shaders = new List<Shader>
            {
                outline,
                blackLine
            };

            ChainSwap test = new ChainSwap(AssetRegistry.GetTexture2D("box"), shaders, Vector2.One * 10f, Utilities.CentreOfScreen());
            test.Draw();
        }

        public static void ControlTest()
        {
            Shader outline = AssetRegistry.GetShader("MenuOutline");
            outline.SetColour(Color.Red);
            outline.SetParameter("backgroundColour", Color.Yellow);
            outline.SetParameter("xThickness", 0.3f);
            outline.SetParameter("yThickness", 0.3f);

            outline.Apply();
            Drawing.BetterDraw("box", Utilities.CentreOfScreen() * 0.5f, null, Color.Blue, 0f, Vector2.One * 10f, SpriteEffects.None, 1f);
        }
    }
}
