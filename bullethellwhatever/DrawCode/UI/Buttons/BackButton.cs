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
    public class BackButton : Button
    {
        public BackButton(string texture, Vector2 size, Menu owner = null, Vector2 position = default) : base(Assets[texture], size, owner, position)
        {
            Texture = Assets[texture];
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
