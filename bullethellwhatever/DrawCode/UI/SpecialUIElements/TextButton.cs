using bullethellwhatever.AssetManagement;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using static System.Net.Mime.MediaTypeNames;

namespace bullethellwhatever.DrawCode.UI.SpecialUIElements
{
    public class TextButton : UIElement
    {
        public string Text;
        public float TextHorizontalPadding;
        public float TextVerticalPadding;
        public TextButton(string text, float horizontalPadding, float verticalPadding, Vector2 size, Vector2 position) : base("box", size, position)
        {
            Text = text;
            TextHorizontalPadding = horizontalPadding;
            TextVerticalPadding = verticalPadding;
            Colour = Color.Black;
        }

        public override void Draw(SpriteBatch s)
        {
            DrawAtPosition(s, Position);
        }

        public override void DrawAtPosition(SpriteBatch s, Vector2 position)
        {
            Color colour = ColourIfSelected();
            Vector2 topLeft = new Vector2(position.X - Size.X / 2, position.Y - Size.Y / 2);

            Drawing.BetterDraw(Texture, position, null, colour * Opacity, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1);

            Vector2 textSize = font.MeasureString(Text);
            float availableWidth = Size.X - 2 * TextHorizontalPadding;
            float availableHeight = Size.Y - 2 * TextVerticalPadding;

            float widthScale = availableWidth / textSize.X;
            float heightScale = availableHeight / textSize.Y;
            float scale = Min(widthScale, heightScale);

            Drawing.DrawText(Text, topLeft + new Vector2(TextHorizontalPadding, TextVerticalPadding), s, font, Color.White, new Vector2(scale));
        }
    }
}
