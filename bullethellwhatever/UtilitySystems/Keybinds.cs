using System;
using System.Collections.Generic;
using System.Configuration;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FMOD;
using Microsoft.Xna.Framework.Input;
using SharpDX.DXGI;

namespace bullethellwhatever.UtilitySystems
{
    [JsonConverter(typeof(KeybindJsonConverter))]
    public struct Keybind
    {
        // i hate that mouse buttons arent actually counted as buttons the way keys are so i just have to do this
        public Keys Key;
        public MouseButtons MouseButton;

        public Keybind(char key)
        {
            Key = CharToKey(key);
            MouseButton = MouseButtons.None;
        }

        public Keybind(string button)
        {
            bool keySuccess = Enum.TryParse(button, out Key);
            MouseButton = MouseButtons.None;

            if (!keySuccess)
            {
                Key = Keys.None;
                bool mouseSuccess = Enum.TryParse(button, out MouseButton);

                if (!mouseSuccess)
                {
                    throw new Exception("invalid key");
                }
            }
        }
        public Keybind(Keys key)
        {
            Key = key;
            MouseButton = MouseButtons.None;
        }

        public Keybind(MouseButtons button)
        {
            MouseButton = button;
            Key = Keys.None;
        }

        public static Keys CharToKey(char character)
        {
            return (Keys)character;
        }

        public bool IsPressed()
        {
            return IsKeyPressed(Key) || IsMouseButtonPressed(MouseButton);
        }
    }

    // thank you microsoft!
    public enum MouseButtons
    {
        LeftClick,
        RightClick,
        MiddleClick,
        Mouse4,
        Mouse5,
        None
    }

    public class KeybindJsonConverter : JsonConverter<Keybind>
    {
        public override Keybind Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Keybind value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value.Key == Keys.None) // assume that this indicates that a mousebutton is bound
            {
                string mbutton = value.MouseButton.ToString();
                writer.WriteString(nameof(mbutton), mbutton);
            }
            else
            {
                string key = ((char)(int)value.Key).ToString(); // this is gross
                writer.WriteString(nameof(key), key);
            }

            writer.WriteEndObject();
        }
    }
    public static class Keybinds
    {
        public static Dictionary<string, Keybind> KeybindMap;

        public static void Initialise()
        {
            // to do: read in from save system to get the already selected keybinds

            KeybindMap = new Dictionary<string, Keybind>();
            
        }

        public static Dictionary<string, Keybind> DefaultKeybinds()
        {
            Dictionary<string, char> keyboardBinds = new Dictionary<string, char>
            {
                { "Dash", (char)32 }, //Keys.Space = 32
                { "Up", 'W' },
                { "Down", 'S' },
                { "Left", 'A' },
                { "Right", 'D' },
                { "Precision", (char)160 }, //Keys.LeftShift = 160
                { "MenuTab", (char)9 }, //Keys.Tab = 9
                { "MenuSelect", (char)13 } //Keys.Enter = 13
            };

            Dictionary<string, MouseButtons> mouseBinds = new Dictionary<string, MouseButtons>
            {
                { "LeftClick", MouseButtons.LeftClick },
                { "RightClick", MouseButtons.RightClick }
            };

            Dictionary<string, Keybind> output = new Dictionary<string, Keybind>();

            foreach (KeyValuePair<string, char> pair in keyboardBinds)
            {
                output.Add(pair.Key, new Keybind(pair.Value));
            }

            foreach (KeyValuePair<string, MouseButtons> pair in mouseBinds)
            {
                output.Add(pair.Key, new Keybind(pair.Value));
            }

            return output;
        }

        public static Dictionary<string, string> KeybindMapToStrings(Dictionary<string, Keybind> input)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();

            foreach (KeyValuePair<string, Keybind> pair in input)
            {
                string keybind = pair.Value.Key.ToString();

                if (IsKey(keybind))
                {
                    output.Add(pair.Key, keybind);
                }
                else
                {
                    output.Add(pair.Key, pair.Value.MouseButton.ToString());
                }
            }

            return output;
        }

        public static bool IsKey(string stored)
        {
            // oh boy.

            foreach (MouseButtons mousebutton in Enum.GetValues(typeof(MouseButtons)))
            {
                if (stored == mousebutton.ToString())
                {
                    return false;
                }
            }

            return true;
        }

        public static Dictionary<string, Keybind> ReadSave(Dictionary<string, string> savedData)
        {
            Dictionary<string, Keybind> output = new Dictionary<string, Keybind>();

            foreach (KeyValuePair<string, string> pair in savedData)
            {
                //if (IsKey(pair.Value)) // hacky detection for whether this is a key or mouse button
                //{
                //    output.Add(pair.Key, new Keybind(pair.Value)); // actual misery
                //}
                //else
                //{
                //    output.Add(pair.Key, new Keybind(StringToMouseButton(pair.Value)));
                //}

                output.Add(pair.Key, new Keybind(pair.Value));
            }

            return output;
        }
    }
}
