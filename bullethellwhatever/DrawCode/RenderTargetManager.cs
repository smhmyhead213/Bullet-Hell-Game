using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Win32;

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

        public static void SaveAsImage(this Texture2D texture, string filename)
        {
            FileStream stream;

            if (!File.Exists(filename))
            {
                stream = File.Create(filename);
            }
            else
            {
                stream = File.OpenWrite(filename);
            }

            texture.SaveAsJpeg(stream, texture.Width, texture.Height);
            stream.Dispose();
        }

        public static void SaveAsImage(this RenderTarget2D renderTarget2D, string filename)
        {
            FileStream stream;

            if (!File.Exists(filename))
            {
                stream = File.Create(filename);
            }
            else 
            {
                stream = File.OpenWrite(filename);
            }

            renderTarget2D.SaveAsJpeg(stream, renderTarget2D.Width, renderTarget2D.Height);
            stream.Dispose();
        }
    }
}
