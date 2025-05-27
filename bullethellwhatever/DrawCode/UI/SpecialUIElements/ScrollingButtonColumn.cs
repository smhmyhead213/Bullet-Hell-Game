using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace bullethellwhatever.DrawCode.UI.SpecialUIElements
{
    public class ScrollingButtonColumn : Menu
    {
        public ScrollingButtonColumn(string texture, Vector2 size, Vector2 position) : base(texture, size, position)
        {

        }

        public override bool VerticalSpaceAvailableFor(int row, float requestedHeight)
        {
            return true; // add as many things as we like vertically, because were gonna be hiding the ones that overflow
        }
    }
}
