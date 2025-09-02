using bullethellwhatever.DrawCode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.UtilitySystems
{
    public static class BoxDrawer
    {
        public static List<Vector2> BoxPositions = new List<Vector2>();
        public static void DrawBox(Vector2 position)
        {
            BoxPositions.Add(position);
        }
        public static void DrawBoxes()
        {
            foreach (Vector2 box in BoxPositions)
            {
                Drawing.DrawBox(box, Color.Red, 1f);
            }
        }
        public static void Clear()
        {
            BoxPositions.Clear();
        }
    }
}
