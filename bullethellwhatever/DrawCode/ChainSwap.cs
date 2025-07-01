using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;

namespace bullethellwhatever.DrawCode
{
    public class ChainSwap
    {
        public Texture2D Texture;
        public Vector2 Scale;
        public RenderTarget2D RT1;
        public RenderTarget2D RT2;
        public List<Shader> ShadersToApply;
        public ChainSwap(Texture2D texture, List<Shader> shadersToApply)
        {
            Texture = texture;
            ShadersToApply = shadersToApply;
            int width = (int)(Texture.Width * Scale.X);
            int height = (int)(Texture.Height * Scale.Y);
            RT1 = new RenderTarget2D(MainInstance.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            RT2 = new RenderTarget2D(MainInstance.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        }

        public void Draw()
        {
            RenderTarget2D original = RenderTargetManager.CurrentRT;
            Texture2D currentTextureStep = Texture;
            RenderTarget2D rtToUse = RT1;
            SpriteBatchSettings originalSpriteBatchSettings = Drawing.SBSettings;

            // dont return to main RT automatically - we are gonna do this manually
            Drawing.RestartSB(_spriteBatch, true, originalSpriteBatchSettings.UsingCamera, false);

            for (int i = 0; i < ShadersToApply.Count; i++)
            {
                RenderTargetManager.SetRenderTarget(rtToUse);
                ShadersToApply[i].Apply();
                _spriteBatch.Draw(currentTextureStep, Vector2.Zero, null, Color.White);
                currentTextureStep = rtToUse;
                rtToUse = rtToUse == RT1 ? RT2 : RT1;
            }
        }
    }
}
