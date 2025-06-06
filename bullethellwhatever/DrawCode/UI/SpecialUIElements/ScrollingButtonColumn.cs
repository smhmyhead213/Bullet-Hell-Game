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
using bullethellwhatever.AssetManagement;

namespace bullethellwhatever.DrawCode.UI.SpecialUIElements
{
    public class ScrollingButtonColumn : Menu
    {
        public float ScrollSpeed;
        public RenderTarget2D MenuRenderTarget;
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

        public override void Display()
        {
            base.Display();
            Size.Y = CalculateTotalHeight();
            //MenuRenderTarget = Drawing.CreateRTWithPreferredDefaults((int)Size.X, (int)Size.Y);
            MenuRenderTarget = Drawing.CreateRTWithPreferredDefaults(GameWidth / 2, GameHeight / 2);
        }
        public override void Draw(SpriteBatch s)
        {
            MainInstance.GraphicsDevice.SetRenderTarget(MenuRenderTarget);

            //base.Draw(s);

            //Drawing.DrawBox(Vector2.Zero, Color.Red, 1f);
            //Drawing.DrawBox(new Vector2(0f, Height()), Color.Red, 1f);
            //Drawing.DrawBox(new Vector2(Width(), 0f), Color.Red, 1f);
            //Drawing.DrawBox(new Vector2(Width(), Height()), Color.Red, 1f);

            Texture2D box = AssetRegistry.GetTexture2D("box");
            Vector2 origin = new Vector2(box.Width, box.Height) / 2f;
            s.Draw(box, Vector2.Zero, null, Color.Red, 0f, origin, Vector2.One, SpriteEffects.None, 0f);
            s.Draw(box, new Vector2(0f, MenuRenderTarget.Height), null, Color.Red, 0f, origin, Vector2.One, SpriteEffects.None, 0f);
            s.Draw(box, new Vector2(MenuRenderTarget.Width, 0f), null, Color.Red, 0f, origin, Vector2.One, SpriteEffects.None, 0f);
            s.Draw(box, new Vector2(MenuRenderTarget.Width, MenuRenderTarget.Height), null, Color.Red, 0f, origin, Vector2.One, SpriteEffects.None, 0f);

            s.End();
         
            MainInstance.GraphicsDevice.SetRenderTarget(MainRT);

            s.Begin();

            //Drawing.BetterDraw(MenuRenderTarget, Utilities.CentreOfScreen(), null, Color.White, 0f, Vector2.One, SpriteEffects.None, 1f);
            s.Draw(MenuRenderTarget, new Vector2(GameWidth / 2f, GameHeight / 2f), null, Color.White, 0f, new Vector2(MenuRenderTarget.Width, MenuRenderTarget.Height) / 2, Vector2.One, SpriteEffects.None, 1f);
        }
    }
}
