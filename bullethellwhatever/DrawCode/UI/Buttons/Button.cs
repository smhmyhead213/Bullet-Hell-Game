using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.AssetManagement;

namespace bullethellwhatever.DrawCode.UI.Buttons
{
    public class Button : UIElement
    {
        public Button(string texture, Vector2 size, Vector2 position = default) : base(texture, size, position)
        {

        }

        public Button(string texture, float size, Vector2 position = default) : base(texture, size, position)
        {

        }
    }
}

