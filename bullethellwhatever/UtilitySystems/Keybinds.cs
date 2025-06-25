using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace bullethellwhatever.UtilitySystems
{
    public static class Keybinds
    {
        public static Dictionary<string, Keybind> KeybindMap;
        public static void Initialise()
        {
            // to do: read in from save system to get the already selected keybinds

            KeybindMap = new Dictionary<string, Keybind>();
            KeybindMap.Add("Up", new Keybind(Keys.W));
        }
    }
}
