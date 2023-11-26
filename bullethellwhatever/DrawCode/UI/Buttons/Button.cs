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

namespace bullethellwhatever.DrawCode.UI.Buttons
{
    public class Button : UIElement
    {
        public Button(Texture2D texture, Vector2 size, Menu owner = null, Vector2 position = default) : base(texture, size, owner, position)
        {
            Texture = texture;
            Size = size;
            Position = position;
            Owner = owner;
        }
        public Button(string texture, Vector2 size, Menu owner = null, Vector2 position = default) : base(texture, size, owner, position)
        {
            Texture = Assets[texture];
            Size = size;
            Position = position;
            Owner = owner;
        }
    }
}

