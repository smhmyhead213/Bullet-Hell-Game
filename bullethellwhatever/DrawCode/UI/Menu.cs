using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace bullethellwhatever.DrawCode.UI
{
    public class Menu : UIElement
    {
        public List<UIElement> UIElements;

        public bool Important; // whether or not stuff in game can happen while this menu is up

        public bool Draggable;

        public int TimeSinceLastDrag;

        public Menu(string texture, Vector2 size, Vector2 position) : base(texture, size, position)
        {
            Position = position;
            Size = size;
            Texture = AssetRegistry.GetTexture2D(texture);

            UIElements = new List<UIElement>();

            PrepareMenu();
        }

        public void PrepareMenu()
        {          
            if (Texture == AssetRegistry.GetTexture2D("box"))
                Colour = Color.Black; // dont colour if none is specified
             
            Important = false;
            Draggable = false;

            TimeSinceLastDrag = 0;

            UpdateClickBox();
        }
        public void SetImportant(bool important)
        {
            Important = important;
        }

        public void SetDraggable(bool draggable)
        {
            Draggable = draggable;
        }

        public void Display()
        {
            UIManager.UIElementsToAddNextFrame.Add(this);
        }

        public void Hide()
        {
            UIManager.UIElemntsToRemoveNextFrame.Add(this);
        }

        public override void Update()
        {
            if (Draggable)
            {
                TimeSinceLastDrag++;

                if ((ClickBox.Contains(MousePosition) || TimeSinceLastDrag < 10) && IsLeftClickDown())
                {
                    TimeSinceLastDrag = 0;

                    // calculate the mouses relative position within the menu

                    Vector2 mouseRelativePosition = MousePosition - TopLeft();

                    Position = TopLeft() + mouseRelativePosition;
                }
            }

            UpdateClickBox();

            foreach (UIElement uIElement in UIElements)
            {
                uIElement.Update();
            }
        }

        public void UpdateClickBox()
        {
            ClickBox = new RectangleButGood(TopLeft().X, TopLeft().Y, Width(), Height());
        }

        public void AddUIElements(UIElement[] uIElements)
        {
            foreach (UIElement uIElement in uIElements)
            {
                AddUIElement(uIElement);
            }
        }
        public void AddUIElement(UIElement uiElement)
        {
            uiElement.Owner = this;
            UIElements.Add(uiElement);
        }

        public float Width() => Size.X;

        public float Height() => Size.Y;

        public Vector2 RelativeCentreOfMenu() => new Vector2(Width(), Height()) / 2f;

        public Vector2 TopLeft() => Position - RelativeCentreOfMenu();
        public override void Draw(SpriteBatch s)
        {
            Drawing.BetterDraw(Texture, Position, null, Colour * Opacity, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1); // scale texture up to required size

            foreach (UIElement uiElement in UIElements)
            {
                uiElement.Draw(s);
            }
        }
    }
}
