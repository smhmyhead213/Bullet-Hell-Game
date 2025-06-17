using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using bullethellwhatever.MainFiles;
using bullethellwhatever.AssetManagement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace bullethellwhatever.DrawCode.UI.SpecialUIElements
{
    public class ScrollingButtonColumn : Menu
    {
        public float ScrollSpeed;
        public float ScrollAmount;
        public RenderTarget2D MenuRenderTarget;
        public float TotalButtonHeight;
        public ScrollingButtonColumn(string texture, Vector2 size, Vector2 position, float scrollSpeed) : base(texture, size, position)
        {
            ScrollSpeed = scrollSpeed;
            ScrollAmount = 0;
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

        public override void Update()
        {
            base.Update();

            //if (AITimer == 30)
            //{
            //    //File.Create("test.jpg");
            //    Stream stream = File.OpenWrite("test.jpg");
            //    //string place = Path.GetFullPath("test.jpg");
            //    MenuRenderTarget.SaveAsJpeg(stream, MenuRenderTarget.Width, MenuRenderTarget.Height);
            //}
        }

        public override void AI()
        {
            if (IsKeyPressed(Keys.W))
            {
                // figure out if we are less than 1SS from the bottom
                float scrollDistance = Min(ScrollSpeed, TotalButtonHeight - ScrollAmount - Height());
                ScrollBy(scrollDistance);
                ScrollAmount += scrollDistance;                
            }
            
            if (IsKeyPressed(Keys.S))
            {
                // figure out if we are less than 1SS from the top
                float scrollDistance = Min(ScrollSpeed, ScrollAmount);
                ScrollBy(-scrollDistance);
                ScrollAmount -= scrollDistance;
            }

            foreach (UIElement uIElement in UIElements)
            {
                uIElement.Interactable = uIElement.BoundingBox().Intersects(InsideBoundingBox());
            }
        }

        public override void Display()
        {
            base.Display();
            //Size.Y = CalculateTotalHeight();

            if (Size.Y > 0)
                MenuRenderTarget = Drawing.CreateRTWithPreferredDefaults((int)Size.X, (int)Size.Y);
        }
        public override void Draw(SpriteBatch s)
        {
            s.End();

            MainInstance.GraphicsDevice.SetRenderTarget(MenuRenderTarget);

            s.Begin(samplerState: SamplerState.PointWrap); // this is UI so dont use camera

            MainInstance.GraphicsDevice.Clear(Color.Black);

            // cant just use positions to draw as we are drawing to a render target
            Drawing.BetterDraw(Texture, new Vector2(Size.X, Size.Y) / 2f, null, Colour * Opacity, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1);

            foreach (UIElement uiIElement in UIElements)
            {
                uiIElement.DrawAtPosition(s, uiIElement.PositionInMenu);
            }

            s.End();

            MainInstance.GraphicsDevice.SetRenderTarget(MainRT);

            s.Begin();

            Drawing.BetterDraw(MenuRenderTarget, Position, null, Color.White, 0f, Vector2.One, SpriteEffects.None, 1f);
            Drawing.DrawText(ScrollAmount.ToString(), TopLeft() - new Vector2(200f), s, font, Color.White, Vector2.One);
        }
    }
}
