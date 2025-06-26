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

        public static Dictionary<Keys, KeyData> KeyStates;

        public static bool WasMouseDownLastFrame;
        public static bool WasRightClickDownLastFrame;

        public class KeyData
        {
            public bool WasDownLastFrame;
            public bool IsDown;

            public KeyData()
            {
                WasDownLastFrame = false;
                IsDown = false;
            }
        }
        public static void Initialise()
        {
            KeyStates = new Dictionary<Keys, KeyData>();

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                KeyStates.Add(key, new KeyData());
            }
        }

        public static Vector2 MousePositionWithCamera()
        {
            return (MousePosition / MainCamera.CameraScale) + MainCamera.VisibleArea.TopLeft();
        }
        public static void UpdateInputSystem()
        {
            WasMouseDownLastFrame = IsLeftClickDown();
            WasRightClickDownLastFrame = IsRightClickDown();

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                KeyStates[key].WasDownLastFrame = KeyboardState.IsKeyDown(key);
            }

            KeyboardState = Keyboard.GetState();

            MouseState = Mouse.GetState();

            Vector2 rawMousePosition = new Vector2(MouseState.X, MouseState.Y);

            MousePosition = (rawMousePosition - new Vector2(ScreenViewport.X, ScreenViewport.Y)) / ScreenScaleFactor();

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                KeyStates[key].IsDown = KeyboardState.IsKeyDown(key);
            }
        }

        public static bool IsLeftClickDown()
        {
            return MouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool LeftClickReleased()
        {
            return WasMouseDownLastFrame && !IsLeftClickDown();
        }

        public static bool RightClickReleased()
        {
            return WasRightClickDownLastFrame && !IsRightClickDown();
        }

        public static bool IsRightClickDown()
        {
            return MouseState.RightButton == ButtonState.Pressed;
        }
        public static bool IsKeyPressed(Keys key)
        {
            return KeyStates[key].IsDown;
        }
        public static bool WasKeyPressedLastFrame(Keys key)
        {
            return KeyStates[key].WasDownLastFrame;
        }
        public static bool LeftClickDownButNotLastFrame()
        {
            return IsLeftClickDown() && !WasMouseDownLastFrame;
        }
        public static bool IsKeyPressedAndWasntLastFrame(Keys key)
        {
            return IsKeyPressed(key) && !WasKeyPressedLastFrame(key);
        }

        public static bool IsMouseButtonPressed(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.LeftClick: return IsLeftClickDown();
                case MouseButtons.RightClick: return IsRightClickDown();
                case MouseButtons.MiddleClick: return MouseState.MiddleButton == ButtonState.Pressed;
                case MouseButtons.Mouse4: return MouseState.XButton1 == ButtonState.Pressed;
                case MouseButtons.Mouse5: return MouseState.XButton2 == ButtonState.Pressed;
                case MouseButtons.None: return false;
                default: return false; // we should never get here
            }
        }
    }
}
