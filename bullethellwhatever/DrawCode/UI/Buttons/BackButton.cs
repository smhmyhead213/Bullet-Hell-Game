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
    public class BackButton : Button
    {
        public BackButton(string texture, Vector2 size, Menu owner = null, Vector2 position = default) : base(AssetRegistry.GetTexture2D(texture), size, owner, position)
        {
            Texture = AssetRegistry.GetTexture2D(texture);
            Size = size;
        }
        public BackButton(Texture2D texture, Vector2 size, Menu owner = null, Vector2 position = default) : base(texture, size, owner, position)
        {
            Texture = texture;
            Size = size;
        }
        public override void HandleClick()
        {
            GameState.RevertToPreviousGameState();
            Owner.Hide();

            base.HandleClick();
        }
    }
}
