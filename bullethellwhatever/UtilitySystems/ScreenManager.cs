using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.UtilitySystems
{
    public static class ScreenManager
    {
        public static bool ResizingView;
        public static int ViewWidth;
        public static int ViewHeight;
        public static Matrix4x4 ScreenMatrix;
        public static Viewport ScreenViewport;
        public static void WindowSizeChange(object sender, EventArgs e)
        {
            if (MainInstance.Window.ClientBounds.Width > 0 && MainInstance.Window.ClientBounds.Height > 0 && !ResizingView)
            {
                ResizingView = true;
                _graphics.PreferredBackBufferWidth = MainInstance.Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = MainInstance.Window.ClientBounds.Height;
                UpdateView();                
                ResizingView = false;
            }
        }

        public static void GraphicsManager_DeviceCreated(object sender, EventArgs e) => UpdateView();

        public static void GraphicsManager_DeviceReset(object sender, EventArgs e) => UpdateView();

        public static void UpdateView()
        {
            float width = MainInstance.GraphicsDevice.PresentationParameters.BackBufferWidth;
            float height = MainInstance.GraphicsDevice.PresentationParameters.BackBufferHeight;

            if (width / IdealScreenWidth > height / IdealScreenHeight)
            {
                ViewWidth = (int)(height / IdealScreenHeight * IdealScreenWidth);
                ViewHeight = (int)height;
            }
            else
            {
                ViewWidth = (int)width;
                ViewHeight = (int)(width / IdealScreenWidth * IdealScreenHeight);
            }

            ScreenMatrix = Matrix4x4.CreateScale((float)ViewWidth / IdealScreenWidth);
            ScreenViewport = new((int)(width / 2f - ViewWidth / 2f), (int)(height / 2f - ViewHeight / 2f), ViewWidth, ViewHeight, 0f, 1f);
        }
    }
}
