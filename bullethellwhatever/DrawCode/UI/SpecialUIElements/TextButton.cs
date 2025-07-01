using bullethellwhatever.AssetManagement;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Permissions;

namespace bullethellwhatever.DrawCode.UI.SpecialUIElements
{
    public class TextButton : UIElement
    {
        public string Text;
        public Func<string> TextFunction;

        public Color TextColour;
        public Color HoveredTextColour;

        public float TextHorizontalPaddingLeft;
        public float TextVerticalPaddingTop;
        public float TextMarginX;
        public float TextMarginY;
        public Vector2 TextScale;

        public Shader BGShader;
        
        public TextButton(string text, float horizontalPadding, float verticalPadding, Vector2 size, Vector2 position) : base("box", size, position)
        {
            Text = text;
            Prepare(horizontalPadding, verticalPadding, size, position);
        }
        public TextButton(Func<string> text, float horizontalPadding, float verticalPadding, Vector2 size, Vector2 position) : base("box", size, position)
        {
            TextFunction = text;
            Prepare(horizontalPadding, verticalPadding, size, position);
        }

        public TextButton(string texture, string text, float horizontalPadding, float verticalPadding, Vector2 size, Vector2 position) : base(texture, size, position)
        {
            Text = text;
            Texture = AssetRegistry.GetTexture2D(texture);
            Prepare(horizontalPadding, verticalPadding, size, position);
        }

        public void Prepare(float horizontalPadding, float verticalPadding, Vector2 size, Vector2 position)
        {
            TextHorizontalPaddingLeft = horizontalPadding;
            TextVerticalPaddingTop = verticalPadding;
            Colour = Color.Black;
            TextColour = Color.White;
            HoveredTextColour = Color.Black;
            Size = size;
            Position = position;
            TextScale = Vector2.One;
            TextMarginX = 0f;
            TextMarginY = 0f;

            BGShader = AssetRegistry.GetShader("ColourShader");
        }

        public string GetText()
        {
            return Text == default ? TextFunction() : Text;
        }

        public override void Draw(SpriteBatch s)
        {
            DrawAtPosition(s, Position);
        }

        public override Color HoveredColour()
        {
            //return Color.AliceBlue;
            return new Color(255, 255, 254);
        }

        public void RightAlignText()
        {
            TextHorizontalPaddingLeft = Size.X - 2 * TextMarginX - TextSize().X;
        }
        public void CentreTextVertically()
        {
            float textHeight = TextSize().Y;
            TextVerticalPaddingTop = (Size.Y - textHeight) / 2;
        }
        public void CentreTextHorizontally()
        {
            float textWidth = TextSize().X;
            TextHorizontalPaddingLeft = (Size.X - textWidth) / 2;
        }
        public void CentreText()
        {
            CentreTextHorizontally();
            CentreTextVertically();
        }

        public void ScaleTextToFit()
        {
            Vector2 textSizeNow = TextSize();
            Vector2 availableSpace = new Vector2(Size.X - 2 * TextHorizontalPaddingLeft, Size.Y - 2 * TextVerticalPaddingTop);

            float scaleFactor = Min(availableSpace.X / textSizeNow.X, availableSpace.Y / textSizeNow.Y);
            TextScale = new Vector2(scaleFactor);
        }
        public Vector2 TextSize()
        {
            Vector2 textSize = font.MeasureString(GetText());
            textSize.X *= TextScale.X;
            textSize.Y *= TextScale.Y;
            return textSize;
        }
        public override void DrawAtPosition(SpriteBatch s, Vector2 position)
        {
            Color colour = ColourIfSelected();
            Color textColour = InteractableAndHovered() ? HoveredTextColour : TextColour;

            Vector2 topLeft = new Vector2(position.X - Size.X / 2, position.Y - Size.Y / 2);

            if (IsSelected() && Interactable)
            {
                Drawing.RestartSB(s, true, false, false);

                BGShader.SetColour(Color.White);
                BGShader.Apply();

                Drawing.BetterDraw(Texture, position, null, colour * Opacity, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1);

                Drawing.RestartSB(s, false, false, false);
            }
            else
            {
                Drawing.BetterDraw(Texture, position, null, colour * Opacity, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1);
            }

            Vector2 scale = TextScale;

            // for some reason when the font is drawn it draws ever so slightly below where its told to, 5 added to combat this
            Vector2 pos = topLeft + new Vector2(TextMarginX + TextHorizontalPaddingLeft, TextMarginY + TextVerticalPaddingTop + 6f);

            //pos = topLeft + new Vector2(TextMarginX + TextHorizontalPaddingLeft, 0);

            //one of these should always have a value
            string text = GetText();

            Drawing.DrawText(text, pos, s, font, textColour, scale);
        }
    }
}
