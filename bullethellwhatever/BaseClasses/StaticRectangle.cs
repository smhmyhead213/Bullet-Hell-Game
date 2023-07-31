using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace bullethellwhatever.BaseClasses
{
    public class StaticRectangle
    {
        public RectangleButGood Rectangle;
        public Vector2 Position;

        public StaticRectangle(RectangleButGood rectangle, Vector2 position)
        {
            Rectangle = rectangle;
            Position = position;
        }

        public StaticRectangle()
        {

        }
    }
}
