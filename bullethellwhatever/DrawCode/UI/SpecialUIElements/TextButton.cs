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
        public Color TextColour;
        public Color HoveredTextColour;

        public float TextHorizontalPaddingLeft;
        public float TextVerticalPaddingTop;
        public float TextMarginX;
        public float TextMarginY;
        public bool ScaleTextToFit;
        public Vector2 TextScale;

        public Shader BGShader;
        
        public TextButton(string text, float horizontalPadding, float verticalPadding, Vector2 size, Vector2 position) : base("box", size, position)
        {
            Prepare(text, horizontalPadding, verticalPadding, size, position);
        }
        public TextButton(string texture, string text, float horizontalPadding, float verticalPadding, Vector2 size, Vector2 position) : base(texture, size, position)
        {
            Texture = AssetRegistry.GetTexture2D(texture);
            Prepare(text, horizontalPadding, verticalPadding, size, position);
        }

        public void Prepare(string text, float horizontalPadding, float verticalPadding, Vector2 size, Vector2 position)
        {
            Text = text;
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
            ScaleTextToFit = true;

            BGShader = AssetRegistry.GetShader("ColourShader");
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

        public Vector2 TextSize()
        {
            Vector2 textSize = font.MeasureString(Text);
            textSize.X *= TextScale.X;
            textSize.Y *= TextScale.Y;
            return textSize;
        }
        public override void DrawAtPosition(SpriteBatch s, Vector2 position)
        {
            Color colour = ColourIfSelected();
            Color textColour = InteractableAndHovered() ? HoveredTextColour : TextColour;

            Vector2 topLeft = new Vector2(position.X - Size.X / 2, position.Y - Size.Y / 2);

            if (IsHovered() && Interactable)
            {
                Drawing.RestartSB(s, true, false);

                BGShader.SetColour(Color.White);
                BGShader.Apply();

                Drawing.BetterDraw(Texture, position, null, colour * Opacity, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1);

                Drawing.RestartSB(s, false, false);
            }

            Vector2 scale = TextScale;

            if (ScaleTextToFit)
            {
                Vector2 textSize = TextSize();

                float availableWidth = Size.X - 2 * TextMarginX - TextHorizontalPaddingLeft;
                float availableHeight = Size.Y - 2 * TextMarginY - TextVerticalPaddingTop;

                float widthScale = availableWidth / textSize.X;
                float heightScale = availableHeight / textSize.Y;
                scale = new Vector2(Min(widthScale, heightScale));
            }

            // for some reason when the font is drawn it draws ever so slightly below where its told to, 5 added to combat this
            Vector2 pos = topLeft + new Vector2(TextMarginX + TextHorizontalPaddingLeft, TextMarginY + TextVerticalPaddingTop + 6f);
            //pos = topLeft + new Vector2(TextMarginX + TextHorizontalPaddingLeft, 0);

            Drawing.DrawText(Text, pos, s, font, textColour, scale);
        }
    }
}
