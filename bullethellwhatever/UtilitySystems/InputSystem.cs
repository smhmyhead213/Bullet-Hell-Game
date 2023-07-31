using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace bullethellwhatever.UtilitySystems
{
    public static class InputSystem
    {
        public static KeyboardState KeyboardState;
        public static MouseState MouseState;
        public static Vector2 MousePosition;
        public static void UpdateInputSystem()
        {
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            MousePosition = new Vector2(MouseState.X, MouseState.Y);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return KeyboardState.IsKeyDown(key);
        }

    
    }
}
