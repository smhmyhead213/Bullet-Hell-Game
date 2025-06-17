using bullethellwhatever.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.DrawCode.UI.SpecialUIElements
{
    public class EmptySpace : UIElement
    {
        public EmptySpace(Vector2 size, Vector2 position) : base("box", size, position)
        {
            Colour = Color.White;
            Opacity = 0f; // its like its not even there...
            Interactable = false;
        }

        public override void DrawAtPosition(SpriteBatch s, Vector2 position)
        {
            Color colour = Colour * Opacity;
            //colour = Color.Red;
            Drawing.BetterDraw(Texture, position, null, colour, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1);
        }
    }
}
