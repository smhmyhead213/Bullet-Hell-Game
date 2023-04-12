using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.UtilitySystems.Dialogue
{
    public struct DialogueObject
    {
        public Vector2 Position;
        public string Text;
        public bool DeleteNextFrame;
        public Entity Owner;

        public DialogueObject(Vector2 position, string text, Entity owner)
        {
            Position = position;
            Text = text;
            DeleteNextFrame = false;
            Owner = owner;

        }
    }
}
