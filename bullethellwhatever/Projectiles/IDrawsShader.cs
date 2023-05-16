using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Projectiles
{
    public interface IDrawsShader
    {
        public void DrawWithShader(SpriteBatch spriteBatch);
    }
}
