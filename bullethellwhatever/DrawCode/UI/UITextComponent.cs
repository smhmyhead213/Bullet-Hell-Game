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
        public Vector2 TextScale;
        public Vector2 TextPosition;
        public Color TextColour;
        public UITextComponent(Vector2 position, Vector2 scale, Func<string> textFunction) : base()
        {
            TextFunction = textFunction;
            TextScale = scale;
            TextPosition = position;
        }
        public override void Initialise()
        {
            TextFunction = () => "";
            TextScale = Vector2.One;
        }
        public override void Update()
        {
            
        }

        public override void Draw(SpriteBatch s)
        {
            Drawing.DrawText(TextFunction(), TextPosition, s, font, TextColour, TextScale);
        }
    }
}
