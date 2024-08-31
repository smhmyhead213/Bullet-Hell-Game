using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace bullethellwhatever.DrawCode.UI
{
    public class Menu : UIElement
    {
        public List<UIElement> UIElements;

        public bool Important; // whether or not stuff in game can happen while this menu is up

        public bool Draggable;

        public int TimeSinceLastDrag;

        public int IndexOfSelected
        {
            get;
            set;
        }

        public Menu(string texture, Vector2 size, Vector2 position) : base(texture, size, position)
        {
            Position = position;
            Size = size;
            Texture = AssetRegistry.GetTexture2D(texture);

            UIElements = new List<UIElement>();

            Name = "";

            PrepareMenu();
        }

        public void PrepareMenu()
        {          
            if (Texture == AssetRegistry.GetTexture2D("box"))
                Colour = Color.Black; // dont colour if none is specified
             
            Important = false;
            Draggable = false;

            TimeSinceLastDrag = 0;

            IndexOfSelected = -1;

            UIManager.ResetAllSelections();

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

        public bool IsDisplayed()
        {
            return UIManager.ActiveUIElements.Contains(this);
        }

        public override void Display()
        {
            UIManager.UIElementsToAddNextFrame.Add(this);
        }

        public override void Remove()
        {
            UIManager.IndexOfInteractable = UIManager.ActiveUIElements.IndexOf(this) - 1;
            base.Remove();
        }

        public override void Update()
        {
            base.Update();

            if (Draggable)
            {
                TimeSinceLastDrag++;

                if ((ClickBox.Contains(MousePosition) || TimeSinceLastDrag < 10) && IsLeftClickDown())
                {
                    TimeSinceLastDrag = 0;

                    // calculate the mouses relative position within the menu

                    Vector2 mouseRelativePosition = MousePosition - TopLeft();

                    Position = TopLeft() + mouseRelativePosition;

                    UIManager.IndexOfInteractable = UIManager.ActiveUIElements.IndexOf(this);
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

        public void UpdateSelectedElement()
        {
            if (IndexOfSelected == UIElements.Count - 1) // if we are at the last ui element
            {
                IndexOfSelected = 0; // no ui element selected
            }
            else
            {
                IndexOfSelected++; // if we are not at the last ui element, increment               
            }

            while (UIElements[IndexOfSelected] is InactiveElement) // if the newly selected ui element does nothing on click (change this condition to better represent any uninteractable element)
            {
                if (IndexOfSelected == UIElements.Count) // if we are at the last ui element
                {
                    IndexOfSelected = 0; // no ui element selected
                }
                else
                {
                    IndexOfSelected++; // go to next ui element
                }
            }
        }

        public override void HandleEnter()
        {
            if (IndexOfSelected >= 0)
            {
                UIElements[IndexOfSelected].HandleClick();
            }
        }
        public override void HandleTab()
        {
            UpdateSelectedElement();
        }
        public UIElement? GetSelectedElement()
        {
            if (IndexOfSelected >= 0)
            {
                return UIElements[IndexOfSelected];
            }
            else return null;
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

            //Utilities.drawTextInDrawMethod("Index of selected = " + IndexOfSelected.ToString(), Utilities.CentreOfScreen() / 4f + new Vector2(0f, 50f), s, font, Color.White);
        }
    }
}
