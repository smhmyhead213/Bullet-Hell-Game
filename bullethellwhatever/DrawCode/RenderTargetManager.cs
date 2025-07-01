using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode
{
    public static class RenderTargetManager
    {
        public static RenderTarget2D CurrentRT;

        public static void SetRenderTarget(RenderTarget2D rt)
        {
            CurrentRT = rt;
            MainInstance.GraphicsDevice.SetRenderTarget(rt);
        }
    }
}
