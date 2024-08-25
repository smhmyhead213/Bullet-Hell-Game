using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace bullethellwhatever.UtilitySystems
{
    public static class InputSystem
    {
        public static KeyboardState KeyboardState;
        public static MouseState MouseState;
        public static Vector2 MousePosition;
        public static bool WasMouseDownLastFrame;
        public static void UpdateInputSystem()
        {
            WasMouseDownLastFrame = IsLeftClickDown();

            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            Vector2 rawMousePosition = new Vector2(MouseState.X, MouseState.Y);

            MousePosition = (rawMousePosition - new Vector2(ScreenViewport.X, ScreenViewport.Y)) / ScreenScaleFactor();
        }

        public static bool IsLeftClickDown()
        {
            return MouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool IsRightClickDown()
        {
            return MouseState.RightButton == ButtonState.Pressed;
        }
        public static bool IsKeyPressed(Keys key)
        {
            return KeyboardState.IsKeyDown(key);
        }

    
    }
}
