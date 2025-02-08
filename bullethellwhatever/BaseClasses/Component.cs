using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.BaseClasses.Entities;

namespace bullethellwhatever.BaseClasses
{
    public abstract class Component
    {
        public string Name;
        public Entity Owner;
        public abstract void Draw(SpriteBatch s);
        public abstract void PreUpdate();
        public abstract void PostUpdate();
        public void SetName(string name)
        {
            Name = name;
        }
    }
}
