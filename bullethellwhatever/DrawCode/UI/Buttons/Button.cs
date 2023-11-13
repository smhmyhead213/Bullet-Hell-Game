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
        public Button(Vector2 position, string texture, Vector2 size, Menu owner) : base(position, texture, size, owner)
        {
            PositionInMenu = position;
            Texture = Assets[texture];
            Size = size;
            Owner = owner;
        }

        public Button(Texture2D texture, Vector2 size, Menu owner) : base(texture, size, owner)
        {
            Texture = texture;
            Size = size;
            Owner = owner;
        }
        public Button(string texture, Vector2 size, Menu owner) : base(texture, size, owner)
        {
            Texture = Assets[texture];
            Size = size;
            Owner = owner;
        }
        public Button(Vector2 position, Texture2D texture, Vector2 size) : base(position, texture, size)
        {
            Texture = texture;
            Size = size;
            Position = position;
        }
    }
}

