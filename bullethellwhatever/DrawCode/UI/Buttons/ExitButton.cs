using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.AssetManagement;

namespace bullethellwhatever.DrawCode.UI.Buttons
{
    public class ExitButton : Button
    {
        public ExitButton(string texture, Vector2 size, Menu owner = null, Vector2 position = default) : base(AssetRegistry.GetTexture2D(texture), size, owner, position)
        {
            Texture = AssetRegistry.GetTexture2D(texture);
            Size = size;
        }
        public override void HandleClick()
        {
            MainInstance.Exit();

            base.HandleClick();
        }
    }
}
