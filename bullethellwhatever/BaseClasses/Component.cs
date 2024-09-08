using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.BaseClasses
{
    public abstract class Component
    {
        public Entity Owner;
        public abstract void Draw(SpriteBatch s);
        public abstract void Update();
    }
}
