using bullethellwhatever.AssetManagement;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.DrawCode.UI.SpecialUIElements
{
    public class Slider : UIElement
    {
        public float Value;
        public float MinValue;
        public float MaxValue;
        public float SliderLengthRatio;
        public float SliderHeightRatio;
        public Func<float, string> SliderText;
        public Slider(string texture, Vector2 size, Vector2 position, float minValue, float maxValue, float initialValue) : base(texture, size, position)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Value = initialValue;
            SliderLengthRatio = 0.7f;
            SliderHeightRatio = 0.1f;
            SliderText = (float val) => val.ToString();
            Colour = Color.Black;
        }

        public override void Update()
        {
            base.Update();
            
            if (IsLeftClickDown() && ClickBox.Contains(MousePosition))
            {
                float leftEnd = SliderLineLeftEnd(Position).X;
                float rightEnd = SliderLineRightEnd(Position).X;

                float interpolant = Utilities.InverseLerp(leftEnd, rightEnd, MousePosition.X);
                Value = MathHelper.Lerp(MinValue, MaxValue, interpolant);
            }           
        }

        public override RectangleF CalculateClickbox()
        {
            float width = SliderLineWidth();
            float height = SliderLineHeight();

            // this only encapulates bar - make it tall enough to encapsulate full grabber as well
            return new RectangleF(Position.X - width / 2f, Position.Y - height / 2f, width, height);
        }
        public Vector2 GrabberPosition(Vector2 sliderPosition)
        {
            float interpolant = Utilities.InverseLerp(MinValue, MaxValue, Value);
            return SliderLineLeftEnd(sliderPosition) + new Vector2(interpolant * SliderLineWidth(), 0f);
        }

        public float SliderLineWidth()
        {
            return Size.X * SliderLengthRatio;
        }
        public float SliderLineHeight()
        {
            return Size.Y * SliderHeightRatio;
        }
        public Vector2 SliderLineLeftEnd(Vector2 positionOfSlider)
        {
            float width = SliderLineWidth();
            return new Vector2(positionOfSlider.X - width / 2, positionOfSlider.Y);
        }
        public Vector2 SliderLineRightEnd(Vector2 positionOfSlider)
        {
            float width = SliderLineWidth();
            return new Vector2(positionOfSlider.X + width / 2, positionOfSlider.Y);
        }
        public override void DrawAtPosition(SpriteBatch s, Vector2 position)
        {
            Color colour = Colour;
            Color sliderlineColour = Color.White;
            Color grabberColour = Color.White;

            Drawing.BetterDraw(Texture, position, null, colour * Opacity, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1);

            Texture2D sliderLine = AssetRegistry.GetTexture2D("box");
            Texture2D grabbingCircle = AssetRegistry.GetTexture2D("Circle");
            Vector2 scale = new Vector2(Size.X * SliderLengthRatio / sliderLine.Width, Size.Y * SliderHeightRatio / sliderLine.Height);
            Drawing.BetterDraw(sliderLine, position, null, sliderlineColour * Opacity, 0f, scale, SpriteEffects.None, 1f);
            
            Vector2 grabberPos = GrabberPosition(position);
            Drawing.BetterDraw(grabbingCircle, grabberPos, null, grabberColour, 0f, Vector2.One, SpriteEffects.None, 1f);

            Drawing.DrawText(SliderText(Round(Value, 0)), position - Size / 2f + new Vector2(30f), s, font, Color.White, Vector2.One);
        }
    }
}
