using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.DrawCode.UI.Buttons
{
    public class ExitButton : Button
    {
        public ExitButton(Vector2 position, string texture, Vector2 size) : base(position, Assets[texture], size)
        {
            Texture = Assets[texture];
            Size = size;
        }
        public ExitButton(string texture, Vector2 size, Menu owner) : base(texture, size, owner)
        {
            Texture = Assets[texture];
            Size = size;
            Owner = owner;
        }
        public override void HandleClick()
        {
            MainInstance.Exit();

            base.HandleClick();
        }
    }
}
