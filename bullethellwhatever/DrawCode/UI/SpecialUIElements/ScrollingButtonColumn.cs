using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using bullethellwhatever.MainFiles;
using SharpDX.Direct3D9;

namespace bullethellwhatever.DrawCode.UI.SpecialUIElements
{
    public class ScrollingButtonColumn : Menu
    {
        public float ScrollSpeed;
        
        public ScrollingButtonColumn(string texture, Vector2 size, Vector2 position, float scrollSpeed) : base(texture, size, position)
        {
            ScrollSpeed = scrollSpeed;
        }

        public override bool VerticalSpaceAvailableFor(int row, float requestedHeight)
        {
            return true; // add as many things as we like vertically, because were gonna be hiding the ones that overflow
        }

        public void ScrollBy(float height)
        {
            foreach (UIElement uIElement in UIElements)
            {
                uIElement.PositionInMenu -= new Vector2(0f, height);
            }
        }
       
        public override void AI()
        {
            if (IsKeyPressed(Keys.W))
            {
                ScrollBy(ScrollSpeed);
            }
            
            if (IsKeyPressed(Keys.S))
            {
                ScrollBy(-ScrollSpeed);
            }

            foreach (UIElement uIElement in UIElements)
            {
                uIElement.Interactable = uIElement.BoundingBox().Intersects(BoundingBox());
            }
        }

        public override void Draw(SpriteBatch s)
        {
            //MainInstance.GraphicsDevice.SetRenderTarget()
            base.Draw(s);
        }
    }
}
