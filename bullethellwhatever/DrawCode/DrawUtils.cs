using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.DrawCode
{
    public static class DrawUtils
    {
        public static void DrawLine(Vector2 start, Vector2 end, float width, Color colour)
        {
            float direction = Utilities.VectorToAngle(end - start);
            float length = (end - start).Length();
            Texture2D tex = AssetRegistry.GetTexture2D("box");
            Drawing.BetterDraw(tex, start, null, colour, direction, new Vector2(width / tex.Width, length / tex.Height), SpriteEffects.None, 0f, new Vector2(width / 2, 0));
        }
    }
}
