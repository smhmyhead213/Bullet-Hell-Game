using bullethellwhatever.AssetManagement;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode.UI
{
    public class InactiveElement : UIElement
    {
        public InactiveElement(string texture, Vector2 size, Vector2 position = default) : base(texture, size, position)
        {

        }
        public InactiveElement(string texture, float size, Vector2 position = default) : base(texture, size, position)
        {

        }
        public override void Update()
        {
            if (ExtraAI is not null)
                ExtraAI();

            AITimer++;
        }
        public override bool CanBeClicked()
        {
            return false;
        }

        public override Color ColourIfSelected()
        {
            return Colour; // do not change colour if hovered
        }
    }
}
