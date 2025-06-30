using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode.UI
{
    public abstract class UIComponent
    {
        public UIElement Owner;
        public UIComponent() 
        {
            Initialise();
        }
        public abstract void Initialise();

        public abstract void Update();

        public abstract void Draw(SpriteBatch s);
    }
}
