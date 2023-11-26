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
        public ExitButton(string texture, Vector2 size, Menu owner = null, Vector2 position = default) : base(Assets[texture], size, owner, position)
        {
            Texture = Assets[texture];
            Size = size;
        }
        public override void HandleClick()
        {
            MainInstance.Exit();

            base.HandleClick();
        }
    }
}
