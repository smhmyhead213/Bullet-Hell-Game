using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Security.Cryptography;

namespace bullethellwhatever.UtilitySystems
{
    
    public static class InputSystem
    {
        public static KeyboardState KeyboardState;
        public static MouseState MouseState;
        public static Vector2 MousePosition;
        public static Vector2 LastMousePosition;

        public static Dictionary<Keybind, KeyData> KeyStates;
        public static bool IgnoreKeybindPresses;
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
            IgnoreKeybindPresses = false;

            KeyStates = new Dictionary<Keybind, KeyData>();

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                KeyStates.Add(new Keybind(key), new KeyData());
            }

            foreach (MouseButtons mouseButton in Enum.GetValues(typeof(MouseButtons)))
            {
                if (mouseButton != MouseButtons.None)
                    KeyStates.Add(new Keybind(mouseButton), new KeyData());
            }
        }

        public static KeyData GetKeyData(Keys key)
        {
            return KeyStates[new Keybind(key)];
        }
        public static KeyData GetKeyData(MouseButtons mouseButton)
        {
            return KeyStates[new Keybind(mouseButton)];
        }

        public static Vector2 MousePositionWithCamera()
        {

            return (MousePosition / MainCamera.CameraScale) + MainCamera.VisibleArea.TopLeft();
        }
        public static void UpdateInputSystem()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                GetKeyData(key).WasDownLastFrame = KeyboardState.IsKeyDown(key);
            }

            foreach (MouseButtons mouseButton in Enum.GetValues(typeof(MouseButtons)))
            {
                GetKeyData(mouseButton).WasDownLastFrame = IsMouseButtonPressed(mouseButton);
            }

            LastMousePosition = TransformMousePositionForViewport(new Vector2(MouseState.X, MouseState.Y));
            
            KeyboardState = Keyboard.GetState();

            MouseState = Mouse.GetState();

            Vector2 rawMousePosition = new Vector2(MouseState.X, MouseState.Y);

            //MousePosition = (rawMousePosition - new Vector2(ScreenViewport.X, ScreenViewport.Y)) / ScreenScaleFactor();

            MousePosition = TransformMousePositionForViewport(rawMousePosition);

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                GetKeyData(key).IsDown = KeyboardState.IsKeyDown(key);
            }

            foreach (MouseButtons mouseButton in Enum.GetValues(typeof(MouseButtons)))
            {
                GetKeyData(mouseButton).IsDown = IsMouseButtonPressed(mouseButton);
            }
        }

        public static Vector2 TransformMousePositionForViewport(Vector2 position)
        {
            Vector2 rawMousePosition = position;

            return rawMousePosition - new Vector2(ScreenViewport.X, ScreenViewport.Y) / ScreenScaleFactor();
        }
        public static bool KeyDownLastFrame(string name)
        {
            return KeyStates[KeybindMap[name]].WasDownLastFrame;
        }

        public static bool IsLeftClickDown()
        {
            return KeyStates[KeybindMap[LeftClick]].IsDown;
        }

        public static bool LeftClickDownLastFrame()
        {
            return KeyStates[KeybindMap[LeftClick]].WasDownLastFrame;
        }
        public static bool LeftClickReleased()
        {
            return LeftClickDownLastFrame() && !IsLeftClickDown();
        }

        public static bool RightClickReleased()
        {
            return KeyStates[KeybindMap[RightClick]].WasDownLastFrame && !IsRightClickDown();
        }

        public static bool IsRightClickDown()
        {
            return KeyStates[KeybindMap[RightClick]].IsDown;
        }

        public static bool IsKeyPressed(string key)
        {
            return KeyStates[new Keybind(key)].IsDown;
        }

        public static bool IsKeyPressed(Keys key)
        {
            return KeyStates[new Keybind(key)].IsDown;
        }

        public static bool KeybindReleased(string keybind)
        {
            return WasKeybindPressedLastFrame(keybind) && !IsKeybindPressed(keybind);
        }
        public static bool IsKeybindPressed(string key)
        {
            return KeybindMap[key].IsPressed();
        }

        public static bool WasKeybindPressedLastFrame(string key)
        {
            return KeyStates[KeybindMap[key]].WasDownLastFrame;
        }

        public static bool WasKeyPressedLastFrame(string key)
        {
            return KeyStates[new Keybind(key)].WasDownLastFrame;
        }
        public static bool WasKeyPressedLastFrame(Keys key)
        {
            return KeyStates[new Keybind(key)].WasDownLastFrame;
        }
        public static bool LeftClickDownButNotLastFrame()
        {
            return IsLeftClickDown() && !LeftClickDownLastFrame();
        }
        public static bool IsKeyPressedAndWasntLastFrame(string key)
        {
            return IsKeyPressed(key) && !WasKeyPressedLastFrame(key);
        }

        public static bool IsKeyPressedAndWasntLastFrame(Keys key)
        {
            return IsKeyPressed(key) && !WasKeyPressedLastFrame(key);
        }

        public static List<Keybind> PressedKeys()
        {
            List<Keybind> keysPressedNow = new List<Keybind>();

            foreach (KeyValuePair<Keybind, KeyData> pair in KeyStates)
            {
                if (pair.Key.IsPressed())
                {
                    keysPressedNow.Add(pair.Key);
                }
            }

            return keysPressedNow;
        }

        public static List<Keybind> KeysPressedLastFrame()
        {
            List<Keybind> keysPressed = new List<Keybind>();

            foreach (KeyValuePair<Keybind, KeyData> pair in KeyStates)
            {
                if (KeyStates[pair.Key].WasDownLastFrame)
                {
                    keysPressed.Add(pair.Key);
                }
            }

            return keysPressed;
        }

        public static bool IsAnyKeyPressed(out Keybind key)
        {
            if (PressedKeys().Any())
            {
                key = PressedKeys().First(); // if multiple keys are pressed just take the first one who even cares
                return true;
            }
            else
            {
                key = new Keybind(Keys.None);
                return false;
            }
        }

        public static bool AnyKeyNewlyPressed(out Keybind key)
        {
            foreach (Keybind keybind in KeyStates.Keys)
            {
                if (!KeyStates[keybind].WasDownLastFrame && KeyStates[keybind].IsDown)
                {
                    key = keybind;
                    return true;
                }
            }

            key = new Keybind(Keys.None);
            return false;
        }

        public static bool IsMouseButtonPressed(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.LeftClick: return MouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.RightClick: return MouseState.RightButton == ButtonState.Pressed;
                case MouseButtons.MiddleClick: return MouseState.MiddleButton == ButtonState.Pressed;
                case MouseButtons.Mouse4: return MouseState.XButton1 == ButtonState.Pressed;
                case MouseButtons.Mouse5: return MouseState.XButton2 == ButtonState.Pressed;
                case MouseButtons.None: return false;
                default: return false; // we should never get here
            }
        }
    }
}
