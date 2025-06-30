using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode.UI
{
    public class UITextComponent : UIComponent
    {
        public Func<string> TextFunction;
        public Vector2 TextSize;
        public Vector2 TextPosition;
        public Color TextColour;
        public UITextComponent(Vector2 position, Vector2 textSize, Func<string> textFunction) : base()
        {
            TextFunction = textFunction;
            TextSize = textSize;
            TextPosition = position;
            TextColour = Color.White;
        }
        public override void Initialise()
        {
            TextFunction = () => "";
            TextSize = Vector2.One;
        }
        public override void Update()
        {
            
        }

        public override void Draw(SpriteBatch s)
        {
            Vector2 textSizeCurrent = font.MeasureString(TextFunction());

            Drawing.DrawText(TextFunction(), TextPosition, s, font, TextColour, new Vector2(TextSize.X / textSizeCurrent.X, TextSize.Y / textSizeCurrent.Y));
        }
    }
}
