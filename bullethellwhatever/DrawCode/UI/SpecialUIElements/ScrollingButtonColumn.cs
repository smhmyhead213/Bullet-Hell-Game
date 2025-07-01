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
        public Dictionary<UIElement, float> DefaultElementHeights;
        public Dictionary<string, float> ScrollCheckpoints;
        public Action AutoScroller;
        public int AutoScrollTimer;
        public ScrollingButtonColumn(string texture, Vector2 size, Vector2 position, float scrollSpeed) : base(texture, size, position)
        {
            ScrollSpeed = scrollSpeed;
            ScrollAmount = 0;
            ScrollCheckpoints = new Dictionary<string, float>();
        }

        public override bool VerticalSpaceAvailableFor(int row, float requestedHeight)
        {
            return true; // add as many things as we like vertically, because were gonna be hiding the ones that overflow
        }

        public void ScrollBy(float height)
        {
            ScrollAmount += height;
            ClampScroll();
        }

        public void ScrollTo(float height, int time)
        {
            // dont pass a direct ref to scrollamount
            float currentScroll = ScrollAmount;
            AutoScrollTimer = 0;

            AutoScroller = new Action(() =>
            {
                AutoScrollTimer++;

                ScrollAmount = MathHelper.Lerp(currentScroll, height, (float)AutoScrollTimer / time);

                if (AutoScrollTimer == time)
                {
                    CancelAutoScroll(); // KILL YOURSELF
                }
            });
        }

        public void ClampScroll()
        {
            ScrollAmount = MathHelper.Clamp(ScrollAmount, 0f, TotalButtonHeight - Height());
        }

        public override void Update()
        {
            base.Update();

            foreach (UIElement uIElement in UIElements)
            {
                uIElement.PositionInMenu.Y = DefaultElementHeights[uIElement] - ScrollAmount;
            }
            //if (AITimer == 30)
            //{
            //    //File.Create("test.jpg");
            //    Stream stream = File.OpenWrite("test.jpg");
            //    //string place = Path.GetFullPath("test.jpg");
            //    MenuRenderTarget.SaveAsJpeg(stream, MenuRenderTarget.Width, MenuRenderTarget.Height);
            //}

            if (AutoScroller is not null)
            {
                AutoScroller();
            }

            ClampScroll();
        }

        public override void HandleTab()
        {
            base.HandleTab();

            UIElement focused = UIManager.InteractableUIElement();

            if (UIElements.Contains(focused))
            {
                int index = UIElements.IndexOf(focused);

                Vector2 bottomRight = focused.BottomRight();

                if (!InsideBoundingBox().Contains(bottomRight))
                {
                    float scrollAmount = bottomRight.Y - LowestVisibleY(); //asjdkvasduyk
                    //ScrollBy(scrollAmount);
                    ScrollTo(ScrollAmount + scrollAmount, 10);
                }
            }
        }
        public override void AI()
        {
            if (KeyPressed(Keys.W))
            {
                // figure out if we are less than 1SS from the bottom
                //float scrollDistance = Min(ScrollSpeed, TotalButtonHeight - Height() - ScrollAmount);
                CancelAutoScroll();
                ScrollBy(ScrollSpeed);
            }
            
            if (KeyPressed(Keys.S))
            {
                // figure out if we are less than 1SS from the top
                //float scrollDistance = Min(ScrollSpeed, ScrollAmount);
                CancelAutoScroll();
                ScrollBy(-ScrollSpeed);
            }

            foreach (UIElement uIElement in UIElements)
            {
                // this is why some elements in scrolling menu are uninteractable
                uIElement.InteractabilityCondition = () => uIElement.BoundingBox().Intersects(InsideBoundingBox());
            }
        }

        public void CancelAutoScroll()
        {
            AutoScroller = null;
            AutoScrollTimer = 0;
        }

        public override void Display()
        {
            base.Display();
            //Size.Y = CalculateTotalHeight();

            if (Size.Y > 0)
                MenuRenderTarget = Drawing.CreateRTWithPreferredDefaults((int)Size.X, (int)Size.Y);

            TotalButtonHeight = CalculateTotalHeight();

            DefaultElementHeights = new Dictionary<UIElement, float>();
            
            foreach (UIElement uIElement in UIElements)
            {
                DefaultElementHeights.Add(uIElement, uIElement.PositionInMenu.Y);
            }
        }

        public float LowestVisibleY()
        {
            return Min(BottomRight().Y, GameHeight);
        }

        public float HighestVisibleY()
        {
            return Max(TopLeft().Y, 0);
        }

        public void AddScrollCheckPoint(string name, float scrollAmount)
        {
            if (ScrollCheckpoints.ContainsKey(name))
            {
                ScrollCheckpoints[name] = scrollAmount;
            }
            else
            {
                ScrollCheckpoints.Add(name, scrollAmount);
            }
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

            s.Begin(samplerState: SamplerState.PointWrap);
            
            Drawing.BetterDraw(MenuRenderTarget, Position, null, Color.White, 0f, Vector2.One, SpriteEffects.None, 1f);

            //foreach (UIElement uIElement in UIElements)
            //{
            //    Drawing.DrawBox(uIElement.BottomRight(), Color.Red, 1f);
            //}

            //Drawing.DrawText(ScrollAmount.ToString(), TopLeft() - new Vector2(200f), s, font, Color.White, Vector2.One);
        }
    }
}
